using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packet;


[MessagePackObject]
public class GameInfo
{
	[Key(0)]
	public int BoardSize;
	[Key(1)]
	public int TotalMineCount;
	[Key(2)]
	public GameCell[,] Board;
	[Key(3)]
	public int RemainMineCount;
	[Key(4)]
	public bool GameOver;
	[Key(5)]
	public bool Win;
}

[MessagePackObject]
public class GameCell
{
	[Key(0)]
	public int Row;
	[Key(1)]
	public int Col;
	[Key(2)]
	public CELL_STATE State;
	[Key(3)]
	public int AdjacentMineCount;
}


[MessagePackObject]
public class Player
{
	[Key(0)]
	public int Index;
	[Key(1)]
	public string Name;
}

