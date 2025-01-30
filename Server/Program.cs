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

			services.AddHostedService<MainServer>();
		});

		var host = builder.Build();
		host.Run();
	}
}
