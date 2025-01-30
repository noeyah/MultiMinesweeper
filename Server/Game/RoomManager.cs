using Packet;

namespace Server;

internal class RoomManager
{
	private readonly Dictionary<ROOM_LEVEL, GameRoom> _rooms = new Dictionary<ROOM_LEVEL, GameRoom>();

	public RoomManager()
	{
		foreach (ROOM_LEVEL level in Enum.GetValues(typeof(ROOM_LEVEL)))
		{
			if (level == ROOM_LEVEL.NONE)
			{
				continue;
			}

			GameRoom room = new GameRoom(level);
			_rooms[level] = room;
		}
	}

	public GameRoom GetGameRoom(ROOM_LEVEL level)
	{
		if (_rooms.TryGetValue(level, out var room))
		{
			return room;
		}

		return null;
	}

	public List<ROOM_LEVEL> GetRooms()
	{
		return _rooms.Keys.ToList();
	}

	public void Clear()
	{
		foreach (var room in _rooms.Values)
		{
			room.Close();
		}
		_rooms.Clear();
	}
}
