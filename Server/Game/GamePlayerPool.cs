
namespace Server;

internal class GamePlayerPool
{
	private readonly Stack<GamePlayer> _pool = new Stack<GamePlayer>();

	public GamePlayer Get(int sessionID, string name)
	{
		if ( _pool.Count == 0 )
		{
			return new GamePlayer(sessionID, name);
		}

		var player = _pool.Pop();
		player.Set(sessionID, name);
		return player;
	}

	public void Return(GamePlayer player)
	{
		player.Reset();
		_pool.Push(player);
	}
}
