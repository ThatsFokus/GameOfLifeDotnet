using SkiaSharp;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;
class Mygame
{
	public static readonly int SizeX = 1280;
	public static readonly int SizeY = 720;
	private IWindow window;
	private SKSurface surface;
	private SKCanvas canvas;
	private GL gl;

	private SKPaint paint;
	private List<Key> pressedKeys;

	public Mygame(int width, int height, string title){
		var options = WindowOptions.Default;
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

	private void OnUpdate(double arg1){
		
	}

	private void OnRender(double arg1){
		swapBuffers();
	}

	private void OnLoad(){
		//create and bind input context
		var input = window.CreateInput();

		foreach (var keyboard in input.Keyboards){
			keyboard.KeyDown += OnKeyDown;
			keyboard.KeyUp += OnKeyUp;
		}

		foreach (var mouse in input.Mice){
			mouse.MouseDown += OnMouseDown;
			mouse.MouseMove += OnMouseMove;
			mouse.Scroll += OnMouseScroll;
		}

		//create and configure OpenGL context
		gl = window.CreateOpenGL();
		gl.ClearColor(1f, 1f, 1f, 1f);

		//create SkiaSharp context
		var grinterface = GRGlInterface.CreateOpenGl(name => {
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

	public void Run(){
		window.Run();
	}

	private void OnKeyDown(IKeyboard arg1, Key arg2, int arg3){
		if (arg2 == Key.Escape) window.Close();
		if (!pressedKeys.Contains(arg2)) pressedKeys.Add(arg2);
	}

	private void OnKeyUp(IKeyboard arg1, Key arg2, int arg3){
		pressedKeys.Remove(arg2);
	}

	private void OnMouseMove(IMouse arg1, System.Numerics.Vector2 arg2){

	}

	private void OnMouseDown(IMouse arg1, MouseButton arg2){

	}

	private void OnMouseScroll(IMouse arg1, ScrollWheel arg2){

	}
	


	private void swapBuffers(){
		canvas.Flush();
		window.SwapBuffers();
		canvas.Clear(SKColors.DarkSlateGray); // set the background color here
	}

	private void createObjects(){
		//create all variables
		pressedKeys = new List<Key>();
	}

	void onClosing(){
	}
}