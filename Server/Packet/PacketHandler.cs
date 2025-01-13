using MessagePack;
using Packet;

namespace Server;

internal partial class PacketHandler
{
	public delegate void SendHandler(int sessionID, ushort packetID, byte[] data);
	private readonly SendHandler Send;

	private readonly GameManager _gameMgr;
    private readonly UserManager _userMgr;


	public PacketHandler(GameManager gameMgr, UserManager userMgr, SendHandler sendHandler)
    {
        _gameMgr = gameMgr;
        _userMgr = userMgr;
		Send = sendHandler;
    }
	
    public void RegistPacketHandler(Dictionary<ushort, Action<PacketData>> dicPacketHandler)
	{
		dicPacketHandler.Add((ushort)PACKET_ID.LoginReq, OnLoginReq);
		dicPacketHandler.Add((ushort)PACKET_ID.OpenReq, OnOpenReq);
		dicPacketHandler.Add((ushort)PACKET_ID.SetFlagReq, OnSetFlagReq);
		dicPacketHandler.Add((ushort)PACKET_ID.ResetReq, OnResetReq);
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
        _userMgr.RemoveUser(packetData.SessionID);

		var packet = new LeavePlayerNoti();
		packet.LeavePlayerIndex = packetData.SessionID;
		SendNoti(packet);
	}
	#endregion

	#region packet serialize
	private T GetPacket<T>(PacketData packetData) where T : IPacket, new()
	{
		T packet = MessagePackSerializer.Deserialize<T>(packetData.Body);
		return packet;
	}

	private void SendPacket<T>(int sessionID, T packet) where T : IPacket
	{
		var data = MessagePackSerializer.Serialize(packet);
		Send(sessionID, (ushort)packet.PacketID, data);
	}
	#endregion

}
