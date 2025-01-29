using Packet;
using System.Collections.Concurrent;

namespace Server;

internal class GameRoom : RoomWorker
{
	private readonly GamePlayerPool _playerPool = new GamePlayerPool();
	private readonly ConcurrentDictionary<int, GamePlayer> _dicPlayers = new();

	private readonly Board _board;
	private readonly int _size;
	private readonly int _mineCount;
	private readonly ROOM_LEVEL _level;

	private bool _gameOver;
	private bool _win;

	private object _lock = new object();

	public bool GameOver => _gameOver;
	public bool Win => _win;
	public ROOM_LEVEL Level => _level;

	public GameRoom(ROOM_LEVEL level) : base()
	{
		_level = level;
		_size = Utils.GetBoardSize(level);
		_mineCount = Utils.GetMineCount(level);
		_board = new Board(_size, _mineCount);

		_gameOver = false;
		_win = false;

		_board.Reset();
	}

	public Player JoinPlayer(int sessionID, string name)
	{
		var player = _playerPool.Get(sessionID, name);
		_dicPlayers[sessionID] = player;

		return player.GetPlayer();
	}

	public void LeavePlayer(int sessionID)
	{
		if (_dicPlayers.TryRemove(sessionID, out var player))
		{
			_playerPool.Return(player);
		}
	}

	public void OpenCell(int row, int col, Action<ERROR_CODE, List<GameCell>> callback)
	{
		Enqueue(() =>
		{
			var openCells = new List<GameCell>();
			var errorCode = OpenCell(row, col, ref openCells);
			callback(errorCode, openCells);
		});
	}

	public void FlagCell(int row, int col, bool flag, Action<ERROR_CODE, GameCell?> callback)
	{
		Enqueue(() =>
		{
			(var errorCode, var flagCell) = FlagCell(row, col, flag);
			callback(errorCode, flagCell);
		});
	}

	public ERROR_CODE Reset()
	{
        lock (_lock)
		{
			if (!_gameOver)
			{
				return ERROR_CODE.FAIL_RESET_NOT_GAME_OVER;
			}

			_board.Reset();
			_gameOver = false;
			_win = false;

			return ERROR_CODE.OK;
		}
	}

	#region 게임
	private ERROR_CODE OpenCell(int row, int col, ref List<GameCell> update_cells)
	{
		if (_gameOver)
		{
			return ERROR_CODE.FAIL_ALREADY_GAME_OVER;
		}

		var check_error_code = CheckOpenCell(row, col);
		if (check_error_code != ERROR_CODE.OK)
		{
			return check_error_code;
		}

		// 오픈 & 0이면 연쇄 오픈까지
		_board.OpenCell(row, col, ref update_cells);

		// 오픈한 셀이 지뢰면 게임 오버
		var state = _board.GetCellState(row, col);

		if (state == CELL_STATE.MINE)
		{
			_gameOver = true;
			_win = false;
		}
		else
		{
			CheckWin();
		}

		return ERROR_CODE.OK;
	}

	private (ERROR_CODE, GameCell?) FlagCell(int row, int col, bool flag)
	{
		GameCell? flagCell = null;

		if (_gameOver)
		{
			return (ERROR_CODE.FAIL_ALREADY_GAME_OVER, null);

		}

		var check_error_code = InvalidFlag(row, col, flag);

		if (check_error_code != ERROR_CODE.OK)
			{
				return (check_error_code, null);
		}

		flagCell = _board.FlagCell(row, col, flag);

		return (ERROR_CODE.OK, flagCell);
	}

	private ERROR_CODE CheckOpenCell(int row, int col)
	{
		if (_board.InvalidCellIndex(row, col))
		{
			return ERROR_CODE.FAIL_CELL_INDEX;
		}

		var state = _board.GetCellState(row, col);
		if (state != CELL_STATE.CLOSE)
		{
			return ERROR_CODE.FAIL_NOT_CLOSE_CELL;
		}

		return ERROR_CODE.OK;
	}

	private ERROR_CODE InvalidFlag(int row, int col, bool new_flag)
	{
		if (_board.InvalidCellIndex(row, col))
		{
			return ERROR_CODE.FAIL_CELL_INDEX;
		}

		var state = _board.GetCellState(row, col);

		if (state != CELL_STATE.FLAG && state != CELL_STATE.CLOSE)
		{
			return ERROR_CODE.FAIL_INVALID_FLAG;
		}

		var current_flag = state == CELL_STATE.FLAG;
		if (new_flag == current_flag)
		{
			return ERROR_CODE.FAIL_ALREADY_FLAG;
		}

		return ERROR_CODE.OK;
	}

	private void CheckWin()
	{
		if (_gameOver)
		{
			return;
		}

		// 지뢰를 제외한 cell 모두 오픈하면 승리
		var totalCellCount = _size * _size;
		var openCount = _board.OpenCellCount;

		if (totalCellCount - _mineCount == openCount)
		{
			// 혹시 모르니까 한번씩 확인해보장... 
			if (!_board.CheckWinByBoard())
			{
				Console.WriteLine($"버그 발생!!!! 승리가 아닌데 승리로 체크됨!!!");
			}
			_gameOver = true;
			_win = true;
		}
	}
	#endregion

	#region 패킷용
	// 방 현재 진행 상황
	public RoomInfo GetRoomInfo()
	{
		var gameInfo = new RoomInfo();

		lock (_lock)
		{
			gameInfo.BoardSize = _size;
			gameInfo.TotalMineCount = _mineCount;
			gameInfo.RemainMineCount = GetRoomRemainMineCount();
			gameInfo.Board = _board.GetGameCells();
			gameInfo.GameOver = _gameOver;
			gameInfo.Win = _win;
		}

		return gameInfo;
	}

	// 방에 참여중인 플레이어 리스트
	public List<Player> GetRoomPlayers()
	{
		return _dicPlayers.Values.Select(v => v.GetPlayer()).ToList();
	}

	public ICollection<int> GetRoomSessionIDs()
	{
		return _dicPlayers.Keys;
	}

	// 지뢰 셀
	public List<GameCell> GetAllMineGameCell()
	{
		return _board.GetAllMineGameCell();
	}

	// 유저들 입장에서 남은 지뢰 개수 (실제 남은 지뢰 개수랑 다름)
	public int GetRoomRemainMineCount()
	{
		return _mineCount - _board.FlagCellCount;
	}

	#endregion

}
