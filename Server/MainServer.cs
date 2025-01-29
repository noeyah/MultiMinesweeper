using Microsoft.Extensions.Options;
using Packet;
using ServerCore;

namespace Server;

internal class MainServer : NetworkService
{
	private readonly Listener _listener = new Listener();

	private readonly PacketProcessor _packetProcessor;
	private readonly ServerSettings serverSettings;

	public MainServer(IOptions<ServerSettings> settings, PacketProcessor packetProcessor, SendWorker sendWorker, BroadcastWorker broadcastWorker)
	{
		serverSettings = settings.Value;

		base.Init(serverSettings.PoolCount, serverSettings.BufferSize);

		_packetProcessor = packetProcessor;

		sendWorker.Init(GetSession);
		broadcastWorker.Init(GetSession);

		_listener.AcceptHandler = Connected;
	}

	public void Start()
	{
		_listener.Start(serverSettings.IP, serverSettings.Port, serverSettings.BackLog);
	}

	public void Stop()
	{
		_packetProcessor.Stop();
	}

	protected override void OnConnected(int sessionID)
	{
		PacketData packetData = new PacketData();
		packetData.SessionID = sessionID;
		packetData.PacketID = (ushort)PACKET_ID.Connected;
		_packetProcessor.Receive(packetData);
	}

	protected override void OnDisconnected(int sessionID)
	{
		PacketData packetData = new PacketData();
		packetData.SessionID = sessionID;
		packetData.PacketID = (ushort)PACKET_ID.Disconnected;
		_packetProcessor.Receive(packetData);
	}

	protected override void OnReceiveData(int sessionID, ArraySegment<byte> data)
	{
		if (data.Array is null)
		{
			Console.WriteLine("OnReceiveData - data.Array is null");
			return;
		}
		var dataSize = BitConverter.ToUInt16(data.Array, data.Offset);
		var packetID = BitConverter.ToUInt16(data.Array, data.Offset + NetworkDefine.HEADER_DATA_SIZE);

		var dataSegment = new ArraySegment<byte>(data.Array, data.Offset + NetworkDefine.HEADER_SIZE, data.Count - NetworkDefine.HEADER_SIZE);

		PacketData packetData = new PacketData();
		packetData.SessionID = sessionID;
		packetData.PacketID = packetID;
		packetData.Size = dataSize;
		packetData.Body = dataSegment;

		_packetProcessor.Receive(packetData);
	}

	protected override void OnSendCompleted(int sessionID, byte[]? buffer, IList<ArraySegment<byte>>? bufferList)
	{
		if ( buffer != null )
		{
			BufferPool.Return(new ArraySegment<byte>(buffer));
		}

		if (bufferList != null)
		{
			foreach (var segment in bufferList)
			{
				BufferPool.Return(segment);
			}
		}
	}

}
