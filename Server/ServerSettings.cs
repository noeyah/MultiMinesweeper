
namespace Server;

public class ServerSettings
{
	public string IP { get; set; }
	public int Port { get; set; }
	public int BackLog { get; set; }
	public int PoolCount { get; set; }
	public int BufferSize { get; set; }
	public int TaskCount { get; set; }
	public int MaxConnectCount { get; set; }
	public int AcceptCount { get; set; }
}
