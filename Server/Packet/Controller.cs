using MessagePack;
using Packet;
using ServerCore;
using System.Text.RegularExpressions;

namespace Server;

internal partial class PacketHandler
{
	private void OnLoginReq(PacketData packetData)
	{
        var req = GetPacket<LoginReq>(packetData);
		if ( req is null )
		{
			return;
		}

		var res = new LoginRes();

		if (Regex.IsMatch(req.Name, "^[0-9a-zA-Z가-힣]*$") == false)
		{
			res.ErrorCode = ERROR_CODE.Fail_Login_Name;
			SendPacket(packetData.SessionID, res);
			return;
		}

		if (!_userMgr.AddUser(packetData.SessionID, req.Name))
		{
			res.ErrorCode = ERROR_CODE.Fail_Add_User;
			SendPacket(packetData.SessionID, res);
			return;
		}

        Console.WriteLine($"참가 - {packetData.SessionID}");

        var myPlayer = _userMgr.GetPlayer(packetData.SessionID);
		var room = _gameMgr.GetGameRoom();

		res.ErrorCode = ERROR_CODE.OK;
		res.MyPlayerIndex = myPlayer.Index;
		res.GameInfo = room.GetGameInfo();
		res.Players = _userMgr.GetPlayers();

		SendPacket(packetData.SessionID, res);

		// 다른 유저한테 참가 노티
		var noti = new JoinPlayerNoti();
		noti.JoinPlayer = myPlayer;
		SendNoti(noti);
	}

	// cell 오픈(좌클릭)
	private void OnOpenReq(PacketData packetData)
	{
		var req = GetPacket<OpenReq>(packetData);
		if (req is null)
		{
			return;
		}

		var res = new OpenRes();
		var room = _gameMgr.GetGameRoom();

		if (room.InvalidOpen(req.row, req.col))
		{
			res.ErrorCode = ERROR_CODE.Fail_Invalid_Open;
			SendPacket(packetData.SessionID, res);
			return;
		}

		if ( room.GameOver )
		{
			res.ErrorCode = ERROR_CODE.Fail_Alreay_End;
			SendPacket(packetData.SessionID, res);
			return;
		}

        Console.WriteLine($"{packetData.SessionID} - ({req.row}, {req.col}) Open");

        var updateCells = new List<GameCell>();
		room.OpenCell(req.row, req.col, ref updateCells);

		res.ErrorCode = ERROR_CODE.OK;
		SendPacket(packetData.SessionID, res);

		if ( room.GameOver )
		{
			Console.WriteLine($"GameOver - win? {room.Win}");
			GameOverNoti(updateCells, room);
		}
		else
		{
			UpdateCellsNoti(updateCells, room);
		}
	}

	// 깃발(우클릭)
	// 플래그로는 승리 체크를 하지 않음
	private void OnSetFlagReq(PacketData packetData)
	{
		var req = GetPacket<SetFlagReq>(packetData);
		if (req is null)
		{
			return;
		}

		var res = new SetFlagRes();
		var room = _gameMgr.GetGameRoom();

		if (room.GameOver)
		{
			res.ErrorCode = ERROR_CODE.Fail_Alreay_End;
			SendPacket(packetData.SessionID, res);
			return;
		}

		if (!room.CheckIndex(req.row, req.col))
		{
			res.ErrorCode = ERROR_CODE.Fail_Cell_Index;
			SendPacket(packetData.SessionID, res);
			return;
		}
		
		Console.WriteLine($"{packetData.SessionID} - ({req.row}, {req.col}) Flag {req.flag}");

		var gameCell = room.FlagCell(req.row, req.col, req.flag);
        if (gameCell is null)
        {
			res.ErrorCode = ERROR_CODE.Fail_Flag;
			SendPacket(packetData.SessionID, res);
			return;
        }

		res.ErrorCode = ERROR_CODE.OK;
		SendPacket(packetData.SessionID, res);

		UpdateCellsNoti(new List<GameCell>() { gameCell }, room);
    }

	private void OnResetReq(PacketData packetData)
	{
		var req = GetPacket<ResetReq>(packetData);
		if (req is null)
		{
			return;
		}

		var res = new ResetRes();
		var room = _gameMgr.GetGameRoom();

		// 게임이 끝나야지 리셋 가능
		if ( !room.GameOver )
		{
			res.ErrorCode = ERROR_CODE.Fail_Reset_Still_Playing;
			SendPacket(packetData.SessionID, res);
			return;
		}

		Console.WriteLine($"Reset");
		room.Reset();

		res.ErrorCode = ERROR_CODE.OK;
		SendPacket(packetData.SessionID, res);

		var noti = new ResetNoti();
		noti.GameInfo = room.GetGameInfo();
		SendNoti(noti);
	}

	#region Noti
	private void UpdateCellsNoti(List<GameCell> cells, GameRoom room)
	{
		if ( cells.Count == 0 )
		{
			return; 
		}

		var packet = new UpdateCellNoti();
		packet.UpdateCells = cells;
		packet.RemainMineCount = room.GetRemainMineCount_Player();

		SendNoti(packet);
	}

	private void GameOverNoti(List<GameCell> cells, GameRoom room)
	{
		var packet = new GameOverNoti();
		packet.UpdateCells = cells;
		packet.RemainMineCount = room.GetRemainMineCount_Player();
		packet.MineCells = room.GetAllMineGameCell();
		packet.Win = room.Win;

		SendNoti(packet);
	}

	private void SendNoti<T>(T packet) where T : IPacket
	{
		var data = MessagePackSerializer.Serialize<T>(packet);
		var users = _userMgr.GetUsers();
		foreach (var user in users)
		{
			Send(user, (ushort)packet.PacketID, data);
		}
	}
	#endregion
}
