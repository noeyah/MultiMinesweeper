using Microsoft.Extensions.Options;
using Packet;
using System.Threading.Channels;

namespace Server;

internal class PacketProcessor
{
	private readonly Channel<PacketData> _channel = Channel.CreateUnbounded<PacketData>();
	private readonly Dictionary<ushort, Action<PacketData>> _dicPacketHandler = new();

	private readonly CancellationTokenSource _cts = new();

	public PacketProcessor(IOptions<ServerSettings> settings, PacketHandler packetHandler)
	{
		var settingsSettings = settings.Value;
		
		packetHandler.RegistCommon(_dicPacketHandler);
		packetHandler.RegistPacketHandler(_dicPacketHandler);

		for (int i = 0; i < settingsSettings.TaskCount; i++)
		{
			Task.Run(() => ProcessAsync(_channel, _cts.Token));
		}
	}

	public void Receive(PacketData packetData)
	{
		if (!_channel.Writer.TryWrite(packetData))
		{
            Console.WriteLine($"PacketProcessor Receive 실패 : {packetData.PacketID}");
        }
	}

	public void Stop()
	{
		_channel.Writer.Complete();
		_cts.Cancel();
	}

	private async Task ProcessAsync(Channel<PacketData> channel, CancellationToken ctsToken)
	{
		while (await channel.Reader.WaitToReadAsync(ctsToken))
		{
			while (channel.Reader.TryRead(out var packetData))
			{
				try
				{
                    if (_dicPacketHandler.TryGetValue(packetData.PacketID, out var handler))
					{
						handler(packetData);
					}
					else
					{
						Console.WriteLine($"알 수 없는 패킷이 들어옴 - sessionID : {packetData.SessionID}, packetID : {packetData.PacketID}");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"ProcessAsync - {ex.Message}");
				}
			}
		}
	}
}
