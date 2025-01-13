using Packet;

namespace Server;

internal class GameRoom
{
	private readonly Board _board;
	private readonly int _size;

	private bool _gameOver;
	private bool _win;

	public bool GameOver => _gameOver;
	public bool Win => _win;

	public GameRoom(int size, int mineCount)
	{
		_board = new Board(size, mineCount);
		_size = size;

		_gameOver = false;
		_win = false;
	}

	public void Reset()
	{
		_board.Reset();
		_gameOver = false;
		_win = false;
	}

	public void OpenCell(int row, int col, ref List<GameCell> updateCells)
	{
		if ( _gameOver )
		{
			return;
		}

		// 오픈 & 0이면 연쇄 오픈까지
		_board.OpenCell(row, col, ref updateCells);

		// 오픈한 셀이 지뢰면 게임 오버
		var state = _board.GetCellState(row, col);
		if ( state == CELL_STATE.MINE )
		{
			_gameOver = true;
			_win = false;
			return;
		}

		CheckWin();
	}

	public GameCell FlagCell(int row, int col, bool flag)
	{
		if (_gameOver)
		{
			return null;
		}

		return _board.FlagCell(row, col, flag);
	}

	#region 패킷용
	public GameInfo GetGameInfo()
	{
		var gameInfo = new GameInfo();
		gameInfo.BoardSize = _size;
		gameInfo.TotalMineCount = _board.TotalMineCount;
		gameInfo.RemainMineCount = _board.RemainMineCount_Player;
		gameInfo.Board = _board.GetGameCells();
		gameInfo.GameOver = _gameOver;
		gameInfo.Win = _win;

		return gameInfo;
	}

	public List<GameCell> GetAllMineGameCell()
	{
		return _board.GetAllMineGameCell();
	}

	public int GetRemainMineCount_Player()
	{
		return _board.RemainMineCount_Player;
	}

	#endregion

	public bool CheckIndex(int row, int col)
	{
		return row >= 0 
			&& row < _size 
			&& col >= 0
			&& col < _size;
	}

	private void CheckWin()
	{
		if (_gameOver)
		{
			return;
		}

		var remainMines = _board.RemainMineCount_Real;
		var remainClose = _board.CloseCellCount;
		var flagCount = _board.FlagCellCount;

		if ( remainMines == (remainClose - flagCount) )
		{
			_gameOver = true;
			_win = true;
		}
	}
}
