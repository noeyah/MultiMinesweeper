using MessagePack;
using Packet;

namespace DummyClient;

internal class PacketHandler
{
	private Dictionary<ushort, Action<int, ArraySegment<byte>>> _dicPacketHandler = new Dictionary<ushort, Action<int, ArraySegment<byte>>>();

	private Server _server;

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

		var level = sessionID % 2 == 0 ? ROOM_LEVEL.EASY : ROOM_LEVEL.NORMAL;
		var packet = new JoinRoomrReq();
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

		var level = sessionID % 2 == 0 ? ROOM_LEVEL.EASY : ROOM_LEVEL.NORMAL;
		Console.WriteLine($"{level} 방 인원 : {res.Players.Count}");
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
			return;
		}

		Console.WriteLine($"오픈 성공 {sessionID} - ({res.OpenCells.Count})");
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
			return;
		}

		Console.WriteLine($"플래그 성공 {sessionID} - ({res.RemainMineCount})");
	}

	private void OnResetRes(int sessionID, ArraySegment<byte> data)
	{
		var res = MessagePackSerializer.Deserialize<GameResetRes>(data);
		if (res is null)
		{
			return;
		}

		if (res.ErrorCode != ERROR_CODE.OK)
		{
			return;
		}
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
	}

	private void OnResetNot(int sessionID, ArraySegment<byte> data)
	{
		var noti = MessagePackSerializer.Deserialize<GameResetNot>(data);
		if (noti is null)
		{
			return;
		}

	}

	private void OnJoinPlayerNot(int sessionID, ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<JoinRoomNot>(data);
		if (packet is null)
		{
			return;
		}
		var level = sessionID % 2 == 0 ? ROOM_LEVEL.EASY : ROOM_LEVEL.NORMAL;
		//Console.WriteLine($"{level} 방에 참가한 유저 : {packet.JoinPlayer.Name}({packet.JoinPlayer.UID})");
	}

	private void OnLeavePlayerNot(int sessionID, ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<LeaveRoomNot>(data);
		if (packet is null)
		{
			return;
		}
		var level = sessionID % 2 == 0 ? ROOM_LEVEL.EASY : ROOM_LEVEL.NORMAL;
		//Console.WriteLine($"{level} 방에 떠난 유저 : {packet.LeaveUID}");
	}

}
