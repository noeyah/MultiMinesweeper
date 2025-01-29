using Packet;

namespace Server;

internal class User
{
	private int _sessionID;
	private string _name;
	private ROOM_LEVEL _level;

	public int SessionID => _sessionID;
	public string Name => _name;

	public ROOM_LEVEL RoomLevel => _level;

	public User(int sessionID, string name)
	{
		_sessionID = sessionID;
		_name = name;
	}

	public void JoinRoom(ROOM_LEVEL level)
	{
		_level = level;
	}

	public void LeaveRoom()
	{
		_level = ROOM_LEVEL.NONE;
	}

	public bool IsLobby()
	{
		return _level == ROOM_LEVEL.NONE;
	}
}
