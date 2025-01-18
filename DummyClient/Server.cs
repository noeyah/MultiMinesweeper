using MessagePack;
using Packet;
using ServerCore;
using System;

namespace DummyClient;

internal class Server : NetworkService
{
	private Connector _connector = new Connector();

	private PacketHandler _packetHandler = new PacketHandler();

	public override void Init(int poolCount, int bufferSize)
	{
		base.Init(poolCount, bufferSize);
		_connector.ConnectedHandler = Connected;

		_packetHandler.Init(this);
		_packetHandler.RegistPacketHandler();
	}

	public void Connect(string ip, int port, int clientCount)
	{
		for (int i = 0; i < clientCount; i++)
		{
			_connector.Connect(ip, port);
		}
	}

	protected override void OnConnected(int sessionID)
	{
		Console.WriteLine($"OnConnected - {sessionID}");

		//Thread.Sleep(1000);

		_packetHandler.Handler((ushort)PACKET_ID.Connected, sessionID, null);
	}

	protected override void OnDisconnected(int sessionID)
	{
		Console.WriteLine($"OnDisconnected - {sessionID}");
	}

	protected override void OnReceiveData(int sessionID, ArraySegment<byte> data)
	{
		if (data.Array is null)
		{
			Console.WriteLine("OnReceiveData - data.Array is null");
			return;
		}
		var dataSize = BitConverter.ToUInt16(data.Array, data.Offset);
		var packetID = BitConverter.ToUInt16(data.Array, data.Offset + NetworkDefine.HEADER_DATA_SIZE);

		var dataSegment = new ArraySegment<byte>(data.Array, data.Offset + NetworkDefine.HEADER_SIZE, data.Count - NetworkDefine.HEADER_SIZE);

		_packetHandler.Handler(packetID, sessionID, dataSegment);
	}

	protected override void OnSendCompleted(int sessionID, int bytesTransferred, IList<ArraySegment<byte>> bufferList)
	{
	}

	public void Send<T>(int sessionID, T packet) where T : IPacket
	{
		var data = MessagePackSerializer.Serialize(packet);
        var totalSize = data.Length + NetworkDefine.HEADER_SIZE;

		var buffer = new byte[totalSize];
		var span = buffer.AsSpan();

		BitConverter.TryWriteBytes(span.Slice(0, NetworkDefine.HEADER_DATA_SIZE), (ushort)totalSize);
		BitConverter.TryWriteBytes(span.Slice(NetworkDefine.HEADER_DATA_SIZE, NetworkDefine.HEADER_PACKET_ID_SIZE), (ushort)packet.PacketID);
		data.AsSpan().CopyTo(span.Slice(NetworkDefine.HEADER_SIZE));

		var session = GetSession(sessionID);
		session.Send(buffer);
	}
}
