using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Packet;
using ServerCore;

namespace Server;

internal class MainServer : NetworkService, IHostedService
{
	private readonly Listener _listener = new Listener();

	private readonly ServerSettings _serverSettings;
	private readonly RoomManager _roomManager;
	private readonly UserManager _userManager;
	private readonly SendWorker _sendWorker;
	private readonly BroadcastWorker _broadcastWorker;
	private readonly PacketHandler _packetHandler;
	private readonly PacketProcessor _packetProcessor;

	public MainServer(
		IOptions<ServerSettings> settings, 
		RoomManager roomManager,
		UserManager userManager,
		SendWorker sendWorker, 
		BroadcastWorker broadcastWorker,
		PacketHandler packetHandler,
		PacketProcessor packetProcessor) 
	{
		_serverSettings = settings.Value;
		_roomManager = roomManager;
		_userManager = userManager;
		_sendWorker = sendWorker;
		_broadcastWorker = broadcastWorker;
		_packetHandler = packetHandler;
		_packetProcessor = packetProcessor;

		base.Init(_serverSettings.PoolCount, _serverSettings.BufferSize);

		sendWorker.Init(GetSession);
		broadcastWorker.Init(GetSession);

		_listener.AcceptHandler = Connected;
	}

	public void Start()
	{
		_listener.Start(_serverSettings.IP, _serverSettings.Port, _serverSettings.BackLog);
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		Start();
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_roomManager.Clear();
		_packetProcessor.Stop();
		_broadcastWorker.Stop();

		ServerDown();	// 세션 다 끊음(콜백 다 무시)

		return Task.CompletedTask;
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
