using System.Buffers;

namespace ServerCore;

public class BufferPool
{
	private static ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;

	public static ArraySegment<byte> Rent(int size)
	{
		var buffer = _arrayPool.Rent(size);
		return new ArraySegment<byte>(buffer, 0, size);
	}

	public static void Return(ArraySegment<byte> buffer)
	{
		if ( buffer.Array is null )
		{
			return;
		}
		_arrayPool.Return(buffer.Array);
    }
}
