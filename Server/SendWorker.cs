using ServerCore;
using System.Collections.Concurrent;

namespace Server;

internal class SendWorker
{
	private readonly ConcurrentQueue<(int sessionID, ArraySegment<byte> buffer)> _sendQueue = new();

	private object _lock = new object();
	private bool _isProcessing = false;

	private Func<int, Session> _sessionFunc;

	public void Init(Func<int, Session> getSessionFunc)
	{
		_sessionFunc = getSessionFunc;
	}

	public void Send(int sessionID, ArraySegment<byte> buffer)
	{
		_sendQueue.Enqueue((sessionID, buffer));

		lock (_lock)
		{
			if (!_isProcessing)
			{
				_isProcessing = true;
				Task.Run(Process);
			}
		}
	}

	private void Process()
	{
		while (_sendQueue.TryDequeue(out var data))
		{
			var session = _sessionFunc(data.sessionID);
			if (session != null)
			{
				session.Send(data.buffer);
			}
		}

		lock (_lock)
		{
			_isProcessing = false;
			if ( _sendQueue.Count > 0 )
			{
				_isProcessing = true;
				Task.Run(Process);
			}
		}
	}

}
