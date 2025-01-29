
using Packet;

namespace TestClient
{
	partial class TestClient
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			button_connect = new Button();
			textBox_port = new TextBox();
			label_port = new Label();
			label_ip = new Label();
			textBox_ip = new TextBox();
			button_login = new Button();
			label_name = new Label();
			textBox_name = new TextBox();
			listBox_players = new ListBox();
			textBox_mines = new TextBox();
			label_mines = new Label();
			button_reset = new Button();
			label_players = new Label();
			tableLayoutPanel_board = new TableLayoutPanel();
			label1 = new Label();
			richTextBox_msg = new RichTextBox();
			label_result = new Label();
			comboBox_room = new ComboBox();
			groupBox_room = new GroupBox();
			button_leave = new Button();
			button_join = new Button();
			groupBox_connect = new GroupBox();
			groupBox_login = new GroupBox();
			checkBox_ignore_player_not = new CheckBox();
			groupBox_room.SuspendLayout();
			groupBox_connect.SuspendLayout();
			groupBox_login.SuspendLayout();
			SuspendLayout();
			// 
			// button_connect
			// 
			button_connect.Location = new Point(321, 19);
			button_connect.Name = "button_connect";
			button_connect.Size = new Size(75, 23);
			button_connect.TabIndex = 4;
			button_connect.Text = "연결";
			button_connect.UseVisualStyleBackColor = true;
			button_connect.Click += button_connect_Click;
			// 
			// textBox_port
			// 
			textBox_port.Location = new Point(229, 19);
			textBox_port.Name = "textBox_port";
			textBox_port.Size = new Size(73, 23);
			textBox_port.TabIndex = 3;
			textBox_port.Text = "7777";
			// 
			// label_port
			// 
			label_port.AutoSize = true;
			label_port.Location = new Point(194, 22);
			label_port.Name = "label_port";
			label_port.Size = new Size(29, 15);
			label_port.TabIndex = 2;
			label_port.Text = "Port";
			// 
			// label_ip
			// 
			label_ip.AutoSize = true;
			label_ip.Location = new Point(10, 22);
			label_ip.Name = "label_ip";
			label_ip.Size = new Size(17, 15);
			label_ip.TabIndex = 1;
			label_ip.Text = "IP";
			// 
			// textBox_ip
			// 
			textBox_ip.Location = new Point(33, 19);
			textBox_ip.Name = "textBox_ip";
			textBox_ip.Size = new Size(152, 23);
			textBox_ip.TabIndex = 0;
			textBox_ip.Text = "127.0.0.1";
			// 
			// button_login
			// 
			button_login.Location = new Point(320, 18);
			button_login.Name = "button_login";
			button_login.Size = new Size(75, 23);
			button_login.TabIndex = 2;
			button_login.Text = "로그인";
			button_login.UseVisualStyleBackColor = true;
			button_login.Click += button_login_Click;
			// 
			// label_name
			// 
			label_name.AutoSize = true;
			label_name.Location = new Point(9, 21);
			label_name.Name = "label_name";
			label_name.Size = new Size(165, 15);
			label_name.TabIndex = 1;
			label_name.Text = "이름 (한글/영어/숫자만 가능)";
			// 
			// textBox_name
			// 
			textBox_name.Location = new Point(180, 18);
			textBox_name.Name = "textBox_name";
			textBox_name.Size = new Size(121, 23);
			textBox_name.TabIndex = 0;
			// 
			// listBox_players
			// 
			listBox_players.FormattingEnabled = true;
			listBox_players.ItemHeight = 15;
			listBox_players.Location = new Point(432, 249);
			listBox_players.Name = "listBox_players";
			listBox_players.Size = new Size(95, 379);
			listBox_players.TabIndex = 2;
			// 
			// textBox_mines
			// 
			textBox_mines.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
			textBox_mines.ForeColor = Color.Red;
			textBox_mines.Location = new Point(83, 188);
			textBox_mines.Name = "textBox_mines";
			textBox_mines.ReadOnly = true;
			textBox_mines.Size = new Size(42, 27);
			textBox_mines.TabIndex = 3;
			textBox_mines.Text = "0";
			textBox_mines.TextAlign = HorizontalAlignment.Center;
			// 
			// label_mines
			// 
			label_mines.AutoSize = true;
			label_mines.Location = new Point(18, 196);
			label_mines.Name = "label_mines";
			label_mines.Size = new Size(59, 15);
			label_mines.TabIndex = 4;
			label_mines.Text = "남은 지뢰";
			// 
			// button_reset
			// 
			button_reset.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
			button_reset.Location = new Point(141, 189);
			button_reset.Name = "button_reset";
			button_reset.Size = new Size(75, 27);
			button_reset.TabIndex = 5;
			button_reset.Text = "초기화";
			button_reset.UseVisualStyleBackColor = true;
			button_reset.Click += button_reset_Click;
			// 
			// label_players
			// 
			label_players.Location = new Point(432, 223);
			label_players.Name = "label_players";
			label_players.Size = new Size(95, 23);
			label_players.TabIndex = 6;
			label_players.Text = "참가자";
			label_players.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// tableLayoutPanel_board
			// 
			tableLayoutPanel_board.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tableLayoutPanel_board.ColumnCount = 16;
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.Location = new Point(16, 228);
			tableLayoutPanel_board.Name = "tableLayoutPanel_board";
			tableLayoutPanel_board.RowCount = 16;
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.RowStyles.Add(new RowStyle(SizeType.Percent, 6.25F));
			tableLayoutPanel_board.Size = new Size(400, 400);
			tableLayoutPanel_board.TabIndex = 7;
			// 
			// label1
			// 
			label1.BorderStyle = BorderStyle.Fixed3D;
			label1.Location = new Point(12, 60);
			label1.Name = "label1";
			label1.Size = new Size(410, 2);
			label1.TabIndex = 8;
			// 
			// richTextBox_msg
			// 
			richTextBox_msg.Location = new Point(432, 18);
			richTextBox_msg.Name = "richTextBox_msg";
			richTextBox_msg.ReadOnly = true;
			richTextBox_msg.Size = new Size(95, 100);
			richTextBox_msg.TabIndex = 9;
			richTextBox_msg.Text = "";
			// 
			// label_result
			// 
			label_result.AutoSize = true;
			label_result.Font = new Font("맑은 고딕", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
			label_result.Location = new Point(259, 189);
			label_result.Name = "label_result";
			label_result.Size = new Size(50, 25);
			label_result.TabIndex = 10;
			label_result.Text = "승패";
			// 
			// comboBox_room
			// 
			comboBox_room.FormattingEnabled = true;
			comboBox_room.Location = new Point(9, 18);
			comboBox_room.Name = "comboBox_room";
			comboBox_room.Size = new Size(150, 23);
			comboBox_room.TabIndex = 11;
			// 
			// groupBox_room
			// 
			groupBox_room.Controls.Add(button_leave);
			groupBox_room.Controls.Add(button_join);
			groupBox_room.Controls.Add(comboBox_room);
			groupBox_room.Location = new Point(12, 124);
			groupBox_room.Name = "groupBox_room";
			groupBox_room.Size = new Size(408, 50);
			groupBox_room.TabIndex = 12;
			groupBox_room.TabStop = false;
			groupBox_room.Text = "3. 방 입장/나가기";
			// 
			// button_leave
			// 
			button_leave.Location = new Point(319, 17);
			button_leave.Name = "button_leave";
			button_leave.Size = new Size(75, 23);
			button_leave.TabIndex = 13;
			button_leave.Text = "나가기";
			button_leave.UseVisualStyleBackColor = true;
			button_leave.Click += button_leave_click;
			// 
			// button_join
			// 
			button_join.Location = new Point(179, 18);
			button_join.Name = "button_join";
			button_join.Size = new Size(75, 23);
			button_join.TabIndex = 12;
			button_join.Text = "입장";
			button_join.UseVisualStyleBackColor = true;
			button_join.Click += button_join_click;
			// 
			// groupBox_connect
			// 
			groupBox_connect.Controls.Add(button_connect);
			groupBox_connect.Controls.Add(textBox_port);
			groupBox_connect.Controls.Add(label_port);
			groupBox_connect.Controls.Add(label_ip);
			groupBox_connect.Controls.Add(textBox_ip);
			groupBox_connect.Location = new Point(11, 12);
			groupBox_connect.Name = "groupBox_connect";
			groupBox_connect.Size = new Size(408, 50);
			groupBox_connect.TabIndex = 5;
			groupBox_connect.TabStop = false;
			groupBox_connect.Text = "1. 서버 연결";
			// 
			// groupBox_login
			// 
			groupBox_login.Controls.Add(button_login);
			groupBox_login.Controls.Add(label_name);
			groupBox_login.Controls.Add(textBox_name);
			groupBox_login.Location = new Point(11, 68);
			groupBox_login.Name = "groupBox_login";
			groupBox_login.Size = new Size(408, 50);
			groupBox_login.TabIndex = 10;
			groupBox_login.TabStop = false;
			groupBox_login.Text = "2. 로그인";
			// 
			// checkBox_ignore_player_not
			// 
			checkBox_ignore_player_not.AutoSize = true;
			checkBox_ignore_player_not.Location = new Point(434, 134);
			checkBox_ignore_player_not.Name = "checkBox_ignore_player_not";
			checkBox_ignore_player_not.Size = new Size(90, 19);
			checkBox_ignore_player_not.TabIndex = 13;
			checkBox_ignore_player_not.Text = "참가자 끄기";
			checkBox_ignore_player_not.UseVisualStyleBackColor = true;
			checkBox_ignore_player_not.CheckedChanged += checkBox_ignore_player_not_Changed;
			// 
			// TestClient
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(541, 648);
			Controls.Add(checkBox_ignore_player_not);
			Controls.Add(groupBox_login);
			Controls.Add(groupBox_connect);
			Controls.Add(groupBox_room);
			Controls.Add(label_result);
			Controls.Add(richTextBox_msg);
			Controls.Add(label1);
			Controls.Add(tableLayoutPanel_board);
			Controls.Add(label_players);
			Controls.Add(button_reset);
			Controls.Add(label_mines);
			Controls.Add(textBox_mines);
			Controls.Add(listBox_players);
			Name = "TestClient";
			Text = "MultiMinesweeper";
			groupBox_room.ResumeLayout(false);
			groupBox_connect.ResumeLayout(false);
			groupBox_connect.PerformLayout();
			groupBox_login.ResumeLayout(false);
			groupBox_login.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private Label label_ip;
		private TextBox textBox_ip;
		private Button button_connect;
		private TextBox textBox_port;
		private Label label_port;
		private Label label_name;
		private TextBox textBox_name;
		private Button button_login;
		private ListBox listBox_players;
		private TextBox textBox_mines;
		private Label label_mines;
		private Button button_reset;
		private Label label_players;
		private TableLayoutPanel tableLayoutPanel_board;
		private Label label1;
		private RichTextBox richTextBox_msg;
		private Label label_result;
		private ComboBox comboBox_room;
		private GroupBox groupBox_room;
		private GroupBox groupBox_connect;
		private Button button_leave;
		private Button button_join;
		private GroupBox groupBox_login;
		private CheckBox checkBox_ignore_player_not;
	}
}
