using SkiaSharp;
public class Cell{
	private bool isAlive;
	private static SKColor aliveColor = SKColors.MediumSeaGreen;
	private static int size = 10;
	public static int Size{
		get {return size; }
	}
	public static SKColor AliveColor{
		get{ return aliveColor;}
	}
	private static SKColor deadColor = SKColors.Crimson;
	public static SKColor DeadColor{
		get{ return deadColor;}
	}
	private bool hasChanged;
	public bool IsAlive{
		get{return isAlive;}
	}

	public Cell(int x, int y){
		isAlive = false;
	}

	public void Change(){
		if(hasChanged) return;
		hasChanged = true;
		isAlive = !isAlive;
	}

	public void Reset(){
		hasChanged = false;
	}
}