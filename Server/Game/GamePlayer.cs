using Packet;

namespace Server;

internal class GamePlayer
{
	private int _sessionID = 0;
	private string _name = string.Empty;

	public int UID => _sessionID;

	public GamePlayer(int sessionID, string name)
	{
		Set(sessionID, name);
	}

	public void Set(int sessionID, string name)
	{
		_sessionID = sessionID;
		_name = name;
	}

	public void Reset()
	{
		_sessionID = 0;
		_name = string.Empty;
	}

	public Player GetPlayer()
	{
		return new Player
		{
			UID = _sessionID,
			Name = _name
		};
	}

}
