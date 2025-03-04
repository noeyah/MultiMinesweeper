using System.Collections.Concurrent;

namespace ServerCore;

internal class SendQueue
{
	private ConcurrentQueue<byte[]> _queue = new ConcurrentQueue<byte[]>();

	public void Add(byte[] data)
	{
		_queue.Enqueue(data);
	}

	public void Add(List<byte[]> list)
	{
		foreach (var item in list)
		{
			_queue.Enqueue(item);
		}
	}

	public List<ArraySegment<byte>> GetSendQueue()
	{
		return _queue.Select(v => new ArraySegment<byte>(v)).ToList();
	}

	public void Remove(int count)
	{
		for (int i = 0; i < count; ++i)
		{
			_queue.TryDequeue(out _);
		}
	}

	public void Clear()
	{
		_queue.Clear();
	}

	public int Count => _queue.Count;
}
