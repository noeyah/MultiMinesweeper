using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Packet;
using ServerCore;

namespace Server;

internal class GameServer : IHostedService
{
	private readonly NetworkService _networkService;

	private readonly ServerSettings _serverSettings;
	private readonly RoomManager _roomManager;
	private readonly BroadcastWorker _broadcastWorker;
	private readonly PacketProcessor _packetProcessor;

	public GameServer(
		IOptions<ServerSettings> settings, 
		RoomManager roomManager,
		SendWorker sendWorker, 
		BroadcastWorker broadcastWorker,
		PacketProcessor packetProcessor) 
	{
		_serverSettings = settings.Value;

		var serverConfig = new ServerConfig()
		{
			IP = _serverSettings.IP,
			Port = _serverSettings.Port,
			BackLog = _serverSettings.BackLog,
			PoolCount = _serverSettings.PoolCount,
			BufferSize = _serverSettings.BufferSize,
			MaxConnectCount = 1000,
			AcceptCount = 2,
		};

		_networkService = new NetworkService(serverConfig);

		_roomManager = roomManager;
		_broadcastWorker = broadcastWorker;
		_packetProcessor = packetProcessor;

		sendWorker.Init(_networkService.GetSession);
		broadcastWorker.Init(_networkService.GetSession);

		_networkService.SessionReceiveDataCallback += OnReceiveData;
		_networkService.SessionConnectedCallback += OnConnected;
		_networkService.SessionDisconnectedCallback += OnDisconnected;
	}

	public void Start()
	{
		_networkService.Start();
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

		_networkService.ServerDown();	// 세션 다 끊음(콜백 다 무시)

		return Task.CompletedTask;
	}

	private void OnConnected(int sessionID)
	{
		PacketData packetData = new PacketData();
		packetData.SessionID = sessionID;
		packetData.PacketID = (ushort)PACKET_ID.Connected;
		_packetProcessor.Receive(packetData);
	}

	private void OnDisconnected(int sessionID)
	{
		PacketData packetData = new PacketData();
		packetData.SessionID = sessionID;
		packetData.PacketID = (ushort)PACKET_ID.Disconnected;
		_packetProcessor.Receive(packetData);
	}

	private void OnReceiveData(int sessionID, ArraySegment<byte> data)
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

}
