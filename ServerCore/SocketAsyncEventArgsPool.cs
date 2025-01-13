using System.Collections.Concurrent;
using System.Net.Sockets;

namespace ServerCore;

class SocketAsyncEventArgsPool
{
	private ConcurrentQueue<SocketAsyncEventArgs> _argsPool = new ConcurrentQueue<SocketAsyncEventArgs>();
	private EventHandler<SocketAsyncEventArgs> _completedHandler;

	public SocketAsyncEventArgsPool(int poolCount, Action<object, SocketAsyncEventArgs> completedHandler)
	{
		_completedHandler = new EventHandler<SocketAsyncEventArgs>(completedHandler);

		SocketAsyncEventArgs args;
		for (int i = 0; i < poolCount; i++)
		{
			args = new SocketAsyncEventArgs();
			args.Completed += _completedHandler;
			_argsPool.Enqueue(args);
		}
	}

	public SocketAsyncEventArgs Pop()
	{
		if ( !_argsPool.TryDequeue(out var args) )
		{
			args = new SocketAsyncEventArgs();
			args.Completed += _completedHandler;
		}
		return args;
	}

	public void Return(SocketAsyncEventArgs args)
	{
		args.UserToken = null;
		_argsPool.Enqueue(args);
	}
}
