using Packet;

namespace Server;

internal class Cell
{
	private readonly int _row;
	private readonly int _col;

	private bool _isMine;
	private int _adjacentMineCount;

	private bool _isFlag;
	private bool _isOpen;

	public (int row, int col) Position => (_row, _col);
	public bool IsMine => _isMine;
	public bool IsFlag => _isFlag;
	public int AdjacentMineCount => _adjacentMineCount;

	public Cell(int row, int col)
	{
		_row = row;
		_col = col;
	}

	public void Reset()
	{
		_isMine = false;
		_adjacentMineCount = -1;
		_isFlag = false;
		_isOpen = false;
	}

	public void SetMine(bool isMine)
	{
		_isMine = isMine;
	}

	public void SetMineCount(int minesCount)
	{
		_adjacentMineCount = minesCount;
	}

	public void SetFlag(bool flag)
	{
		_isFlag = flag;
	}

	public void Open()
	{
		_isOpen = true;
	}

	public GameCell GetGameCell()
	{
		var gameCell = new GameCell();
		gameCell.Row = _row;
		gameCell.Col = _col;
		gameCell.State = GetState();

		if (GetState() == CELL_STATE.OPEN)
		{
			gameCell.AdjacentMineCount = _adjacentMineCount;
		}
		return gameCell;
	}

	public CELL_STATE GetState()
	{
		if (_isFlag)
		{
			return CELL_STATE.FLAG;
		}
		else if (_isOpen)
		{
			if (_isMine)
			{
				return CELL_STATE.MINE;
			}
			else
			{
				return CELL_STATE.OPEN;
			}
		}

		return CELL_STATE.CLOSE;
	}
}
