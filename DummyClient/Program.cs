using Packet;

namespace DummyClient;

internal class Program
{
	static async Task Main(string[] args)
	{
		var ip = "127.0.0.1";
		var port = 7777;
		var clientCount = 100;

		Server server = new Server();
		server.Init(200, 4096);

		while (true)
		{
			Thread.Sleep(1000);

			if (Console.KeyAvailable)
			{
				var key = Console.ReadKey(true);
				if (key.KeyChar == 'q')
				{
					await server.Connect(ip, port, clientCount);
				}
			}
		}
	}
}
