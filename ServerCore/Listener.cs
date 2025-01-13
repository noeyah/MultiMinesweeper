using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Listener
{
	public Action<Socket> AcceptHandler;

	private Socket _listenSocket;

	public void Start(string ip, int port, int backLog)
	{
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

		_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

		try
		{
			_listenSocket.Bind(endPoint);
			_listenSocket.Listen(backLog);

			var eventHandler = new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
			for ( int i = 0; i < backLog; i++ )
			{
				SocketAsyncEventArgs acceptEventArgs = new SocketAsyncEventArgs();
				acceptEventArgs.Completed += eventHandler;
				
				if ( _listenSocket.AcceptAsync(acceptEventArgs) == false )
				{
					OnAcceptCompleted(_listenSocket, acceptEventArgs);
				}
			}
		}
		catch (Exception ex)
		{
            Console.WriteLine(ex);
        }
	}

	private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
	{
		if (args.SocketError != SocketError.Success)
		{
			Console.WriteLine(args.SocketError.ToString());
		}
		else
		{
			var socket = args.AcceptSocket;
			AcceptHandler(socket);
		}

		// next accept
		args.AcceptSocket = null;
		bool acceptPending = true;

		try
		{
			acceptPending = _listenSocket.AcceptAsync(args);
		}
		catch (Exception ex )
		{
			Console.WriteLine(ex.ToString());
			//acceptPeding = true;
			return;
		}

		if (acceptPending == false)
		{
			OnAcceptCompleted(_listenSocket, args);
		}
	}
}
