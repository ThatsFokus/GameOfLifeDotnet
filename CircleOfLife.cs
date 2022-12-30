public class CircleOfLife
{
	private Cell[,] field;
	public int SizeX
	{
		get { return field.GetLength(0); }
	}
	public int SizeY
	{
		get { return field.GetLength(1); }
	}
	public Cell[,] Field
	{
		get { return field; }
	}
	public CircleOfLife(int width, int height)
	{
		field = new Cell[width, height];
		for (int w = 0; w < width; w++)
		{
			for (int h = 0; h < height; h++)
			{
				field[w, h] = new Cell();
			}
		}
	}


	public void generateRandom(double chanceForLifeInPercent)
	{
		chanceForLifeInPercent /= 100;
		Random random = new Random((int)(DateTime.UtcNow.Ticks % int.MaxValue));
		field = new Cell[SizeX, SizeY];
		for (int w = 0; w < SizeX; w++)
		{
			for (int h = 0; h < SizeY; h++)
			{
				field[w, h] = random.NextDouble() <= chanceForLifeInPercent ? new Cell(true) : new Cell();
			}
		}
	}

	public void LoadFromFile(string path)
	{
		string[] read = System.IO.File.ReadAllLines(path);
		stringTofield(read);
	}

	private string[] fieldToString()
	{
		List<string> lines = new List<string>();
		for (int y = 0; y < SizeY; y++)
		{
			string text = "";
			for (int x = 0; x < SizeX; x++)
			{
				text += field[x, y].IsAlive ? "1" : "0";
			}
			lines.Add(text);
		}
		return lines.ToArray();
	}

	private void stringTofield(string[] data)
	{
		string[] newArray = new string[SizeY];
		newArray.Initialize();
		for (int i = 0; i < data.Length; i++)
		{
			newArray[i] = data[i];
		}
		for (int i = 0; i < newArray.Length; i++)
		{
			while (newArray[i].Length < SizeX)
			{
				newArray[i] += "0";
			}
		}
		for (int w = 0; w < SizeX; w++)
		{
			for (int h = 0; h < SizeY; h++)
			{
				field[w, h] = new Cell(newArray[h][w] == '1');
			}
		}
	}

	public void SaveToFile()
	{
		string[] lines = fieldToString();
		System.IO.Directory.CreateDirectory("./positions");
		System.IO.File.WriteAllLines("./positions/newPosition.txt", lines);

	}

	public void killAllLive()
	{
		for (int w = 0; w < SizeX; w++)
		{
			for (int h = 0; h < SizeY; h++)
			{
				field[w, h] = new Cell();
			}
		}
	}
	public void SimulateLive()
	{
		List<Cell> toChange = new List<Cell>();
		for (int x = 0; x < SizeX; x++)
		{
			for (int y = 0; y < SizeY; y++)
			{
				int livingNeighbors = amountOfLivingNeighbors(x, y);
				if (!field[x, y].IsAlive && livingNeighbors == 3) toChange.Add(field[x, y]);
				if (field[x, y].IsAlive && livingNeighbors < 2) toChange.Add(field[x, y]);
				if (field[x, y].IsAlive && livingNeighbors > 3) toChange.Add(field[x, y]);
			}
		}

		toChange.ForEach(delegate (Cell cell) { cell.Change(); });
		toChange.ForEach(delegate (Cell cell) { cell.Reset(); });
	}

	private int amountOfLivingNeighbors(int x, int y)
	{
		int amount = 0;
		for (int i = -1; i < 2; i++)
		{
			if (x + i < 0) continue;
			if (x + i >= SizeX) continue;
			for (int j = -1; j < 2; j++)
			{
				if (i == 0 && j == 0) continue;
				if (y + j < 0) continue;
				if (y + j >= SizeY) continue;
				if (field[x + i, y + j].IsAlive) amount += 1;
			}
		}
		return amount;
	}
}
