using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Listener
{
	private Action<Socket> AcceptHandler;
	private Socket _listenSocket;
	private List<SocketAsyncEventArgs> _acceptArgsList = new();

	public Listener(Action<Socket> acceptHandler)
	{
		AcceptHandler = acceptHandler;
	}

	public void Start(string ip, int port, int backLog, int acceptCount)
	{
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
		_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

		try
		{
			_listenSocket.Bind(endPoint);
			_listenSocket.Listen(backLog);

			for (int i = 0; i < acceptCount; i++)
			{
				var acceptArgs = new SocketAsyncEventArgs();
				acceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
				_acceptArgsList.Add(acceptArgs);

				if (_listenSocket.AcceptAsync(acceptArgs) == false)
				{
					OnAcceptCompleted(null, acceptArgs);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
	}

	public void Stop()
	{
		if ( _listenSocket == null )
		{
			return;
		}

		foreach (var args in _acceptArgsList)
		{
			args.Completed -= new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
			args.Dispose();
		}
		_acceptArgsList.Clear();

		try
		{
			_listenSocket.Close();
		}
		finally
		{
			_listenSocket = null;
		}
	}

	private void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
	{
		if (args.SocketError != SocketError.Success)
		{
			Console.WriteLine(args.SocketError.ToString());
		}
		else
		{
			var socket = args.AcceptSocket;
			if (socket != null)
			{
				AcceptHandler(socket);
			}
		}

		args.AcceptSocket = null;

		try
		{
			if (_listenSocket.AcceptAsync(args) == false)
			{
				OnAcceptCompleted(null, args);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[{nameof(Listener)}] {ex}");
			return;
		}
	}

}
