using MessagePack;

namespace Packet;


[MessagePackObject]
public class LoginReq : IPacket
{
	[Key(0)]
	public string Name;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.LoginReq;
}

[MessagePackObject]
public class LoginRes : IPacket
{
	[Key(0)]
	public ERROR_CODE ErrorCode;
	[Key(1)]
	public List<ROOM_LEVEL> Levels;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.LoginRes;
}

[MessagePackObject]
public class JoinRoomrReq : IPacket
{
	[Key(0)]
	public ROOM_LEVEL RoomLevel;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.JoinRoomReq;
}

[MessagePackObject]
public class JoinRoomRes : IPacket
{
	[Key(0)]
	public ERROR_CODE ErrorCode;
	[Key(1)]
	public RoomInfo RoomInfo;
	[Key(2)]
	public List<Player> Players;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.JoinRoomRes;
}

[MessagePackObject]
public class JoinRoomNot : IPacket
{
	[Key(0)]
	public Player JoinPlayer;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.JoinRoomNot;
}

[MessagePackObject]
public class LeaveRoomReq : IPacket
{
	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.LeaveRoomReq;
}

[MessagePackObject]
public class LeaveRoomRes : IPacket
{
	[Key(0)]
	public ERROR_CODE ErrorCode;
	[Key(1)]
	public List<ROOM_LEVEL> Levels;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.LeaveRoomRes;
}

[MessagePackObject]
public class LeaveRoomNot : IPacket
{
	[Key(0)]
	public int LeaveUID;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.LeaveRoomNot;
}