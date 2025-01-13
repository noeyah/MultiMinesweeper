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
	public int MyPlayerIndex;
	[Key(2)]
	public GameInfo GameInfo;
	[Key(3)]
	public List<Player> Players;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.LoginRes;
}


[MessagePackObject]
public class OpenReq : IPacket
{
	[Key(0)]
	public int row;
	[Key(1)]
	public int col;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.OpenReq;
}

[MessagePackObject]
public class OpenRes : IPacket
{
	[Key(0)]
	public ERROR_CODE ErrorCode;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.OpenRes;
}

[MessagePackObject]
public class SetFlagReq : IPacket
{
	[Key(0)]
	public int row;
	[Key(1)]
	public int col;
	[Key(2)]
	public bool flag;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.SetFlagReq;
}

[MessagePackObject]
public class SetFlagRes : IPacket
{
	[Key(0)]
	public ERROR_CODE ErrorCode;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.SetFlagRes;
}

[MessagePackObject]
public class UpdateCellNoti : IPacket
{
	[Key(0)]
	public List<GameCell> UpdateCells;
	[Key(1)]
	public int RemainMineCount;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.UpdateCellNoti;
}

[MessagePackObject]
public class GameOverNoti : IPacket
{
	[Key(0)]
	public List<GameCell> UpdateCells;
	[Key(1)]
	public int RemainMineCount;
	[Key(2)]
	public List<GameCell> MineCells;
	[Key(3)]
	public bool Win;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.GameOverNoti;
}

[MessagePackObject]
public class ResetReq : IPacket
{
	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.ResetReq;
}

[MessagePackObject]
public class ResetRes : IPacket
{
	[Key(0)]
	public ERROR_CODE ErrorCode;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.ResetRes;
}

[MessagePackObject]
public class ResetNoti : IPacket
{
	[Key(0)]
	public GameInfo GameInfo;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.ResetNoti;
}

[MessagePackObject]
public class JoinPlayerNoti : IPacket
{
	[Key(0)]
	public Player JoinPlayer;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.JoinPlayerNoti;
}

[MessagePackObject]
public class LeavePlayerNoti : IPacket
{
	[Key(0)]
	public int LeavePlayerIndex;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.LeavePlayerNoti;
}