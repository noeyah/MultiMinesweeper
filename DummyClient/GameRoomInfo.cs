
using Packet;

namespace DummyClient;

internal class GameRoomInfo
{
	public readonly int Size;
	public GameCell[,] Board;
	public bool GameOver;
	public bool Win;

	private List<GameCell> cells;

	public GameRoomInfo(int size, GameCell[,] board, bool gameOver, bool win)
	{
		Size = size;

		Board = board;
		BoardToList();

		GameOver = gameOver;
		Win = win;
	}

	public void Reset(GameCell[,] board)
	{
		Board = board;
		BoardToList();

		GameOver = false;
		Win = false;
	}

	public void SetCellState(int row, int col, CELL_STATE state)
	{
		if (row < 0
			|| row >= Size 
			|| col < 0
			|| col >= Size)
		{
			return;
		}

		Board[row, col].State = state;

		if ( state == CELL_STATE.MINE
			|| state == CELL_STATE.OPEN )
		{
			cells.Remove(Board[row, col]);
		}
	}

	public (int row, int col) GetRandomCloseCell()
	{
		Random rand = new Random();

		var list = cells.Where(v => v.State == CELL_STATE.CLOSE).ToList();
		var randIndex = rand.Next(0, list.Count);
		var cell = list[randIndex];

		return (cell.Row, cell.Col);
	}

	public (int row, int col, bool currFlag) GetRandomFlagCell()
	{
		Random rand = new Random();

		var randIndex = rand.Next(0, cells.Count);
		var cell = cells[randIndex];

		return (cell.Row, cell.Col, cell.State == CELL_STATE.FLAG);
	}

	private void BoardToList()
	{
		cells = Board.Cast<GameCell>().Where(v => v.State == CELL_STATE.CLOSE || v.State == CELL_STATE.FLAG).ToList();
	}
}
