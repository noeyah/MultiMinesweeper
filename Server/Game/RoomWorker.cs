using System.Threading.Channels;

namespace Server;

internal class RoomWorker
{
	private readonly Channel<Action> _channel = Channel.CreateUnbounded<Action>();
	private readonly CancellationTokenSource _cts = new();

	public RoomWorker()
	{
		Task.Run(ProcessAsync);
	}

	public void StopProcess()
	{
		_channel.Writer.Complete();
		_cts.Cancel();
	}

	protected void Enqueue(Action action)
	{
		if (!_channel.Writer.TryWrite(action))
		{
			Console.WriteLine("GameRoom Enqueue 실패");
		}
	}

	private async Task ProcessAsync()
	{
		while (await _channel.Reader.WaitToReadAsync(_cts.Token))
		{
			while (_channel.Reader.TryRead(out var action))
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
                    Console.WriteLine(ex.Message);
                }
			}
		}
	}
}
