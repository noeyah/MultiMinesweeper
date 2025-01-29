namespace Packet;

public enum ERROR_CODE
{
	OK,
	FAIL,

	FAIL_LOGIN_NAME,		// 잘못된 이름
	FAIL_ADD_USER,			// 유저 추가 실패

	FAIL_USER_NOT_EXISTS,	// 유저를 찾을 수 없음

	FAIL_ROOM_LEVEL,		// 잘못된 룸레벨
	FAIL_ALREADY_JOIN_ROOM,	// 이미 방에 참가중임
	FAIL_ALREADY_LOBBY,		// 이미 로비임

	FAIL_ALREADY_GAME_OVER,	// 이미 끝난 게임
	FAIL_CELL_INDEX,		// row, col 값 확인 필요
	FAIL_NOT_CLOSE_CELL,	// 닫힌 칸이 아님
	FAIL_INVALID_FLAG,		// flag할 수 없는 상태임
	FAIL_ALREADY_FLAG,		// 이미 flag 상태가 동일함

	FAIL_RESET_NOT_GAME_OVER,	// 게임 오버 상태가 아니라서 리셋 불가능

}