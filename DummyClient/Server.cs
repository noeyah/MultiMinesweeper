using MessagePack;
using Packet;
using ServerCore;
using System.Net.Sockets;

namespace DummyClient;

internal class Server
{
	private Connector _connector = new Connector();
	private SessionManager _sessionManager;

	private PacketHandler _packetHandler = new PacketHandler();

	private List<int> _sessionIDs = new List<int>();

	public Server(int poolCount, int bufferSize)
	{
		_sessionManager = new SessionManager(bufferSize);
		_connector.ConnectedHandler = OnConnected;

		_packetHandler.Init(this);
		_packetHandler.RegistPacketHandler();
	}

	public async Task Connect(string ip, int port, int clientCount)
	{
		var tasks = new Task[clientCount];

		for (int i = 0; i < clientCount; i++)
		{
			tasks[i] = Task.Run(() =>
			{
				_connector.Connect(ip, port);
			});
		}

		await Task.WhenAll(tasks);
		Console.WriteLine($"클라 연결 {clientCount}개");
	}

	public void StartSendPacket(int sessionID)
	{
		_packetHandler.Handler((ushort)PACKET_ID.Connected, sessionID, null);
	}

	private void OnConnected(Socket socket)
	{
		Session session = _sessionManager.Create(socket);
		var recvArgs = new SocketAsyncEventArgs();
		recvArgs.UserToken = session;
		recvArgs.Completed += IO_Completed;
		session._recvArgs = recvArgs;

		session.ReceiveCallback += OnReceiveData;
		session.DisconnectedCallback += OnDisconnected;
		session.ClosedCallback += OnClosed;

		OnConnected(session.SessionID);

		Task.Run(session.Start);
	}

	private void OnClosed(Session session, bool disconnect)
	{
		_sessionManager.Close(session.SessionID);
		session._recvArgs.Dispose();
	}

	private void OnConnected(int sessionID)
	{
		Console.WriteLine($"OnConnected - {sessionID}");

		_sessionIDs.Add(sessionID);
		StartSendPacket(sessionID);
	}

	private void OnDisconnected(int sessionID)
	{
		Console.WriteLine($"OnDisconnected - {sessionID}");

		_sessionIDs.Remove(sessionID);
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

		_packetHandler.Handler(packetID, sessionID, dataSegment);
	}

	public void Send<T>(int sessionID, T packet) where T : IPacket
	{
		var data = MessagePackSerializer.Serialize(packet);
		var totalSize = data.Length + NetworkDefine.HEADER_SIZE;

		var buffer = new byte[totalSize];
		var span = buffer.AsSpan();

		BitConverter.TryWriteBytes(span.Slice(0, NetworkDefine.HEADER_DATA_SIZE), (ushort)totalSize);
		BitConverter.TryWriteBytes(span.Slice(NetworkDefine.HEADER_DATA_SIZE, NetworkDefine.HEADER_PACKET_ID_SIZE), (ushort)packet.PacketID);
		data.AsSpan().CopyTo(span.Slice(NetworkDefine.HEADER_SIZE));

		var session = _sessionManager.GetSession(sessionID);
		session.Send(buffer);
	}

	private void IO_Completed(object? sender, SocketAsyncEventArgs args)
	{
		switch (args.LastOperation)
		{
			case SocketAsyncOperation.Receive:
				ProcessReceive(args);
				break;
			default:
				throw new ArgumentException($"[ERROR][{nameof(IO_Completed)}] {args.LastOperation.ToString()}");
		}
	}

	private void ProcessReceive(SocketAsyncEventArgs args)
	{
		var session = args.UserToken as Session;
		if (session is null)
		{
			return;
		}

		session.OnRecvCompleted(args);
	}
}
