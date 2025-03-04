using System.Net.Sockets;

namespace ServerCore;

public class NetworkService
{
	public event SessionReceiveData SessionReceiveDataCallback;
	public event SessionConnected SessionConnectedCallback;
	public event SessionDisconnected SessionDisconnectedCallback;

	private readonly SessionManager _sessionManager;
	private readonly SocketAsyncEventArgsPool _receiveArgsPool;
	private readonly Listener _listener;
	private readonly ServerConfig _config;

	public NetworkService(ServerConfig config)
	{
		_config = config;

		_sessionManager = new SessionManager(config.BufferSize);
		_receiveArgsPool = new SocketAsyncEventArgsPool(config.PoolCount, IO_Completed);
		_listener = new Listener(SessionConnected);
	}

	public void Start()
	{
		_listener.Start(_config.IP, _config.Port, _config.BackLog, _config.AcceptCount);
	}

	public Session GetSession(int sessionID)
	{
		return _sessionManager.GetSession(sessionID);
	}

	public void ServerDown()
	{
		_sessionManager.CloseAll();
	}

	private void SessionConnected(Socket socket)
	{
		var recvArgs = _receiveArgsPool.Pop();
		if (recvArgs == null)
		{
			return;
		}

		Session session = _sessionManager.Create(socket);
		recvArgs.UserToken = session;
		session._recvArgs = recvArgs;

		session.ReceiveCallback += SessionReceiveDataCallback;
		session.DisconnectedCallback += SessionDisconnectedCallback;
		session.ClosedCallback += SessionClosed;

		SessionConnectedCallback(session.SessionID);

		Task.Run(session.Start);
	}

	private void SessionClosed(Session session, bool disconnect)
	{
		_receiveArgsPool.Return(session._recvArgs);

		if (disconnect)
		{
			_sessionManager.Close(session.SessionID);
		}
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
