using ServerCore;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Server;

internal class BroadcastWorker
{
	private readonly ConcurrentDictionary<int, List<ArraySegment<byte>>> _dicSend = new();

	private object _lock = new object();
	private bool _isProcessing = false;
	private readonly CancellationTokenSource _cts = new();

	private Func<int, Session> _sessionFunc;

	public void Init(Func<int, Session> getSessionFunc)
	{
		_sessionFunc = getSessionFunc;
	}

	public void AddPacket(int sessionID, ArraySegment<byte> buffer)
	{
		if (!_dicSend.ContainsKey(sessionID))
		{
			if (!_dicSend.TryAdd(sessionID, new List<ArraySegment<byte>>()))
			{
                Console.WriteLine($"AddPacket 실패 : {sessionID}");
                return;
			}
		}

		_dicSend[sessionID].Add(buffer);

		lock (_lock)
		{
			if (!_isProcessing)
			{
				_isProcessing = true;
				Task.Run(ProcessAsync);
			}
		}
	}

	public void RemovePacket(int sessionID)
	{
		_dicSend.TryRemove(sessionID, out _);
	}

	public void Stop()
	{
		_cts.Cancel();
	}

	private async Task ProcessAsync()
	{
		while (true)
		{
			lock ( _lock)
			{
				if (_dicSend.Count == 0)
				{
					_isProcessing = false;
					return;
				}
			}

			var list = new List<(int sessionID, List<ArraySegment<byte>> bufferList)>();

			foreach (var pair in _dicSend)
			{
				if ( _dicSend.TryRemove(pair.Key, out var value))
				{
					list.Add((pair.Key, value));
				}
			}

			if (list.Count > 0)
			{
				foreach ( var (sessionID, bufferList) in list )
				{
					var session = _sessionFunc(sessionID);
					if (session != null)
					{
						session.Send(bufferList);
					}
				}
			}

			await Task.Delay(300, _cts.Token);
		}
	}

}
