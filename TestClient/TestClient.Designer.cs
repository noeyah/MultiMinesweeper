
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
			panel_server = new Panel();
			button_connect = new Button();
			textBox_port = new TextBox();
			label_port = new Label();
			label_ip = new Label();
			textBox_ip = new TextBox();
			panel_login = new Panel();
			label2 = new Label();
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
			panel_server.SuspendLayout();
			panel_login.SuspendLayout();
			SuspendLayout();
			// 
			// panel_server
			// 
			panel_server.Controls.Add(button_connect);
			panel_server.Controls.Add(textBox_port);
			panel_server.Controls.Add(label_port);
			panel_server.Controls.Add(label_ip);
			panel_server.Controls.Add(textBox_ip);
			panel_server.Location = new Point(12, 12);
			panel_server.Name = "panel_server";
			panel_server.Size = new Size(409, 47);
			panel_server.TabIndex = 0;
			// 
			// button_connect
			// 
			button_connect.Location = new Point(325, 12);
			button_connect.Name = "button_connect";
			button_connect.Size = new Size(75, 23);
			button_connect.TabIndex = 4;
			button_connect.Text = "연결";
			button_connect.UseVisualStyleBackColor = true;
			button_connect.Click += button_connect_Click;
			// 
			// textBox_port
			// 
			textBox_port.Location = new Point(233, 12);
			textBox_port.Name = "textBox_port";
			textBox_port.Size = new Size(73, 23);
			textBox_port.TabIndex = 3;
			textBox_port.Text = "7777";
			// 
			// label_port
			// 
			label_port.AutoSize = true;
			label_port.Location = new Point(198, 15);
			label_port.Name = "label_port";
			label_port.Size = new Size(29, 15);
			label_port.TabIndex = 2;
			label_port.Text = "Port";
			// 
			// label_ip
			// 
			label_ip.AutoSize = true;
			label_ip.Location = new Point(14, 15);
			label_ip.Name = "label_ip";
			label_ip.Size = new Size(17, 15);
			label_ip.TabIndex = 1;
			label_ip.Text = "IP";
			// 
			// textBox_ip
			// 
			textBox_ip.Location = new Point(37, 12);
			textBox_ip.Name = "textBox_ip";
			textBox_ip.Size = new Size(152, 23);
			textBox_ip.TabIndex = 0;
			textBox_ip.Text = "127.0.0.1";
			// 
			// panel_login
			// 
			panel_login.Controls.Add(label2);
			panel_login.Controls.Add(button_login);
			panel_login.Controls.Add(label_name);
			panel_login.Controls.Add(textBox_name);
			panel_login.Location = new Point(12, 65);
			panel_login.Name = "panel_login";
			panel_login.Size = new Size(409, 47);
			panel_login.TabIndex = 1;
			// 
			// label2
			// 
			label2.BorderStyle = BorderStyle.Fixed3D;
			label2.Location = new Point(-1, 45);
			label2.Name = "label2";
			label2.Size = new Size(410, 2);
			label2.TabIndex = 9;
			// 
			// button_login
			// 
			button_login.Location = new Point(325, 11);
			button_login.Name = "button_login";
			button_login.Size = new Size(75, 23);
			button_login.TabIndex = 2;
			button_login.Text = "참가";
			button_login.UseVisualStyleBackColor = true;
			button_login.Click += button_login_Click;
			// 
			// label_name
			// 
			label_name.AutoSize = true;
			label_name.Location = new Point(14, 14);
			label_name.Name = "label_name";
			label_name.Size = new Size(165, 15);
			label_name.TabIndex = 1;
			label_name.Text = "이름 (한글/영어/숫자만 가능)";
			// 
			// textBox_name
			// 
			textBox_name.Location = new Point(185, 11);
			textBox_name.Name = "textBox_name";
			textBox_name.Size = new Size(121, 23);
			textBox_name.TabIndex = 0;
			// 
			// listBox_players
			// 
			listBox_players.FormattingEnabled = true;
			listBox_players.ItemHeight = 15;
			listBox_players.Location = new Point(428, 193);
			listBox_players.Name = "listBox_players";
			listBox_players.Size = new Size(95, 379);
			listBox_players.TabIndex = 2;
			// 
			// textBox_mines
			// 
			textBox_mines.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
			textBox_mines.ForeColor = Color.Red;
			textBox_mines.Location = new Point(79, 128);
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
			label_mines.Location = new Point(14, 136);
			label_mines.Name = "label_mines";
			label_mines.Size = new Size(59, 15);
			label_mines.TabIndex = 4;
			label_mines.Text = "남은 지뢰";
			// 
			// button_reset
			// 
			button_reset.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
			button_reset.Location = new Point(137, 129);
			button_reset.Name = "button_reset";
			button_reset.Size = new Size(75, 27);
			button_reset.TabIndex = 5;
			button_reset.Text = "초기화";
			button_reset.UseVisualStyleBackColor = true;
			button_reset.Click += button_reset_Click;
			// 
			// label_players
			// 
			label_players.Location = new Point(428, 167);
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
			tableLayoutPanel_board.Location = new Point(12, 172);
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
			richTextBox_msg.Location = new Point(428, 12);
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
			label_result.Location = new Point(255, 129);
			label_result.Name = "label_result";
			label_result.Size = new Size(0, 25);
			label_result.TabIndex = 10;
			// 
			// TestClient
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(541, 584);
			Controls.Add(label_result);
			Controls.Add(richTextBox_msg);
			Controls.Add(label1);
			Controls.Add(tableLayoutPanel_board);
			Controls.Add(label_players);
			Controls.Add(button_reset);
			Controls.Add(label_mines);
			Controls.Add(textBox_mines);
			Controls.Add(listBox_players);
			Controls.Add(panel_login);
			Controls.Add(panel_server);
			Name = "TestClient";
			Text = "MultiMinesweeper";
			panel_server.ResumeLayout(false);
			panel_server.PerformLayout();
			panel_login.ResumeLayout(false);
			panel_login.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Panel panel_server;
		private Label label_ip;
		private TextBox textBox_ip;
		private Button button_connect;
		private TextBox textBox_port;
		private Label label_port;
		private Panel panel_login;
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
		private Label label2;
		private RichTextBox richTextBox_msg;
		private Label label_result;
	}
}
