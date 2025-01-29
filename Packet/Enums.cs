namespace Packet;

public enum PACKET_ID : ushort
{
	LoginReq,
	LoginRes,

	JoinRoomReq,
	JoinRoomRes,
	JoinRoomNot,

	LeaveRoomReq,
	LeaveRoomRes,
	LeaveRoomNot,

	OpenCellReq,
	OpenCellRes,

	SetFlagReq,
	SetFlagRes,

	GameResetReq,
	GameResetRes,
	GameResetNot,

	UpdateCellNot,
	GameOverNot,

	// 서버
	Connected = 10000,
	Disconnected,
}

public enum CELL_STATE
{
	CLOSE,
	OPEN,
	FLAG,
	MINE,
}

public enum ROOM_LEVEL
{
	NONE,
	EASY,
	NORMAL,
	HARD,
}