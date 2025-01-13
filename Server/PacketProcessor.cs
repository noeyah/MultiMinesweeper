using System.Threading.Channels;

namespace Server;

internal class PacketProcessor
{
	private PacketHandler _packetHandler;

	private Channel<PacketData> _channel = Channel.CreateUnbounded<PacketData>();
	private Task _processTask;

	private Dictionary<ushort, Action<PacketData>> _dicPacketHandler = new Dictionary<ushort, Action<PacketData>>();

	public void Init(GameManager gameManager, UserManager userManager, PacketHandler.SendHandler sendHandler)
	{
		_packetHandler = new PacketHandler(gameManager, userManager, sendHandler);
		
		_packetHandler.RegistCommon(_dicPacketHandler);
		_packetHandler.RegistPacketHandler(_dicPacketHandler);
		_processTask = ProcessAsync();
	}

	public void Receive(PacketData packetData)
	{
		_channel.Writer.TryWrite(packetData);
	}

	public void Stop()
	{
		_channel.Writer.Complete();
	}

	private async Task ProcessAsync()
	{
		while (await _channel.Reader.WaitToReadAsync())
		{
			while (_channel.Reader.TryRead(out var packetData))
			{
				try
				{
					if (_dicPacketHandler.TryGetValue(packetData.PacketID, out var handler))
					{
						handler(packetData);
					}
					else
					{
						Console.WriteLine($"알 수 없는 패킷이 들어옴 - sessionID : {packetData.SessionID}, packetID : {packetData.PacketID}, size : {packetData.Size}");
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
