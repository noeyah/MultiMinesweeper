using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server;

internal partial class PacketHandler
{
	// cell 오픈(좌클릭)
	private void OnOpenCellReq(PacketData packetData)
	{
		var req = GetPacket<OpenCellReq>(packetData);
		if (req is null)
		{
			return;
		}

		var res = new OpenCellRes();

		var user = _userMgr.GetUser(packetData.SessionID);
		if (user == null)
		{
			res.ErrorCode = ERROR_CODE.FAIL_USER_NOT_EXISTS;
			SendPacket(packetData.SessionID, res);
			return;
		}

		var room = _roomMgr.GetGameRoom(user.RoomLevel);
		if (room == null)
		{
			res.ErrorCode = ERROR_CODE.FAIL_ROOM_LEVEL;
			SendPacket(user.SessionID, res);
			return;
		}

		room.OpenCell(req.row, req.col, (errorCode, openCells) =>
		{
			res.ErrorCode = errorCode;
			if (res.ErrorCode != ERROR_CODE.OK)
			{
				SendPacket(user.SessionID, res);
				return;
			}

			Console.WriteLine($"{room.Level} ({req.row}, {req.col}) 오픈 - {user.Name}({user.SessionID})");

			res.OpenCells = openCells;
			SendPacket(packetData.SessionID, res);

			// 방내 다른 유저들한테 openCells 업뎃
			var not = new UpdateCellNot();
			not.UpdateCells = openCells;
			not.RemainMineCount = room.GetRoomRemainMineCount();
			Broadcast(not, room, user.SessionID);

			// 게임 오버면 방내 모든 유저한테 전송
			if (room.GameOver)
			{
				Console.WriteLine($"{room.Level} 게임오버 - {(room.Win ? "승리" : "패배")}");

				var gameOvetNot = new GameOverNot();
				gameOvetNot.Win = room.Win;
				gameOvetNot.MineCells = room.GetAllMineGameCell().ToList();
				Broadcast(gameOvetNot, room);
			}
		});
	}

	// 깃발(우클릭)
	private void OnSetFlagReq(PacketData packetData)
	{
		var req = GetPacket<SetFlagReq>(packetData);
		if (req is null)
		{
			return;
		}

		var res = new SetFlagRes();

		var user = _userMgr.GetUser(packetData.SessionID);
		if (user == null)
		{
			res.ErrorCode = ERROR_CODE.FAIL_USER_NOT_EXISTS;
			SendPacket(packetData.SessionID, res);
			return;
		}

		var room = _roomMgr.GetGameRoom(user.RoomLevel);
		if (room == null)
		{
			res.ErrorCode = ERROR_CODE.FAIL_ROOM_LEVEL;
			SendPacket(user.SessionID, res);
			return;
		}

		room.FlagCell(req.row, req.col, req.flag, (errorCode, flagCell) =>
		{
			res.ErrorCode = errorCode;
			if (res.ErrorCode != ERROR_CODE.OK)
			{
				SendPacket(user.SessionID, res);
				return;
			}

			if (flagCell is null)
			{
				Console.WriteLine($"flagCell is null");
				res.ErrorCode = ERROR_CODE.FAIL;
				SendPacket(user.SessionID, res);
				return;
			}

			Console.WriteLine($"{room.Level} ({req.row}, {req.col}) 플래그 {(req.flag ? "" : "해제 ")}- {user.Name}({user.SessionID})");

			var remainMineCount = room.GetRoomRemainMineCount();

			res.UpdateCell = flagCell;
			res.RemainMineCount = remainMineCount;
			SendPacket(packetData.SessionID, res);

			// 방내 다른 유저들한테 flagCell 업뎃
			var not = new UpdateCellNot();
			not.UpdateCells = new List<GameCell>() { flagCell };
			not.RemainMineCount = remainMineCount;
			Broadcast(not, room, user.SessionID);
		});
	}

	private void OnGameResetReq(PacketData packetData)
	{
		var req = GetPacket<GameResetReq>(packetData);
		if (req is null)
		{
			return;
		}

		var res = new GameResetRes();

		var user = _userMgr.GetUser(packetData.SessionID);
		if (user == null)
		{
			res.ErrorCode = ERROR_CODE.FAIL_USER_NOT_EXISTS;
			SendPacket(packetData.SessionID, res);
			return;
		}

		var room = _roomMgr.GetGameRoom(user.RoomLevel);
		if (room == null)
		{
			res.ErrorCode = ERROR_CODE.FAIL_ROOM_LEVEL;
			SendPacket(user.SessionID, res);
			return;
		}

		res.ErrorCode = room.Reset();

		if (res.ErrorCode != ERROR_CODE.OK)
		{
			SendPacket(user.SessionID, res);
			return;
		}

		Console.WriteLine($"Reset");
		SendPacket(user.SessionID, res);

		// 방내 모든 유저한테 전달
		var not = new GameResetNot();
		not.RoomInfo = room.GetRoomInfo();
		Broadcast(not, room);
	}

}
