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
		while (true)
		{
			(int sessionID, ArraySegment<byte> buffer) data;

			lock (_lock)
			{
				if (!_sendQueue.TryDequeue(out data))
				{
					_isProcessing = false;
					return;
				}
			}

			// send 처리
			var session = _sessionFunc(data.sessionID);
			if (session != null)
			{
				session.Send(data.buffer);
			}
		}
	}

}
