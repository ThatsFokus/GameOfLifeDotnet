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
				field[w,h] = new Cell();
			}
		}
	}

	public void killAllLive(){
		for (int w = 0; w < SizeX; w++){
			for (int h = 0; h < SizeY; h++){
				field[w,h] = new Cell();
			}
		}
	}
	public void SimulateLive(){
		List<Cell> toChange = new List<Cell>();
		for (int x = 0; x < SizeX; x++){
			for (int y = 0; y < SizeY; y++){
				int livingNeighbors = amountOfLivingNeighbors(x,y);
				if(!field[x,y].IsAlive && livingNeighbors == 3) toChange.Add(field[x, y]);
				if(field[x,y].IsAlive && livingNeighbors < 2) toChange.Add(field[x, y]);
				if(field[x,y].IsAlive && livingNeighbors > 3) toChange.Add(field[x, y]);
			}
		}

		toChange.ForEach(delegate(Cell cell) {cell.Change(); });
		toChange.ForEach(delegate(Cell cell) {cell.Reset(); });
	}

	private int amountOfLivingNeighbors(int x, int y){
		int amount = 0;
		for(int i = -1; i < 2; i++){
			if (x + i < 0) continue;
			if (x + i >= SizeX) continue;
			for(int j = -1; j < 2; j++){
				if(i == 0 && j == 0) continue;
				if(y + j < 0) continue;
				if(y + j >= SizeY) continue;
				if(field[x + i, y + j].IsAlive) amount += 1;
			}
		}
		return amount;
	}
}
