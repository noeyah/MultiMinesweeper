
namespace Packet;

public class Utils
{
	public static int GetBoardSize(ROOM_LEVEL level)
	{
		switch (level)
		{
			case ROOM_LEVEL.EASY:
				return Consts.EASY_SIZE;
			case ROOM_LEVEL.NORMAL:
				return Consts.NORMAL_SIZE;
			case ROOM_LEVEL.HARD:
				return Consts.HARD_SIZE;
			case ROOM_LEVEL.NONE:
			default:
				Console.WriteLine($"알 수 없는 룸레벨 {level}");
				return Consts.EASY_SIZE;
		}
	}

	public static int GetMineCount(ROOM_LEVEL level)
	{
		switch (level)
		{
			case ROOM_LEVEL.EASY:
				return Consts.EASY_MINECOUNT;
			case ROOM_LEVEL.NORMAL:
				return Consts.NORMAL_MINECOUNT;
			case ROOM_LEVEL.HARD:
				return Consts.HARD_MINECOUNT;
			case ROOM_LEVEL.NONE:
			default:
				return Consts.EASY_MINECOUNT;
		}
	}
}
