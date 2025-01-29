using Packet;

namespace TestClient;


internal class Cell
{
	public delegate void ClickEvent(int row, int col, CELL_STATE currentState);
	public ClickEvent ClickLeft;
	public ClickEvent ClickRight;

	public int Row { get; private set; }
	public int Col { get; private set; }
	public Button Button { get; private set; }

	public CELL_STATE State;
	public int AdjacentMineCount;
	public int FlagPlayerIndex;

	public bool Hide;

	private bool _isMine;

	public Cell(int row, int col, Button button)
	{
		Row = row;
		Col = col;
		Button = button;
		button.MouseUp += ClickButton;
	}

	private void ClickButton(object sender, MouseEventArgs e)
	{
		if ( e.Button == MouseButtons.Left )
		{
			ClickLeft(Row, Col, State);
		}
		else if ( e.Button == MouseButtons.Right )
		{
			ClickRight(Row, Col, State);
		}
	}

	public void Reset()
	{
		_isMine = false;
		State = CELL_STATE.CLOSE;
		AdjacentMineCount = 0;
		FlagPlayerIndex = 0;
	}

	public void Update(GameCell cell)
	{
		if (Row != cell.Row && Col != cell.Col)
		{
			return;
		}
		State = cell.State;
		AdjacentMineCount = cell.AdjacentMineCount;
	}

	public void SetMine(bool isMine)
	{
		_isMine = isMine;
	}

	public void Show(bool result = false)
	{
		Button.Visible = !Hide;

		if (Hide)
		{
			return;
		}

		string text = string.Empty;
		FlatStyle style = FlatStyle.Standard;
		
		switch (State)
		{
			case CELL_STATE.CLOSE:
				{
					if ( result && _isMine )
					{
						text = "★";
					}
				}
				break;
			case CELL_STATE.OPEN:
				{
					if ( AdjacentMineCount > 0 )
					{
						text = $"{AdjacentMineCount}";
					}
					style = FlatStyle.Flat;
				}
				break;
			case CELL_STATE.FLAG:
				{
					if ( result && !_isMine )
					{
						text = "X";
					}
					else
					{
						text = "▶";
					}
				}
				break;
			case CELL_STATE.MINE:
				{
					text = "★";
				}
				break;
		}

		Button.Text = text;
		Button.FlatStyle = style;
	}

}
