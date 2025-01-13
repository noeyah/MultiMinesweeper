namespace ServerCore;

class RecvBuffer
{
	private byte[] _buffer;
	private int _readPos;
	private int _writePos;

	public RecvBuffer(int bufferSize)
	{
		_buffer = new byte[bufferSize];
	}

	public int DataSize
	{
		get
		{
			return _writePos - _readPos;
		}
	}

	public int FreeSize
	{
		get
		{
			return _buffer.Length - _writePos;
		}
	}

	public ArraySegment<byte> DataSegment
	{
		get
		{
			return new ArraySegment<byte>(_buffer, _readPos, DataSize);
		}
	}

	public ArraySegment<byte> FreeSegment
	{
		get
		{
			return new ArraySegment<byte>(_buffer, _writePos, FreeSize);
		}
	}

	public void Clean()
	{
		int dataSize = DataSize;
		if ( dataSize == 0 )
		{
			_readPos = 0;
			_writePos = 0;
			return;
		}
		Array.Copy(_buffer, _readPos, _buffer, 0, dataSize);
		_readPos = 0;
		_writePos = dataSize;
	}

	public bool Write(int bytesTransferred)
	{
		if ( bytesTransferred > FreeSize )
		{
			return false;
		}

		_writePos += bytesTransferred;
		return true;
	}

	public bool Read(int readBytes)
	{
		if ( readBytes > DataSize )
		{
			return false;
		}

		_readPos += readBytes;
		return true;
	}
}
