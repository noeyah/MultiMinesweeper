using Packet;
using System.Text.RegularExpressions;

namespace Server;

internal partial class PacketHandler
{
	// 로그인
	private void OnLoginReq(PacketData packetData)
	{
		var req = GetPacket<LoginReq>(packetData);
		if (req is null)
		{
			return;
		}

		var res = new LoginRes();

		if (Regex.IsMatch(req.Name, "^[0-9a-zA-Z가-힣]*$") == false)
		{
			res.ErrorCode = ERROR_CODE.FAIL_LOGIN_NAME;
			SendPacket(packetData.SessionID, res);
			return;
		}

		if (!_userMgr.AddUser(packetData.SessionID, req.Name))
		{
			res.ErrorCode = ERROR_CODE.FAIL_ADD_USER;
			SendPacket(packetData.SessionID, res);
			return;
		}

		Console.WriteLine($"로그인 : {req.Name}({packetData.SessionID})");

		res.ErrorCode = ERROR_CODE.OK;
		res.Levels = _roomMgr.GetRooms();
		SendPacket(packetData.SessionID, res);
	}

	// 방 입장
	private void OnJoinRoomReq(PacketData packetData)
	{
		var req = GetPacket<JoinRoomReq>(packetData);
		if (req is null)
		{
			return;
		}

		var res = new JoinRoomRes();

		// 방을 떠날거면 다른 패킷을 보내야함
		if (req.RoomLevel == ROOM_LEVEL.NONE)
		{
			res.ErrorCode = ERROR_CODE.FAIL_ROOM_LEVEL;
			SendPacket(packetData.SessionID, res);
			return;
		}

		var user = _userMgr.GetUser(packetData.SessionID);
		if (user == null)
		{
			res.ErrorCode = ERROR_CODE.FAIL_USER_NOT_EXISTS;
			SendPacket(packetData.SessionID, res);
			return;
		}

		var room = _roomMgr.GetGameRoom(req.RoomLevel);
		if (room == null)
		{
			res.ErrorCode = ERROR_CODE.FAIL_ROOM_LEVEL;
			SendPacket(user.SessionID, res);
			return;
		}

		// 이미 방에 참가중임
		if (!user.IsLobby())
		{
			res.ErrorCode = ERROR_CODE.FAIL_ALREADY_JOIN_ROOM;
			SendPacket(user.SessionID, res);
			return;
		}

		Console.WriteLine($"방 {req.RoomLevel} 참가 : {user.Name}({user.SessionID})");

		var joinPlayer = room.JoinPlayer(user.SessionID, user.Name);
		user.JoinRoom(req.RoomLevel);

		res.ErrorCode = ERROR_CODE.OK;
		res.RoomInfo = room.GetRoomInfo();
		res.Players = room.GetRoomPlayers();
		SendPacket(user.SessionID, res);

		// 방내 유저들한테 입장 알림
		var not = new JoinRoomNot();
		not.JoinPlayer = joinPlayer;
		Broadcast(not, room, user.SessionID);
	}

	// 방 나가기
	private void OnLeaveRoomReq(PacketData packetData)
	{
		var req = GetPacket<LeaveRoomReq>(packetData);
		if (req is null)
		{
			return;
		}

		var res = new LeaveRoomRes();

		var user = _userMgr.GetUser(packetData.SessionID);
		if (user == null)
		{
			res.ErrorCode = ERROR_CODE.FAIL_USER_NOT_EXISTS;
			SendPacket(packetData.SessionID, res);
			return;
		}

		if (user.IsLobby())
		{
			res.ErrorCode = ERROR_CODE.FAIL_ALREADY_LOBBY;
			SendPacket(user.SessionID, res);
			return;
		}

		var room = _roomMgr.GetGameRoom(user.RoomLevel);
		if (room == null)
		{
			res.ErrorCode = ERROR_CODE.FAIL_ROOM_LEVEL;
			SendPacket(user.SessionID, res);
			return;
		}

		Console.WriteLine($"방 {user.RoomLevel} 나감 : {user.Name}({user.SessionID})");

		room.LeavePlayer(user.SessionID);
		user.LeaveRoom();
		_broadcastWorker.RemovePacket(packetData.SessionID);

		res.ErrorCode = ERROR_CODE.OK;
		res.Levels = _roomMgr.GetRooms();
		SendPacket(user.SessionID, res);

		// 방내 유저들한테 나감 알림
		var not = new LeaveRoomNot();
		not.LeaveUID = user.SessionID;
		Broadcast(not, room, user.SessionID);
	}

}
