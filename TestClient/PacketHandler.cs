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
		_dicPacketHandler.Add((ushort)PACKET_ID.OpenRes, OnOpenRes);
		_dicPacketHandler.Add((ushort)PACKET_ID.SetFlagRes, OnSetFlagRes);
		_dicPacketHandler.Add((ushort)PACKET_ID.ResetRes, OnResetRes);
		
		_dicPacketHandler.Add((ushort)PACKET_ID.ResetNoti, OnResetNoti);
		_dicPacketHandler.Add((ushort)PACKET_ID.UpdateCellNoti, OnUpdateCellNoti);
		_dicPacketHandler.Add((ushort)PACKET_ID.GameOverNoti, OnGameOverNoti);
		_dicPacketHandler.Add((ushort)PACKET_ID.JoinPlayerNoti, OnJoinPlayerNoti);
		_dicPacketHandler.Add((ushort)PACKET_ID.LeavePlayerNoti, OnLeavePlayerNoti);
	}

	public void SetClient(TestClient client)
	{
		_client = client;
	}

	public void EventHandler(ushort packetID, ArraySegment<byte> data)
	{
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

		_client.SetMyPlayerIndex(packet.MyPlayerIndex);

		_client.ShowText("플레이어 추가", true);

		foreach (var player in packet.Players)
		{
			_client.AddPlayer(player);
		}

		_client.ShowText("보드 세팅", true);

		_client.SetBoard(packet.GameInfo);

		_client.ShowText("완료", true);
	}

	private void OnOpenRes(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<OpenRes>(data);
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
		_client.ShowText("플래그");
	}

	private void OnResetRes(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<ResetRes>(data);
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

	private void OnUpdateCellNoti(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<UpdateCellNoti>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("UpdateCellNoti is null");
			return;
		}
		_client.UpdateCells(packet.UpdateCells, packet.RemainMineCount);
	}

	private void OnGameOverNoti(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<GameOverNoti>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("GameOverNoti is null");
			return;
		}
		_client.ShowText("게임 오버");

		_client.UpdateCells(packet.UpdateCells, packet.RemainMineCount);
		_client.GameOver(packet.MineCells, packet.Win);
	}

	private void OnResetNoti(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<ResetNoti>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("ResetNoti is null");
			return;
		}

		_client.SetBoard(packet.GameInfo);
	}

	private void OnJoinPlayerNoti(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<JoinPlayerNoti>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("JoinPlayerNoti is null");
			return;
		}

		_client.AddPlayer(packet.JoinPlayer);
	}

	private void OnLeavePlayerNoti(ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<LeavePlayerNoti>(data);
		if (packet is null)
		{
			_client.ShowMessageBox("LeavePlayerNoti is null");
			return;
		}

		_client.RemovePlayer(packet.LeavePlayerIndex);
	}

}
