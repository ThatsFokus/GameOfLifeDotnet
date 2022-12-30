using SkiaSharp;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using System.Collections.Generic;
class MyGame
{
	private int SizeX;
	private int SizeY;
	private IWindow window;
	private SKSurface surface;
	private SKCanvas canvas;
	private GL gl;

	private SKPaint paint;
	private List<Key> pressedKeys;
	private bool mouse1Pressed;
	private CircleOfLife circleOfLife;
	private System.Numerics.Vector2 mousePosition;
	private List<Cell> wereChanged;
	private float simulationSpeed;
	private bool simulationRunning;
	private float simulationTimer;
	private string title;

	public MyGame(int width, int height, string title)
	{
		var options = WindowOptions.Default;
		this.title = title;
		SizeX = width;
		SizeY = height;
		options.Title = title;
		options.Size = new Silk.NET.Maths.Vector2D<int>(width, height);
		options.ShouldSwapAutomatically = false;
		window = Window.Create(options);
		window.FramesPerSecond = 60;
		window.Update += OnUpdate;
		window.Render += OnRender;
		window.Load += OnLoad;
		window.Closing += onClosing;
	}

	private void OnUpdate(double arg1)
	{
		window.Title = title + "   Simulation Speed:" + (simulationSpeed * 100).ToString("0.00") + "%";
		window.Title += simulationRunning ? "    RUNNING" : "    PAUSED";
		if (simulationRunning)
		{
			if (simulationTimer < 0)
			{
				circleOfLife.SimulateLive();
				simulationTimer += 1;
			}
			simulationTimer -= (float)arg1 * simulationSpeed;
		}
		else
		{
			simulationTimer = 1;
			if (mouse1Pressed)
			{
				int x = (int)((mousePosition.X / Cell.Size) - ((mousePosition.X / Cell.Size) % 1f));
				int y = (int)((mousePosition.Y / Cell.Size) - ((mousePosition.Y / Cell.Size) % 1f));
				circleOfLife.Field[x, y].Change();
				if (!wereChanged.Contains(circleOfLife.Field[x, y]))
				{
					wereChanged.Add(circleOfLife.Field[x, y]);
				}
			}
		}
	}

	private void onFileDrop(string[] arg1)
	{
		if (arg1.Count() == 0) return;

		circleOfLife.LoadFromFile(arg1[0]);
	}

	private void OnRender(double arg1)
	{
		drawCells();
		swapBuffers();
	}

	private void OnLoad()
	{
		//create and bind input context
		var input = window.CreateInput();

		window.FileDrop += onFileDrop;

		foreach (var keyboard in input.Keyboards)
		{
			keyboard.KeyDown += OnKeyDown;
			keyboard.KeyUp += OnKeyUp;
		}

		foreach (var mouse in input.Mice)
		{
			mouse.MouseDown += OnMouseDown;
			mouse.MouseUp += OnMouseUp;
			mouse.MouseMove += OnMouseMove;
			mouse.Scroll += OnMouseScroll;
		}

		//create and configure OpenGL context
		gl = window.CreateOpenGL();
		gl.ClearColor(1f, 1f, 1f, 1f);

		//create SkiaSharp context
		var grinterface = GRGlInterface.CreateOpenGl(name =>
		{
			if (window.GLContext.TryGetProcAddress(name, out nint fn)) return fn;
			return (nint)0;
		});

		var skiabackendcontext = GRContext.CreateGl(grinterface);
		var format = SKColorType.Rgba8888;
		var backendrendertarget = new GRBackendRenderTarget(window.Size.X, window.Size.Y, window.Samples ?? 1, window.PreferredStencilBufferBits ?? 16, new GRGlFramebufferInfo(
			0, format.ToGlSizedFormat()
		));
		//var info = new SKImageInfo(window.Size.X, window.Size.Y);
		//surface = SKSurface.Create(info);
		surface = SKSurface.Create(skiabackendcontext, backendrendertarget, format);
		canvas = surface.Canvas;
		var typeface = SKTypeface.CreateDefault();
		paint = new SKPaint(new SKFont(typeface));

		//add gameObjects
		//add ground
		createObjects();
	}

	public void Run()
	{
		window.Run();
	}

	private void drawCells()
	{
		var background = new SKPaint() { Color = SKColors.DimGray };
		var alive = new SKPaint() { Color = Cell.AliveColor };
		var dead = new SKPaint() { Color = Cell.DeadColor };
		for (int x = 0; x < circleOfLife.SizeX; x++)
		{
			for (int y = 0; y < circleOfLife.SizeY; y++)
			{
				canvas.DrawRect(x * Cell.Size, y * Cell.Size, Cell.Size, Cell.Size, background);
				canvas.DrawRect(x * Cell.Size + 1, y * Cell.Size + 1, Cell.Size - 2, Cell.Size - 2, circleOfLife.Field[x, y].IsAlive ? alive : dead);
			}
		}
	}

	private void OnKeyDown(IKeyboard arg1, Key arg2, int arg3)
	{
		if (arg2 == Key.Space) simulationRunning = !simulationRunning;
		if (arg2 == Key.Escape) window.Close();
		if (simulationRunning) return;

		if (arg2 == Key.Backspace) circleOfLife.killAllLive();
		if (arg2 == Key.R) circleOfLife.generateRandom(25);
		if (arg1.IsKeyPressed(Key.ControlLeft) && arg2 == Key.S)
		{
			circleOfLife.SaveToFile();
		}
	}

	private void OnKeyUp(IKeyboard arg1, Key arg2, int arg3)
	{

	}

	private void OnMouseMove(IMouse arg1, System.Numerics.Vector2 arg2)
	{
		mousePosition = arg1.Position;
	}

	private void OnMouseDown(IMouse arg1, MouseButton arg2)
	{
		if (MouseButton.Left == arg2) mouse1Pressed = true;
	}

	private void OnMouseUp(IMouse arg1, MouseButton arg2)
	{
		if (MouseButton.Left == arg2)
		{
			mouse1Pressed = false;
			setAllChangedFalse();
		}
	}

	private void setAllChangedFalse()
	{
		/*for (int x = 0; x < circleOfLife.SizeX; x++){
			for (int y = 0; y < circleOfLife.SizeY; y++){
				circleOfLife.Field[x, y].Reset();
			}
		}*/

		wereChanged.ForEach(delegate (Cell cell) { cell.Reset(); });
		wereChanged.Clear();
	}

	private void OnMouseScroll(IMouse arg1, ScrollWheel arg2)
	{
		if (arg2.Y < 0)
		{
			simulationSpeed -= 0.05f;
		}
		else if (arg2.Y > 0)
		{
			simulationSpeed += 0.05f;
		}
	}



	private void swapBuffers()
	{
		canvas.Flush();
		window.SwapBuffers();
		canvas.Clear(SKColors.DarkSlateGray); // set the background color here
	}

	private void createObjects()
	{
		//create all variables
		pressedKeys = new List<Key>();
		circleOfLife = new CircleOfLife(SizeX / Cell.Size, SizeY / Cell.Size);
		mouse1Pressed = false;
		wereChanged = new List<Cell>();
		simulationSpeed = 1f;
		simulationRunning = false;
	}

	void onClosing()
	{
	}
}
