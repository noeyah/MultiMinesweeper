using Microsoft.Extensions.Options;
using Packet;
using ServerCore;

namespace Server;

internal class MainServer : NetworkService
{
	private readonly Listener _listener = new Listener();

	private readonly PacketProcessor _packetProcessor;
	//private SendWorker _sendWorker = new SendWorker();
	//private BroadcastWorker _broadcastWorker = new BroadcastWorker();

	//private RoomManager _roomManager = new RoomManager();
	//private UserManager _userManager = new UserManager();

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

	public void Init(int poolCount, int bufferSize, int taskCount)
	{
		base.Init(serverSettings.PoolCount, serverSettings.BufferSize);

		_listener.AcceptHandler = Connected;

		//_sendWorker.Init(GetSession);
		//_broadcastWorker.Init(GetSession);
		
		//_roomManager.Init();
		//_packetProcessor.Init(taskCount, _roomManager, _userManager, SendData);
	}

	//public void Start(string ip, int port, int backLog)
	//{
	//	_listener.Start(ip, port, backLog);
	//}

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

	//public void SendData(int sessionID, ushort packetID, byte[] data)
	//{
	//	var totalSize = data.Length + NetworkDefine.HEADER_SIZE;

	//	var buffer = BufferPool.Rent(totalSize);
	//	var span = buffer.AsSpan();

	//	BitConverter.TryWriteBytes(span.Slice(0, NetworkDefine.HEADER_DATA_SIZE), (ushort)totalSize);
	//	BitConverter.TryWriteBytes(span.Slice(NetworkDefine.HEADER_DATA_SIZE, NetworkDefine.HEADER_PACKET_ID_SIZE), packetID);
	//	data.AsSpan().CopyTo(span.Slice(NetworkDefine.HEADER_SIZE));

	//	_sendWorker.Send(sessionID, buffer);
	//}

}
