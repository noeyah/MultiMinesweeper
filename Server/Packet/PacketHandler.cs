using MessagePack;
using Packet;
using ServerCore;

namespace Server;

internal partial class PacketHandler
{
	private readonly RoomManager _roomMgr;
	private readonly UserManager _userMgr;
	private readonly SendWorker _sendWorker;
	private readonly BroadcastWorker _broadcastWorker;

	public PacketHandler(RoomManager roomMgr, UserManager userMgr, SendWorker sendWorker, BroadcastWorker broadcastWorker)
	{
		_roomMgr = roomMgr;
		_userMgr = userMgr;
		_sendWorker = sendWorker;
		_broadcastWorker = broadcastWorker;
	}

	public void RegistPacketHandler(Dictionary<ushort, Action<PacketData>> dicPacketHandler)
	{
		dicPacketHandler.Add((ushort)PACKET_ID.LoginReq, OnLoginReq);
		dicPacketHandler.Add((ushort)PACKET_ID.JoinRoomReq, OnJoinRoomReq);
		dicPacketHandler.Add((ushort)PACKET_ID.LeaveRoomReq, OnLeaveRoomReq);

		dicPacketHandler.Add((ushort)PACKET_ID.OpenCellReq, OnOpenCellReq);
		dicPacketHandler.Add((ushort)PACKET_ID.SetFlagReq, OnSetFlagReq);
		dicPacketHandler.Add((ushort)PACKET_ID.GameResetReq, OnGameResetReq);
	}

	#region common handler
	public void RegistCommon(Dictionary<ushort, Action<PacketData>> dicPacketHandler)
	{
		dicPacketHandler.Add((ushort)PACKET_ID.Connected, OnConnected);
		dicPacketHandler.Add((ushort)PACKET_ID.Disconnected, OnDisconnected);
	}

	private void OnConnected(PacketData packetData)
	{
		Console.WriteLine($"연결 - {packetData.SessionID}");
	}

	private void OnDisconnected(PacketData packetData)
	{
		Console.WriteLine($"끊김 - {packetData.SessionID}");

		var user = _userMgr.GetUser(packetData.SessionID);
		if (user is null)
		{
			return;
		}

		_userMgr.RemoveUser(packetData.SessionID);

		if (user.IsLobby())
		{
			return;
		}

		var room = _roomMgr.GetGameRoom(user.RoomLevel);
		if (room is null)
		{
			return;
		}

		room.LeavePlayer(packetData.SessionID);
		user.LeaveRoom();
		_broadcastWorker.RemovePacket(packetData.SessionID);

		// 방내 유저들한테 나감 알림
		var not = new LeaveRoomNot();
		not.LeaveUID = packetData.SessionID;
		Broadcast(not, room, packetData.SessionID);
		user = null;
	}
	#endregion

	#region packet 관련
	private T GetPacket<T>(PacketData packetData) where T : IPacket, new()
	{
		T packet = MessagePackSerializer.Deserialize<T>(packetData.Body);
		return packet;
	}

	// 바로 보냄
	private void SendPacket<T>(int sessionID, T packet) where T : IPacket
	{
		var data = MessagePackSerializer.Serialize(packet);
		var buffer = MakeSendData((ushort)packet.PacketID, data);

		_sendWorker.Send(sessionID, buffer);
	}

	// 바로 보내지 않고 약간 딜레이 있음
	private void Broadcast<T>(T packet, GameRoom room, int excludeSessionID = 0) where T : IPacket
	{
		var data = MessagePackSerializer.Serialize(packet);
		var sessionIDs = room.GetRoomSessionIDs();

		foreach (var sessionID in sessionIDs)
		{
			if (sessionID == excludeSessionID)
			{
				continue;
			}
			var buffer = MakeSendData((ushort)packet.PacketID, data);
			_broadcastWorker.AddPacket(sessionID, buffer);
		}
	}

	private byte[] MakeSendData(ushort packetID, byte[] data)
	{
		var totalSize = data.Length + NetworkDefine.HEADER_SIZE;

		var buffer = new byte[totalSize];
		var span = buffer.AsSpan();

		BitConverter.TryWriteBytes(span.Slice(0, NetworkDefine.HEADER_DATA_SIZE), (ushort)totalSize);
		BitConverter.TryWriteBytes(span.Slice(NetworkDefine.HEADER_DATA_SIZE, NetworkDefine.HEADER_PACKET_ID_SIZE), packetID);
		data.AsSpan().CopyTo(span.Slice(NetworkDefine.HEADER_SIZE));

		return buffer;
	}
	#endregion

}
