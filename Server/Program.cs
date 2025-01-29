using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Server;

internal class Program
{
	static void Main(string[] args)
	{
		Console.WriteLine("Multi Minesweeper Server");

		var builder = Host.CreateDefaultBuilder(args);
		
		builder.ConfigureServices((context, services) =>
		{
			var config = context.Configuration;
			services.Configure<ServerSettings>(config.GetSection("ServerSettings"));

			// DI 등록
			services.AddSingleton<RoomManager>();
			services.AddSingleton<UserManager>();
			services.AddSingleton<SendWorker>();
			services.AddSingleton<BroadcastWorker>();
			services.AddSingleton<PacketHandler>();
			services.AddSingleton<PacketProcessor>();
			services.AddSingleton<MainServer>();
		});

		var host = builder.Build();

		var mainServer = host.Services.GetRequiredService<MainServer>();
		mainServer.Start();

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
