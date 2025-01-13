using Packet;

namespace Server;

internal class GameManager
{
	private GameRoom _gameRoom;

	// 9x9 10
	// 16x16 40

	public void Init()
	{
		_gameRoom = new GameRoom(9, 10);
		_gameRoom.Reset();
	}

	public GameRoom GetGameRoom(/*int roomNum*/)
	{
		return _gameRoom;
	}

}
