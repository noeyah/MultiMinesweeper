using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Connector
{
	public Action<Socket> ConnectedHandler;

	private Socket _socket;

	public void Connect(string ip, int port)
	{
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

		Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		_socket = socket;

		SocketAsyncEventArgs args = new SocketAsyncEventArgs();
		args.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
		args.RemoteEndPoint = endPoint;

		if ( socket.ConnectAsync(args) == false )
		{
			OnConnectCompleted(null, args);
		}
	}

	private void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
	{
		if (args.SocketError != SocketError.Success)
		{
			Console.WriteLine(args.SocketError.ToString());
		}
		else
		{
			var socket = args.ConnectSocket;
			ConnectedHandler(socket);
		}
	}
}
