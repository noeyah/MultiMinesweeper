namespace Server;

internal class Program
{
	static void Main(string[] args)
	{
		Console.WriteLine("Multi Minesweeper Server");

		var mainServer = new MainServer();
		mainServer.Init(100, 4096);
		mainServer.Start("127.0.0.1", 7777, 5);

		while (true)
		{
			Thread.Sleep(1000);

			if (Console.KeyAvailable)
			{
				var key = Console.ReadKey(true);
				if (key.KeyChar == 'x')
				{
					Console.WriteLine("Stop Server");
					//

					break;
				}

			}
		}
	}
}
