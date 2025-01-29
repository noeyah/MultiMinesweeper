using Packet;

namespace Server;

internal class Board
{
	private readonly Cell[,] _grid;
	private readonly int _size;
	private readonly int _mineCount;

	private List<(int row, int col)> _mines = new List<(int row, int col)>();
	private int _flagCount = 0;
	private int _openCount = 0;

	public int OpenCellCount => _openCount;
	public int FlagCellCount => _flagCount;

	public Board(int size, int mineCount)
	{
		_size = size;
		_mineCount = mineCount;

		_grid = new Cell[size, size];
		for (int row = 0; row < size; row++)
		{
			for (int col = 0; col < size; col++)
			{
				_grid[row, col] = new Cell(row, col);
			}
		}
	}

	public void Reset()
	{
		_flagCount = 0;
		_openCount = 0;

		// cell 초기화 + 지뢰심기
		PlantMines();

		// 인접한 지뢰 개수 세팅
		SetAdjacentMines();
	}

	public void OpenCell(int row, int col, ref List<GameCell> update_cells)
	{
		var cell = _grid[row, col];
		if ( cell.State != CELL_STATE.CLOSE)
		{
			return;
		}

		cell.Open();
		update_cells.Add(cell.GetGameCell());

		// 지뢰가 아니면
		if ( !cell.IsMine )
		{
			_openCount++;

			// 주변 지뢰 개수가 0개이면 연쇄 오픈
			if (cell.AdjacentMineCount == 0)
			{
				ChainOpen(row, col, ref update_cells);
			}
		}
	}

	public GameCell? FlagCell(int row, int col, bool new_flag)
	{
		var cell = _grid[row, col];
		var state = cell.State;

		if (state != CELL_STATE.FLAG && state != CELL_STATE.CLOSE )
		{
			return null;
		}

		// Flag -> Close / Close -> Flag 체크
		var current_flag = state == CELL_STATE.FLAG;
		if (new_flag == current_flag)
		{
			return null;
        }

		cell.SetFlag(new_flag);

		if (new_flag)
		{
			_flagCount++;
		}
		else
		{
			_flagCount--;
		}

		return cell.GetGameCell();
	}

	private void ChainOpen(int cellRow, int cellCol, ref List<GameCell> update_cells)
	{
		// cellRow -1, cellRow, cellRow +1
		// cellCol -1, cellCol, cellCol +1
		for (int row = Math.Max(0, cellRow - 1); row <= Math.Min(_size - 1, cellRow + 1); row++)
		{
			for (int col = Math.Max(0, cellCol - 1); col <= Math.Min(_size - 1, cellCol + 1); col++)
			{
				if (_grid[row, col].State == CELL_STATE.CLOSE)
				{
					OpenCell(row, col, ref update_cells);
				}
			}
		}
	}

	public CELL_STATE GetCellState(int row, int col)
	{
		return _grid[row, col].State;
	}

	// 유효하지 않는 인덱스
	public bool InvalidCellIndex(int row, int col)
	{
		return row < 0
			|| row >= _size
			|| col < 0
			|| col >= _size;
	}


	#region 준비 함수
	private void PlantMines()
	{
		_mines.Clear();

		// cell 초기화 및 리스트로
		var cells = new List<(int row, int col)>();
		for (int row = 0; row < _size; row++)
		{
			for (int col = 0; col < _size; col++)
			{
				var cell = _grid[row, col];
				cell.Reset();
				cells.Add(cell.Position);
			}
		}

		// 셀 섞기
		var rand = new Random();
		for ( int i = cells.Count - 1; i >= 0; i--)
		{
			int j = rand.Next(i + 1);
			(cells[i], cells[j]) = (cells[j], cells[i]);
		}

		// _mineCount만큼 지뢰 뽑음
		_mines = cells.GetRange(0, _mineCount);

		// 지뢰 세팅
		foreach (var mine in _mines)
		{
			var cell = _grid[mine.row, mine.col];
			cell.SetMine(true);
		}
	}

	// 인접한 지뢰 개수 세팅
	private void SetAdjacentMines()
	{
		for (int row = 0; row < _size; row++)
		{
			for (int col = 0; col < _size; col++)
			{
				var cell = _grid[row, col];
				if (cell.IsMine)
				{
					continue;
				}
				cell.SetMineCount(CalcAdjacentMineCount(row, col));
			}
		}
	}

	// 인접한 지뢰 개수 계산
	private int CalcAdjacentMineCount(int cellRow, int cellCol)
	{
		int minesCount = 0;
		// cellRow -1, cellRow, cellRow +1
		// cellCol -1, cellCol, cellCol +1
		for (int row = Math.Max(0, cellRow - 1); row <= Math.Min(_size - 1, cellRow + 1); row++)
		{
			for (int col = Math.Max(0, cellCol - 1); col <= Math.Min(_size - 1, cellCol + 1); col++)
			{
				if (_grid[row, col].IsMine)
				{
					minesCount++;
				}
			}
		}
		return minesCount;
	}
	#endregion

	#region 패킷용
	public GameCell[,] GetGameCells()
	{
		var board = new GameCell[_size, _size];
		for (int row = 0; row < _size; row++)
		{
			for (int col = 0; col < _size; col++)
			{
				board[row, col] = _grid[row,col].GetGameCell();
			}
		}
		return board;
	}

	public List<GameCell> GetAllMineGameCell()
	{
		var mineCells = new List<GameCell>();

		foreach (var mine in _mines)
		{
			var cell = _grid[mine.row, mine.col];
			mineCells.Add(cell.GetGameCell());
		}

		return mineCells;
	}
	#endregion

	// 테스트용
	public bool CheckWinByBoard()
	{
		for (int row = 0; row < _size; row++)
		{
			for (int col = 0; col < _size; col++)
			{
				var cell = _grid[row, col];
				var state = cell.State;

				if (state == CELL_STATE.OPEN)
				{
					continue;
				}

				if (state == CELL_STATE.MINE)
				{
					// 버그 발생
					return false;
				}

				if ( cell.IsMine )
				{
					continue;
				}

				// 지뢰가 아닌데 플래그or닫힘 -> 게임 안끝남
				return false;
			}
		}
		return true;
	}
}
