using Packet;
using ServerCore;

namespace Server;

internal class MainServer : NetworkService
{
	private Listener _listener = new Listener();

	private PacketProcessor _packetProcessor = new PacketProcessor();
	private GameManager _gameManager = new GameManager();
	private UserManager _userManager = new UserManager();

	public override void Init(int poolCount, int bufferSize)
	{
		base.Init(poolCount, bufferSize);

		_listener.AcceptHandler = Connected;

		_gameManager.Init();
		_packetProcessor.Init(_gameManager, _userManager, SendData);
	}

	public void Start(string ip, int port, int backLog)
	{
		_listener.Start(ip, port, backLog);
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

	protected override void OnSendCompleted(int sessionID, int bytesTransferred, IList<ArraySegment<byte>> bufferList)
	{
		foreach (var segment in bufferList)
		{
			BufferPool.Return(segment);
		}
	}

	public void SendData(int sessionID, ushort packetID, byte[] data)
	{
		var session = GetSession(sessionID);
		if (session is null)
		{
			Console.WriteLine($"[ERROR]Invalid sessionID {sessionID}");
			return;
		}

		var totalSize = data.Length + NetworkDefine.HEADER_SIZE;

		var buffer = BufferPool.Rent(totalSize);
		var span = buffer.AsSpan();

		BitConverter.TryWriteBytes(span.Slice(0, NetworkDefine.HEADER_DATA_SIZE), (ushort)totalSize);
		BitConverter.TryWriteBytes(span.Slice(NetworkDefine.HEADER_DATA_SIZE, NetworkDefine.HEADER_PACKET_ID_SIZE), packetID);
		data.AsSpan().CopyTo(span.Slice(NetworkDefine.HEADER_SIZE));

		session.Send(buffer);
	}
}
