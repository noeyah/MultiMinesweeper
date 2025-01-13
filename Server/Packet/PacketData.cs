using MessagePack;
using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server;

internal class PacketData
{
	public int SessionID;
	public ushort PacketID;
	public ushort Size;
	public ArraySegment<byte> Body;
}
