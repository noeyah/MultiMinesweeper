using System.Net.Sockets;

namespace ServerCore;
public class Session
{
	public delegate void ReceiveHandler(int sessionID, ArraySegment<byte> data);
	public delegate void SendCompletedHandler(int sessionID, byte[]? buffer, IList<ArraySegment<byte>>? bufferList);
	public delegate void DisconnectedHandler(int sessionID);
	public delegate void ClosedHandler(int sessionID, SocketAsyncEventArgs recvArgs, SocketAsyncEventArgs sendArgs);

	public ReceiveHandler Received;
	public SendCompletedHandler SendCompleted;
	public DisconnectedHandler Disconnected;
	public ClosedHandler Closed;

	private int _sessionID;
	private Socket _socket;

	private int _disconnected;

	private object _lock = new object();

	private SocketAsyncEventArgs _recvArgs;
	private SocketAsyncEventArgs _sendArgs;

	private RecvBuffer _recvBuffer;

	private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
	private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

	public int SessionID => _sessionID;

	public Session(int bufferSize)
	{
		_recvBuffer = new RecvBuffer(bufferSize * 4);
		_disconnected = 0;
	}

	public void SetID(int sessionID)
	{
		_sessionID = sessionID; 
	}

	public void Set(Socket socket, SocketAsyncEventArgs recvArgs, SocketAsyncEventArgs sendArgs)
	{
		_socket = socket;
		_recvArgs = recvArgs;
		_sendArgs = sendArgs;
	}

	public void Start()
	{
		StartRecv(_recvArgs);
	}

	public void Disconnect()
	{
        if ( Interlocked.Exchange(ref _disconnected, 1) == 1 )
		{
			return;
		}

		Disconnected(_sessionID);

		_socket.Shutdown(SocketShutdown.Both);
		_socket.Close();

		lock (_lock)
		{
			_sendQueue.Clear();
			_pendingList.Clear();
		}

		var recvArgs = _recvArgs;
		var sendArgs = _sendArgs;

		_socket = null;
		_recvArgs = null;
		_sendArgs = null;

		Closed(_sessionID, recvArgs, sendArgs);
	}

	public void Send(ArraySegment<byte> buffer)
	{
		lock (_lock)
		{
			_sendQueue.Enqueue(buffer);
			if ( _pendingList.Count == 0 )
			{
				StartSend();
			}
		}
	}
	public void Send(List<ArraySegment<byte>> bufferList)
	{
		if (bufferList.Count == 0)
		{
			return;
		}

		lock (_lock)
		{
			foreach (var buffer in bufferList)
			{
				_sendQueue.Enqueue(buffer);
			}

			if (_pendingList.Count == 0)
			{
				StartSend();
			}
		}
	}

	private void StartSend()
	{
		if (_disconnected == 1 || _sendQueue.Count == 0)
		{
			return;
		}

		while (_sendQueue.Count > 0)
		{
			var buffer = _sendQueue.Dequeue();
			_pendingList.Add(buffer);
		}

		if ( _pendingList.Count == 1 )
		{
			_sendArgs.SetBuffer(_pendingList[0].Array, _pendingList[0].Offset, _pendingList[0].Count);
			_sendArgs.BufferList = null;
		}
		else
		{
			_sendArgs.SetBuffer(null, 0, 0);
			_sendArgs.BufferList = _pendingList;
		}

		try
		{
            bool pending = _socket.SendAsync(_sendArgs);
			if (!pending)
			{
				OnSendCompleted(_sendArgs);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{nameof(StartSend)} - {ex.Message}");
		}
	}

	public void OnSendCompleted(SocketAsyncEventArgs args)
	{
		lock (_lock)
		{
			if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
			{
				try
				{
					SendCompleted(SessionID, _sendArgs.Buffer, _sendArgs.BufferList);
					
					_sendArgs.BufferList = null;
					_sendArgs.SetBuffer(null, 0, 0);
					_pendingList.Clear();

					if ( _sendQueue.Count > 0 )
					{
						StartSend();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"{nameof(OnSendCompleted)} - {ex.Message}");
				}
			}
			else
			{
                Disconnect();
			}
		}
	}

	public void StartRecv(SocketAsyncEventArgs args)
	{
		if ( _disconnected == 1)
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

			Received(SessionID, new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

			processLength += dataSize;
			buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
		}

		return processLength;
	}
}
