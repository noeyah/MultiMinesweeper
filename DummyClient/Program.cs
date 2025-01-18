using Packet;

namespace DummyClient;

internal class Program
{
	static void Main(string[] args)
	{
		var ip = "127.0.0.1";
		var port = 7777;

		Server server = new Server();
		server.Init(200, 4096);
		server.Connect(ip, port, 100);


		while (true)
		{
			Thread.Sleep(1000);

			if (Console.KeyAvailable)
			{
				var key = Console.ReadKey(true);
				if (key.KeyChar == 'x')
				{
					Console.WriteLine("Stop");
					break;
				}
			}
		}
	}
}
