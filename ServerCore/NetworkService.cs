using System.Net.Sockets;

namespace ServerCore;

public abstract class NetworkService
{
	private SessionManager _sessionManager;

	private SocketAsyncEventArgsPool _argsPool;

	#region handler
	protected abstract void OnReceiveData(int sessionID, ArraySegment<byte> data);
	protected abstract void OnSendCompleted(int sessionID, int bytesTransferred, IList<ArraySegment<byte>> bufferList);
	protected abstract void OnConnected(int sessionID);
	protected abstract void OnDisconnected(int sessionID);
	#endregion


	public virtual void Init(int poolCount, int bufferSize)
	{
		_sessionManager = new SessionManager(bufferSize);
		_argsPool = new SocketAsyncEventArgsPool(poolCount, IO_Completed);
	}

	public Session GetSession(int sessionID)
	{
		return _sessionManager.GetSession(sessionID);
	}

	#region session event

	protected void Connected(Socket socket)
	{
		Session session = _sessionManager.Create();

		var recvArgs = _argsPool.Pop();
		var sendArgs = _argsPool.Pop();

		recvArgs.UserToken = session;
		sendArgs.UserToken = session;

		session.Set(socket, recvArgs, sendArgs);
		session.Received = OnReceiveData;
		session.SendCompleted = OnSendCompleted;
		session.Disconnected = OnDisconnected;
		session.Closed = OnClosed;

		OnConnected(session.SessionID);

		session.Start();
	}

	private void OnClosed(int sessionID, SocketAsyncEventArgs recvArgs, SocketAsyncEventArgs sendArgs)
	{
		_argsPool.Return(recvArgs);
		_argsPool.Return(sendArgs);
		_sessionManager.Close(sessionID);
	}

	private void IO_Completed(object sender, SocketAsyncEventArgs args)
	{
		switch (args.LastOperation)
		{
			case SocketAsyncOperation.Receive:
				ProcessReceive(args);
				break;
			case SocketAsyncOperation.Send:
				ProcessSend(args);
				break;
			default:
				throw new ArgumentException($"[ERROR] {nameof(IO_Completed)} - {args.LastOperation.ToString()}");
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

	private void ProcessSend(SocketAsyncEventArgs args)
	{
		var session = args.UserToken as Session;
		if (session is null)
		{
			return;
		}

		session.OnSendCompleted(args);
	}

	#endregion
}
