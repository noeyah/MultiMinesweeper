using Packet;
using System.Text.RegularExpressions;

namespace TestClient
{
	public partial class TestClient : Form
	{
		const int BOARD_SIZE = 16;

		private Server _server;
		private Cell[,] _cells;
		private int _boardSize = 0;
		private int _totalMineCount = 0;
		private int _myPlayerIndex = 0;
		private Dictionary<int, PlayerItem> _dicPlayer = new Dictionary<int, PlayerItem>();

		public TestClient()
		{
			InitializeComponent();
			InitBoard(BOARD_SIZE);

			_server = new Server(10, 4096);
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
				MessageBox.Show("ip, port 확인 필요");
				return;
			}

			if (!int.TryParse(strPort, out int port))
			{
				MessageBox.Show("port 확인 필요");
				return;
			}

			_server.Connect(ip, port);
		}

		private void button_login_Click(object sender, EventArgs e)
		{
			var name = textBox_name.Text;

			if (string.IsNullOrEmpty(name))
			{
				MessageBox.Show("이름 입력");
				return;
			}

			if (Regex.IsMatch(name, "^[0-9a-zA-Z가-힣]*$") == false)
			{
				MessageBox.Show("이름은 한글, 영어, 숫자만 가능");
				return;
			}

			var packet = new LoginReq();
			packet.Name = name;

			_server.Send(packet);
		}


		private void button_reset_Click(object sender, EventArgs e)
		{
			var packet = new GameResetReq();

			_server.Send(packet);
		}

		private void button_join_click(object sender, EventArgs e)
		{
			var selectedItem = comboBox_room.SelectedItem;

			if ( selectedItem is null )
			{
				MessageBox.Show("콤보박스 에러. 방 다시 선택해주세요");
				return;
			}

			if ( !Enum.TryParse<ROOM_LEVEL>(selectedItem.ToString(), out var selectedLevel))
			{
				MessageBox.Show("콤보박스 Enum.TryParse 실패");
				return;
			}

			if (selectedLevel == ROOM_LEVEL.NONE)
			{
				MessageBox.Show("방 선택 잘못됨");
				return;
			}

			if ( selectedLevel == ROOM_LEVEL.HARD)
			{
				ShowText("하드는 칸 부족하니까 윈폼에서는 들어가지 마세요..");
				return;
			}

			var packet = new JoinRoomReq();
			packet.RoomLevel = selectedLevel;
			_server.Send(packet);
		}

		private void button_leave_click(object sender, EventArgs e)
		{
			var packet = new LeaveRoomReq();
			_server.Send(packet);
		}

		public void SetRoomList(List<ROOM_LEVEL> roomList)
		{
			comboBox_room.Items.Clear();
			foreach (ROOM_LEVEL level in roomList)
			{
				comboBox_room.Items.Add(level.ToString());
			}
		}

		public void AddPlayer(Player player)
		{
			if (_dicPlayer.ContainsKey(player.UID))
			{
				return;
			}

			var itemIndex = listBox_players.Items.Add(player.Name);
			var item = new PlayerItem(player, itemIndex);
			_dicPlayer.Add(player.UID, item);
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

		public void RemoveAllPlayer()
		{
			listBox_players.Items.Clear();
			_dicPlayer.Clear();
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

		public void SetBoard(RoomInfo gameInfo)
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
				label_result.Text = gameInfo.Win ? "승리" : "패배";
			}
			else
			{
				label_result.Text = "";
			}
		}

		public void UpdateCells(List<GameCell> updateCells)
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

			label_result.Text = isWin ? "승리" : "패배";
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

			var packet = new OpenCellReq();
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

		public static bool OptionIgnorePlayerNot = false;
		public void checkBox_ignore_player_not_Changed(object sender, EventArgs e)
		{
			OptionIgnorePlayerNot = checkBox_ignore_player_not.Checked;
		}

	}
}
