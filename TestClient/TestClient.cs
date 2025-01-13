using Packet;
using System.Text.RegularExpressions;

namespace TestClient
{
	public partial class TestClient : Form
	{
		const int BOARD_SIZE = 16;

		private Server _server = new Server();
		private Cell[,] _cells;
		private int _boardSize = 0;
		private int _totalMineCount = 0;
		private int _myPlayerIndex = 0;
		private Dictionary<int, PlayerItem> _dicPlayer = new Dictionary<int, PlayerItem>();

		public TestClient()
		{
			InitializeComponent();
			InitBoard(BOARD_SIZE);

			_server.Init(10, 4096);
			_server.SetPacketHandler(this);
		}

		private void InitBoard(int size)
		{
			_cells = new Cell[size, size];

			for (int row = 0; row < size; row++)
			{
				for (int col = 0; col < size; col++)
				{
					var btn = new Button();
					btn.Name = $"cell_{row}_{col}";
					btn.Margin = new Padding(1);

					tableLayoutPanel_board.Controls.Add(btn, col, row);

					var cell = new Cell(row, col, btn);
					cell.ClickLeft = ClickLeft;
					cell.ClickRight = ClickRight;
					_cells[row, col] = cell;
				}
			}
		}

		private void button_connect_Click(object sender, EventArgs e)
		{
			var ip = textBox_ip.Text;
			var strPort = textBox_port.Text;

			if (string.IsNullOrEmpty(ip)
				|| string.IsNullOrEmpty(strPort))
			{
				MessageBox.Show("ip, port È®ÀÎ ÇÊ¿ä");
				return;
			}

			if (!int.TryParse(strPort, out int port))
			{
				MessageBox.Show("port È®ÀÎ ÇÊ¿ä");
				return;
			}

			_server.Connect(ip, port);
		}

		private void button_login_Click(object sender, EventArgs e)
		{
			var name = textBox_name.Text;

			if (string.IsNullOrEmpty(name))
			{
				MessageBox.Show("ÀÌ¸§ ÀÔ·Â");
				return;
			}

			if (Regex.IsMatch(name, "^[0-9a-zA-Z°¡-ÆR]*$") == false)
			{
				MessageBox.Show("ÀÌ¸§Àº ÇÑ±Û, ¿µ¾î, ¼ýÀÚ¸¸ °¡´É");
				return;
			}

			var packet = new LoginReq();
			packet.Name = name;

			_server.Send(packet);
		}


		private void button_reset_Click(object sender, EventArgs e)
		{
			var packet = new ResetReq();

			_server.Send(packet);
		}

		public void SetMyPlayerIndex(int index)
		{
			_myPlayerIndex = index;
		}

		public void AddPlayer(Player player)
		{
			if (_dicPlayer.ContainsKey(player.Index))
			{
				return;
			}

			var itemIndex = listBox_players.Items.Add(player.Name);
			var item = new PlayerItem(player, itemIndex);
			_dicPlayer.Add(player.Index, item);
		}

		public void RemovePlayer(int playerIndex)
		{
			if (!_dicPlayer.TryGetValue(playerIndex, out var item))
			{
				return;
			}

			listBox_players.Items.RemoveAt(item.ItemIndex);
			_dicPlayer.Remove(playerIndex);
		}

		public void ShowMessageBox(string message)
		{
			MessageBox.Show(message);
		}

		public void ShowText(string msg, bool add = false)
		{
			if (add)
			{
				richTextBox_msg.Text += $"\n{msg}";
			}
			else
			{
				richTextBox_msg.Text = msg;
			}
		}

		public void UpdateRemainMineCount(int mineCount)
		{
			textBox_mines.Text = mineCount.ToString();
		}

		public void SetUI(Action action)
		{
			if (InvokeRequired)
			{
				Invoke(action);
				return;
			}

			action();
		}

		public void SetBoard(GameInfo gameInfo)
		{
			_totalMineCount = gameInfo.TotalMineCount;
			_boardSize = gameInfo.BoardSize;

			UpdateRemainMineCount(gameInfo.RemainMineCount);

			for (int row = 0; row < BOARD_SIZE; row++)
			{
				for (int col = 0; col < BOARD_SIZE; col++)
				{
					var cell = _cells[row, col];
					if (row >= _boardSize
						|| col >= _boardSize)
					{
						cell.Hide = true;
					}
					else
					{
						var gameCell = gameInfo.Board[row, col];
						cell.Hide = false;
						cell.State = gameCell.State;
						cell.AdjacentMineCount = gameCell.AdjacentMineCount;
					}
					cell.Show();
				}
			}

			if ( gameInfo.GameOver )
			{
				label_result.Text = gameInfo.Win ? "½Â¸®" : "ÆÐ¹è";
			}
			else
			{
				label_result.Text = "";
			}
		}

		public void UpdateCells(List<GameCell> updateCells, int remainMineCount)
		{
			foreach (GameCell gameCell in updateCells)
			{
				if (!CheckCell(gameCell.Row, gameCell.Col))
				{
					MessageBox.Show($"UpdateCells ({gameCell.Row}, {gameCell.Col})", "ERROR");
					return;
				}
				var cell = _cells[gameCell.Row, gameCell.Col];
				cell.Update(gameCell);
				cell.Show();
			}

			UpdateRemainMineCount(remainMineCount);
		}

		public void GameOver(List<GameCell> mineCells, bool isWin)
		{
			for (int row = 0; row < BOARD_SIZE; row++)
			{
				for (int col = 0; col < BOARD_SIZE; col++)
				{
					var cell = _cells[row, col];
					var isMine = mineCells.Exists(v => v.Row == row && v.Col == col);
					cell.SetMine(isMine);
					cell.Show(true);
				}
			}

			label_result.Text = isWin ? "½Â¸®" : "ÆÐ¹è";
		}

		public void ClickRight(int row, int col, CELL_STATE currentState)
		{
			if (currentState != CELL_STATE.CLOSE
				&& currentState != CELL_STATE.FLAG)
			{
				return;
			}

			var packet = new SetFlagReq();
			packet.row = row;
			packet.col = col;
			packet.flag = currentState == CELL_STATE.CLOSE;

			_server.Send(packet);
		}

		public void ClickLeft(int row, int col, CELL_STATE currentState)
		{
			if (currentState != CELL_STATE.CLOSE)
			{
				return;
			}

			var packet = new OpenReq();
			packet.row = row;
			packet.col = col;

			_server.Send(packet);
		}

		public bool CheckCell(int row, int col)
		{
			if (row < 0
				|| row >= _boardSize
				|| col < 0
				|| col >= _boardSize)
			{
				return false;
			}

			var cell = _cells[row, col];
			if (cell.Hide)
			{
				return false;
			}

			return true;
		}

	}
}
