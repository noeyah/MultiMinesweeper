using MessagePack;
using Packet;

namespace TestClient;

internal class PacketHandler
{
	private Dictionary<ushort, Action<ArraySegment<byte>>> _dicPacketHandler = new Dictionary<ushort, Action<ArraySegment<byte>>>();
	private TestClient _client;

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

	public void SetClient(TestClient client)
	{
		_client = client;
	}

	public void EventHandler(ushort packetID, ArraySegment<byte> data)
	{
		if (TestClient.OptionIgnorePlayerNot)
		{
			if (packetID == (ushort)PACKET_ID.JoinRoomNot
				|| packetID == (ushort)PACKET_ID.LeaveRoomNot)
			{
				return;
			}
		}

		_client.SetUI(() =>
		{
			_dicPacketHandler[packetID](data);
		});
	}

	private void OnConnected(ArraySegment<byte> empty)
	{
		_client.ShowText("연결 성공");
	}

	private void OnLoginRes(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<LoginRes>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("LoginRes is null");
			return;
		}

		if ( packet.ErrorCode != ERROR_CODE.OK )
		{
			_client.ShowMessageBox($"LoginRes - {packet.ErrorCode}");
			return;
		}

		_client.ShowText("로그인 성공");

		_client.SetRoomList(packet.Levels);

		_client.ShowText($"방 개수 : {packet.Levels.Count}", true);
	}

	private void OnJoinRoomRes(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<JoinRoomRes>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("JoinRoomRes is null");
			return;
		}

		if (packet.ErrorCode != ERROR_CODE.OK)
		{
			_client.ShowMessageBox($"JoinRoomRes - {packet.ErrorCode}");
			return;
		}

		_client.ShowText("방 참가 성공");
		_client.ShowText($"방 참가자 : {packet.Players.Count}명", true);

		foreach (var player in packet.Players)
		{
			_client.AddPlayer(player);
		}

		_client.ShowText("보드 세팅", true);

		_client.SetBoard(packet.RoomInfo);

		_client.ShowText("게임 ㄱㄱ", true);
	}

	private void OnLeaveRoomRes(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<LeaveRoomRes>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("LeaveRoomRes is null");
			return;
		}

		if (packet.ErrorCode != ERROR_CODE.OK)
		{
			_client.ShowMessageBox($"LeaveRoomRes - {packet.ErrorCode}");
			return;
		}

		_client.ShowText("방 나감");

		_client.SetRoomList(packet.Levels);
		_client.RemoveAllPlayer();

		_client.ShowText($"방 개수 : {packet.Levels.Count}", true);
	}

	private void OnOpenRes(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<OpenCellRes>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("OpenRes is null");
			return;
		}

		if (packet.ErrorCode != ERROR_CODE.OK)
		{
			_client.ShowMessageBox($"OpenRes - {packet.ErrorCode}");
			return;
		}

		_client.UpdateCells(packet.OpenCells, packet.RemainMineCount);

		_client.ShowText("오픈");
	}

	private void OnSetFlagRes(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<SetFlagRes>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("SetFlagRes is null");
			return;
		}

		if (packet.ErrorCode != ERROR_CODE.OK)
		{
			_client.ShowMessageBox($"SetFlagRes - {packet.ErrorCode}");
			return;
		}

		_client.UpdateCells(new List<GameCell>() { packet.UpdateCell }, packet.RemainMineCount);

		_client.ShowText("플래그");
	}

	private void OnResetRes(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<GameResetRes>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("ResetRes is null");
			return;
		}

		if (packet.ErrorCode != ERROR_CODE.OK)
		{
			_client.ShowMessageBox($"ResetRes - {packet.ErrorCode}");
			return;
		}
		_client.ShowText("리셋");
	}

	private void OnUpdateCellNot(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<UpdateCellNot>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("UpdateCellNot is null");
			return;
		}
		_client.UpdateCells(packet.UpdateCells, packet.RemainMineCount);
	}

	private void OnGameOverNot(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<GameOverNot>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("GameOverNot is null");
			return;
		}
		_client.ShowText("게임 오버");

		_client.GameOver(packet.MineCells, packet.Win);
	}

	private void OnResetNot(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<GameResetNot>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("ResetNot is null");
			return;
		}
		_client.ShowText("게임 리셋");

		_client.SetBoard(packet.RoomInfo);
	}

	private void OnJoinPlayerNot(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<JoinRoomNot>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("JoinPlayerNot is null");
			return;
		}

		_client.AddPlayer(packet.JoinPlayer);
	}

	private void OnLeavePlayerNot(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<LeaveRoomNot>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("LeavePlayerNot is null");
			return;
		}

		_client.RemovePlayer(packet.LeaveUID);
	}

}
