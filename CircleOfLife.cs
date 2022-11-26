public class CircleOfLife{
	private Cell[,] field;
	public int SizeX{
		get{return field.GetLength(0);}
	}
	public int SizeY{
		get{return field.GetLength(1);}
	}
	public Cell[,] Field{
		get{return field;}
	}
	public CircleOfLife(int width, int height){
		field  = new Cell[width, height];
		for (int w = 0; w < width; w++){
			for (int h = 0; h < height; h++){
				field[w,h] = new Cell(w * 10, h * 10);
			}
		}
	}

	public void SimulateLive(){
		
	}
}