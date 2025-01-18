namespace Packet;

public enum ERROR_CODE
{
	OK,
	Fail,

	Fail_Login_Name,	// 이름 조건 잘못됨
	Fail_Add_User,		// 유저 추가 실패
	Fail_Cell_Index,	// row, col 값 확인 필요
	Fail_Invalid_Open,	// 오픈할 수 없음
	Fail_Alreay_End,	// 이미 끝난 게임
	Fail_Flag,			// 플래그 실패
	Fail_Reset_Still_Playing, // 게임 중이라서 리셋 불가

}