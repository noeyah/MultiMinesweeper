namespace Packet;

public enum PACKET_ID : ushort
{
	LoginReq,
	LoginRes,

	OpenReq,
	OpenRes,

	SetFlagReq,
	SetFlagRes,

	UpdateCellNoti,
	GameOverNoti,

	ResetReq,
	ResetRes,
	ResetNoti,

	JoinPlayerNoti,
	LeavePlayerNoti,

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