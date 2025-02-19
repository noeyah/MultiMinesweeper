using MessagePack;
using Packet;

namespace DummyClient;

internal class PacketHandler
{
	private Dictionary<ushort, Action<int, ArraySegment<byte>>> _dicPacketHandler = new Dictionary<ushort, Action<int, ArraySegment<byte>>>();

	private Server _server;

	private Dictionary<ROOM_LEVEL, GameRoomInfo> _gameInfo = new();
	private object _lock = new object();

	public void Init(Server server)
	{
		_server = server;
	}

	public void RegistPacketHandler()
	{
		_dicPacketHandler.Add((ushort)PACKET_ID.Connected, OnConnected);

		_dicPacketHandler.Add((ushort)PACKET_ID.LoginRes, OnLoginRes);
		_dicPacketHandler.Add((ushort)PACKET_ID.JoinRoomRes, OnJoinRoomRes);
		_dicPacketHandler.Add((ushort)PACKET_ID.LeaveRoomRes, OnLeaveRoomRes);

		_dicPacketHandler.Add((ushort)PACKET_ID.OpenCellRes, OnOpenRes);
		_dicPacketHandler.Add((ushort)PACKET_ID.SetFlagRes, OnSetFlagRes);
		_dicPacketHandler.Add((ushort)PACKET_ID.GameResetRes, OnResetRes);

		_dicPacketHandler.Add((ushort)PACKET_ID.GameResetNot, OnResetNot);
		_dicPacketHandler.Add((ushort)PACKET_ID.UpdateCellNot, OnUpdateCellNot);
		_dicPacketHandler.Add((ushort)PACKET_ID.GameOverNot, OnGameOverNot);
		_dicPacketHandler.Add((ushort)PACKET_ID.JoinRoomNot, OnJoinPlayerNot);
		_dicPacketHandler.Add((ushort)PACKET_ID.LeaveRoomNot, OnLeavePlayerNot);
	}

	public void Handler(ushort packetID, int sessionID, ArraySegment<byte> segment)
	{
		if (_dicPacketHandler.TryGetValue(packetID, out var handler))
		{
			handler(sessionID, segment);
		}
	}

	private void OnConnected(int sessionID, ArraySegment<byte> empty)
	{
		var packet = new LoginReq();
		packet.Name = $"Iam{sessionID}";
		_server.Send(sessionID, packet);
	}

	private void OnLoginRes(int sessionID, ArraySegment<byte> data)
	{
		var res = MessagePackSerializer.Deserialize<LoginRes>(data);
		if (res is null)
		{
			return;
		}

		if (res.ErrorCode != ERROR_CODE.OK)
		{
			Console.WriteLine($"Login Error : {sessionID} - {res.ErrorCode}");
			return;
		}

		Console.WriteLine($"로그인 성공 : {sessionID}");

		var level = GetRoomLevel(sessionID);

		var packet = new JoinRoomReq();
		packet.RoomLevel = level;
		_server.Send(sessionID, packet);
	}

	private void OnJoinRoomRes(int sessionID, ArraySegment<byte> data)
	{
		var res = MessagePackSerializer.Deserialize<JoinRoomRes>(data);
		if (res is null)
		{
			return;
		}

		if (res.ErrorCode != ERROR_CODE.OK)
		{
			Console.WriteLine($"JoinRoomRes Error : {sessionID} - {res.ErrorCode}");
			return;
		}
		Console.WriteLine($"방 참가 성공 : {sessionID}");

		var level = GetRoomLevel(sessionID);

		lock (_lock)
		{
			if (!_gameInfo.ContainsKey(level))
			{
				_gameInfo.Add(level, new GameRoomInfo(res.RoomInfo.BoardSize, res.RoomInfo.Board, res.RoomInfo.GameOver, res.RoomInfo.Win));
			}
		}

		_ = RandomGamePacket(sessionID);
	}


	private void OnLeaveRoomRes(int sessionID, ArraySegment<byte> data)
	{
		var res = MessagePackSerializer.Deserialize<LeaveRoomRes>(data);
		if (res is null)
		{
			return;
		}

		if (res.ErrorCode != ERROR_CODE.OK)
		{
			return;
		}
	}

	private void OnOpenRes(int sessionID, ArraySegment<byte> data)
	{
		var res = MessagePackSerializer.Deserialize<OpenCellRes>(data);
		if (res is null)
		{
			return;
		}

		if (res.ErrorCode != ERROR_CODE.OK)
		{
			Console.WriteLine($"오픈 실패 : {sessionID} - {res.ErrorCode}");
		}
		else
		{
			Console.WriteLine($"오픈 성공 {sessionID} - ({res.OpenCells.Count})");
			var gameInfo = GetGameInfo(sessionID);
			if (gameInfo == null)
			{
				return;
			}

			foreach (var cell in res.OpenCells)
			{
				gameInfo.SetCellState(cell.Row, cell.Col, cell.State);
			}
		}

		_ = RandomGamePacket(sessionID);
	}

	private void OnSetFlagRes(int sessionID, ArraySegment<byte> data)
	{
		var res = MessagePackSerializer.Deserialize<SetFlagRes>(data);
		if (res is null)
		{
			return;
		}

		if (res.ErrorCode != ERROR_CODE.OK)
		{
			Console.WriteLine($"플래그 실패 : {sessionID} - {res.ErrorCode}");
		}
		else
		{
			Console.WriteLine($"플래그 성공 {sessionID} - ({res.RemainMineCount})");

			var gameInfo = GetGameInfo(sessionID);
			if (gameInfo == null)
			{
				return;
			}
			gameInfo.SetCellState(res.UpdateCell.Row, res.UpdateCell.Col, res.UpdateCell.State);
		}
		
		_ = RandomGamePacket(sessionID);
	}

	private void OnResetRes(int sessionID, ArraySegment<byte> data)
	{
		var res = MessagePackSerializer.Deserialize<GameResetRes>(data);
		if (res is null)
		{
			return;
		}

		//if (res.ErrorCode != ERROR_CODE.OK)
		//{
		//	return;
		//}
		_ = RandomGamePacket(sessionID);
	}

	private void OnUpdateCellNot(int sessionID, ArraySegment<byte> data)
	{
		var noti = MessagePackSerializer.Deserialize<UpdateCellNot>(data);
		if (noti is null)
		{
			return;
		}
	}

	private void OnGameOverNot(int sessionID, ArraySegment<byte> data)
	{
		var noti = MessagePackSerializer.Deserialize<GameOverNot>(data);
		if (noti is null)
		{
			return;
		}

		var gameInfo = GetGameInfo(sessionID);
		if (gameInfo == null)
		{
			return;
		}

		if ( !gameInfo.GameOver)
		{
			gameInfo.GameOver = true;
			gameInfo.Win = noti.Win;
		}
	}

	private void OnResetNot(int sessionID, ArraySegment<byte> data)
	{
		var noti = MessagePackSerializer.Deserialize<GameResetNot>(data);
		if (noti is null)
		{
			return;
		}

		var gameInfo = GetGameInfo(sessionID);
		if (gameInfo == null)
		{
			return;
		}

		if (gameInfo.GameOver)
		{
			gameInfo.Reset(noti.RoomInfo.Board);
		}
	}

	private void OnJoinPlayerNot(int sessionID, ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<JoinRoomNot>(data);
		if (packet is null)
		{
			return;
		}
		var level = GetRoomLevel(sessionID);
		//Console.WriteLine($"{level} 방에 참가한 유저 : {packet.JoinPlayer.Name}({packet.JoinPlayer.UID})");
	}

	private void OnLeavePlayerNot(int sessionID, ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<LeaveRoomNot>(data);
		if (packet is null)
		{
			return;
		}
		var level = GetRoomLevel(sessionID);
		//Console.WriteLine($"{level} 방에 떠난 유저 : {packet.LeaveUID}");
	}

	private async Task RandomGamePacket(int sessionID)
	{
		var gameInfo = GetGameInfo(sessionID);
		if (gameInfo == null)
		{
			return;
		}

		Random random = new Random();
		await Task.Delay(random.Next(100, 3000));
		
		if ( gameInfo.GameOver )
		{
			var resetPacket = new GameResetReq();
			_server.Send(sessionID, resetPacket);
			return;
		}
		
		int randValue = random.Next(0, 3);

		if (randValue == 0)
		{
			var cell = gameInfo.GetRandomFlagCell();

			var flagPacket = new SetFlagReq();
			flagPacket.row = cell.row;
			flagPacket.col = cell.col;
			flagPacket.flag = !cell.currFlag;

			_server.Send(sessionID, flagPacket);
		}
		else
		{
			var cell = gameInfo.GetRandomCloseCell();

			var openPacket = new OpenCellReq();
			openPacket.row = cell.row;
			openPacket.col = cell.col;

			_server.Send(sessionID, openPacket);
		}
	}

	private ROOM_LEVEL GetRoomLevel(int sessionID)
	{
		return (sessionID & 1) == 0 ? ROOM_LEVEL.EASY : ROOM_LEVEL.NORMAL;
	}

	private GameRoomInfo GetGameInfo(int sessionID)
	{
		var level = GetRoomLevel(sessionID);
		if (_gameInfo.ContainsKey(level))
		{
			return _gameInfo[level];
		}
		return null;
	}

}
