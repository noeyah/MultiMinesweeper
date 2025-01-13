using Packet;

namespace Server;

internal class User
{
	private int _sessionID;
	private string _name;

	public int SessionID => _sessionID;
	public string Name => _name;

	public User(int sessionID)
	{
		_sessionID = sessionID;
	}

	public void SetName(string name)
	{
		_name = name;
	}

	public Player GetPlayer()
	{
		return new Player()
		{
			Index = _sessionID,
			Name = _name,
		};
	}
}
