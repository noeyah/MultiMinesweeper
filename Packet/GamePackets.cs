using MessagePack;

namespace Packet;

[MessagePackObject]
public class OpenCellReq : IPacket
{
	[Key(0)]
	public int row;
	[Key(1)]
	public int col;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.OpenCellReq;
}

[MessagePackObject]
public class OpenCellRes : IPacket
{
	[Key(0)]
	public ERROR_CODE ErrorCode;
	[Key(1)]
	public List<GameCell> OpenCells;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.OpenCellRes;
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
	[Key(1)]
	public GameCell UpdateCell;
	[Key(2)]
	public int RemainMineCount;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.SetFlagRes;
}

[MessagePackObject]
public class UpdateCellNot : IPacket
{
	[Key(0)]
	public List<GameCell> UpdateCells;
	[Key(1)]
	public int RemainMineCount;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.UpdateCellNot;
}

[MessagePackObject]
public class GameOverNot : IPacket
{
	[Key(0)]
	public List<GameCell> MineCells;
	[Key(1)]
	public bool Win;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.GameOverNot;
}

[MessagePackObject]
public class GameResetReq : IPacket
{
	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.GameResetReq;
}

[MessagePackObject]
public class GameResetRes : IPacket
{
	[Key(0)]
	public ERROR_CODE ErrorCode;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.GameResetRes;
}

[MessagePackObject]
public class GameResetNot : IPacket
{
	[Key(0)]
	public RoomInfo RoomInfo;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.GameResetNot;
}
