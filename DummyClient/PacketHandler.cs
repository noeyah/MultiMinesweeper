using MessagePack;
using Packet;
using System;

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
		_dicPacketHandler.Add((ushort)PACKET_ID.OpenRes, OnOpenRes);
		_dicPacketHandler.Add((ushort)PACKET_ID.SetFlagRes, OnSetFlagRes);
		_dicPacketHandler.Add((ushort)PACKET_ID.ResetRes, OnResetRes);

		_dicPacketHandler.Add((ushort)PACKET_ID.ResetNoti, OnResetNoti);
		_dicPacketHandler.Add((ushort)PACKET_ID.UpdateCellNoti, OnUpdateCellNoti);
		_dicPacketHandler.Add((ushort)PACKET_ID.GameOverNoti, OnGameOverNoti);
		_dicPacketHandler.Add((ushort)PACKET_ID.JoinPlayerNoti, OnJoinPlayerNoti);
		_dicPacketHandler.Add((ushort)PACKET_ID.LeavePlayerNoti, OnLeavePlayerNoti);
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
			return;
		}

		Console.WriteLine($"Login {sessionID}");

		var packet = new OpenReq();
		packet.row = 0;
		packet.col = 0;
		_server.Send(sessionID, packet);
	}

	private void OnOpenRes(int sessionID, ArraySegment<byte> data)
	{
		var res = MessagePackSerializer.Deserialize<OpenRes>(data);
		if (res is null)
		{
			return;
		}

		if (res.ErrorCode != ERROR_CODE.OK)
		{
			return;
		}

		Console.WriteLine($"★ Open {sessionID}");
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
			return;
		}
	}

	private void OnResetRes(int sessionID, ArraySegment<byte> data)
	{
		var res = MessagePackSerializer.Deserialize<ResetRes>(data);
		if (res is null)
		{
			return;
		}

		if (res.ErrorCode != ERROR_CODE.OK)
		{
			return;
		}
	}

	private void OnUpdateCellNoti(int sessionID, ArraySegment<byte> data)
	{
		var noti = MessagePackSerializer.Deserialize<UpdateCellNoti>(data);
		if (noti is null)
		{
			return;
		}
	}

	private void OnGameOverNoti(int sessionID, ArraySegment<byte> data)
	{
		var noti = MessagePackSerializer.Deserialize<GameOverNoti>(data);
		if (noti is null)
		{
			return;
		}
	}

	private void OnResetNoti(int sessionID, ArraySegment<byte> data)
	{
		var noti = MessagePackSerializer.Deserialize<ResetNoti>(data);
		if (noti is null)
		{
			return;
		}

	}

	private void OnJoinPlayerNoti(int sessionID, ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<JoinPlayerNoti>(data);
		if (packet is null)
		{
			return;
		}

	}

	private void OnLeavePlayerNoti(int sessionID, ArraySegment<byte> data)
	{
		var packet = MessagePackSerializer.Deserialize<LeavePlayerNoti>(data);
		if (packet is null)
		{
			return;
		}

	}

}
