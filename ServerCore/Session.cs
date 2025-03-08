using System.Net.Sockets;

namespace ServerCore;

public delegate void SessionReceiveData(int sessionID, ArraySegment<byte> data);
public delegate void SessionConnected(int sessionID);
public delegate void SessionDisconnected(int sessionID);
public delegate void SessionClosed(Session session, bool disconnect);

internal enum SESSION_STATE
{
	NONE,
	SENDING,
	RESERVE_DISCONNECT,
	CLOSED,
}

public class Session
{
	public event SessionReceiveData ReceiveCallback;
	public event SessionDisconnected DisconnectedCallback;
	public event SessionClosed ClosedCallback;

	private int _sessionID;
	private Socket _socket;

	private object _lock = new object();
	private int _state = (int)SESSION_STATE.NONE;

	public SocketAsyncEventArgs _recvArgs;
	private RecvBuffer _recvBuffer;

	private SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
	private SendQueue _sendQueue = new();

	public int SessionID => _sessionID;

	public Session(int sessionID, Socket socket, int bufferSize)
	{
		_sessionID = sessionID;
		_socket = socket;

		_recvBuffer = new RecvBuffer(bufferSize * 4);
	}

	public void Start()
	{
		StartRecv(_recvArgs);
	}
	
	public void Disconnect()
	{
		int oldState = Interlocked.CompareExchange(ref _state, (int)SESSION_STATE.RESERVE_DISCONNECT, (int)SESSION_STATE.SENDING);
		if (oldState == (int)SESSION_STATE.SENDING)
		{
			// Send 중이면 Send 끝나고 Disconnect 실행
			return;
		}

		if (oldState == (int)SESSION_STATE.CLOSED)
		{
			return;
		}
		
		Close(true);
	}

	public void Close(bool disconnect)
	{
		if (Interlocked.Exchange(ref _state, (int)SESSION_STATE.CLOSED) == (int)SESSION_STATE.CLOSED)
		{
			return;
		}

		DisconnectedCallback?.Invoke(_sessionID);

		_sendQueue.Clear();

		try
		{
			_socket.Shutdown(SocketShutdown.Both);
		}
		catch 
		{ 
		}

		_socket.Close();
		_sendArgs?.Dispose();

		ClosedCallback?.Invoke(this, disconnect);

		_socket = null;
		_recvArgs = null;
		_sendArgs = null;
	}

	public void SendAsync(byte[] buffer)
	{
		_sendQueue.Add(buffer);

		if (Interlocked.CompareExchange(ref _state, (int)SESSION_STATE.SENDING, (int)SESSION_STATE.NONE) == (int)SESSION_STATE.NONE)
		{
			StartSend();
		}
	}

	public void SendAsync(List<byte[]> bufferList)
	{
		if (bufferList.Count == 0)
		{
			return;
		}

		_sendQueue.Add(bufferList);

		if (Interlocked.CompareExchange(ref _state, (int)SESSION_STATE.SENDING, (int)SESSION_STATE.NONE) == (int)SESSION_STATE.NONE)
		{
			StartSend();
		}
	}

	private void StartSend()
	{
		if (_state == (int)SESSION_STATE.CLOSED)
		{
			return;
		}

		var list = _sendQueue.GetSendQueue();

		if (list.Count == 1 )
		{
			_sendArgs.SetBuffer(list[0].Array, list[0].Offset, list[0].Count);
			_sendArgs.BufferList = null;
		}
		else
		{
			_sendArgs.SetBuffer(null, 0, 0);
			_sendArgs.BufferList = list;
		}

		try
		{
			if (!_socket.SendAsync(_sendArgs))
			{
				OnSendCompleted(_sendArgs);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[{nameof(StartSend)}] {ex.Message}");
			Interlocked.CompareExchange(ref _state, (int)SESSION_STATE.NONE, (int)SESSION_STATE.SENDING);
			Disconnect();
		}
	}

	private void OnSendCompleted(SocketAsyncEventArgs args)
	{
		lock (_lock)
		{
			if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
			{
				try
				{
					if (args.BufferList != null)
					{
						_sendQueue.Remove(args.BufferList.Count);
						args.BufferList = null;
					}
					else if (args.Buffer != null)
					{
						_sendQueue.Remove(1);
						args.SetBuffer(null, 0, 0);
					}

					if (_sendQueue.Count > 0 )
					{
						StartSend();
						return;
					}

					Interlocked.CompareExchange(ref _state, (int)SESSION_STATE.NONE, (int)SESSION_STATE.SENDING);
					if ( _state == (int)SESSION_STATE.RESERVE_DISCONNECT )
					{
						Disconnect();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"[{nameof(OnSendCompleted)}] {ex.Message}");
					Disconnect();
				}
			}
			else
			{
				Disconnect();
			}
		}
	}

	private void StartRecv(SocketAsyncEventArgs args)
	{
		if (_state == (int)SESSION_STATE.CLOSED)
		{
			return;
		}

		_recvBuffer.Clean();
		var recvBuf = _recvBuffer.FreeSegment;
		_recvArgs.SetBuffer(recvBuf.Array, recvBuf.Offset, recvBuf.Count);

		bool pending = _socket.ReceiveAsync(args);
		if (pending == false)
		{
			OnRecvCompleted(args);
		}
	}

	public void OnRecvCompleted(SocketAsyncEventArgs args)
	{
		if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
		{
			if ( _recvBuffer.Write(args.BytesTransferred) == false )
			{
				Disconnect();
				return;
			}

			var processLength = ProcessRecvData(_recvBuffer.DataSegment);
			if ( processLength > _recvBuffer.DataSize)
			{
				Disconnect();
				return;
			}

			if (_recvBuffer.Read(processLength) == false)
			{
				Disconnect();
				return;
			}

			StartRecv(args);
		}
		else
		{
			Disconnect();
		}
	}

	private int ProcessRecvData(ArraySegment<byte> buffer)
	{
		int processLength = 0;

		while (true)
		{
			if ( buffer.Count < NetworkDefine.HEADER_SIZE )
			{
				break;
			}

			var dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
			if ( buffer.Count < dataSize)
			{
				break;
			}

			ReceiveCallback(SessionID, new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

			processLength += dataSize;
			buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
		}

		return processLength;
	}

}
