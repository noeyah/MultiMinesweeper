# Multi Minesweeeper


## 소개

IOCP 기반 소켓 통신 라이브러리부터 게임 서버까지 구현한 프로젝트.

멀티로 지뢰찾기 하는 게임입니다.

실행 영상 : <https://youtu.be/od36XPGzSlE>

사용 기술 : C#, .net 8.0

![mm_1](https://github.com/user-attachments/assets/ea41c527-5acc-46cb-9434-992c32bc4ee6)

### 게임 룰
- 여러명이서 하나의 지뢰찾기를 실시간으로 진행
- 게임이 끝나야지만 초기화 가능
- 중간에 접속한 유저도 참여 가능
- 다른 유저의 플래그 취소 가능
- 한 명이라도 지뢰을 누르면 GAME OVER

---
## 각 프로젝트 설명

게임 서버 프로젝트 : ServerCore, Server, Packet
테스트용 프로젝트 : TestClient, DummyClient

### ServerCore
- 소켓 통신 관리하는 IOCP 기반 라이브러리
- NetworkService를 상속 받아서 이벤트 처리
- SocketAsyncEventArgsPool : SocketAsyncEventArgs을 풀링해서 재사용
- SessionManager : SessionID를 부여하고 관리
```cs
public abstract class NetworkService
{
	// poolCount : SAEA 초기 풀링 개수. 초과시 더 생성
	public virtual void Init(int poolCount, int bufferSize) {}
	
	// 연결 시도/수락 핸들러에 등록 필요
	protected void Connected(Socket socket){}
	
	// 구현 필요
	protected abstract void OnReceiveData(int sessionID, ArraySegment<byte> data);
	protected abstract void OnSendCompleted(int sessionID, int bytesTransferred, IList<ArraySegment<byte>> bufferList);
	protected abstract void OnConnected(int sessionID);
	protected abstract void OnDisconnected(int sessionID);
}
```
```cs
// 서버에서 사용 예시
internal class MainServer : NetworkService
{
	private Listener _listener = new Listener();

	public override void Init(int poolCount, int bufferSize)
	{
		base.Init(poolCount, bufferSize);

		_listener.AcceptHandler = Connected;
	}

	protected override void OnConnected(int sessionID) {}
	protected override void OnDisconnected(int sessionID) {}
	protected override void OnReceiveData(int sessionID, ArraySegment<byte> data) {}
	protected override void OnSendCompleted(int sessionID, int bytesTransferred, IList<ArraySegment<byte>> bufferList) {}
}
```
```cs
// 클라이언트에서도 사용 가능
internal class Server : NetworkService
{
	private Connector _connector = new Connector();

	public override void Init(int poolCount, int bufferSize)
	{
		base.Init(poolCount, bufferSize);
		_connector.ConnectedHandler = Connected;
	}
}
```

### Server
- 게임 로직이 있는 실제 게임 서버
- 클라이언트로부터 받은 패킷은 PacketProcessor에서 순차적으로 처리
- Channel을 사용해서 패킷이 없을 때는 대기 상태로 전환되어 자원을 최소한으로 사용
```cs
internal class PacketProcessor
{
	private Channel<PacketData> _channel = Channel.CreateUnbounded<PacketData>();

	private async Task ProcessAsync()
	{
		while (await _channel.Reader.WaitToReadAsync())
		{
			while (_channel.Reader.TryRead(out var packetData))
			{
				// packet 처리
			}
		}
	}
}
```


### Packet
- 서버, 클라이언트 공용으로 사용하는 패킷 라이브러리
- MessagePack 사용
- 패킷은 IPacket 인터페이스 구현하고, PacketID 정의
```cs
[MessagePackObject]
public class LoginReq : IPacket
{
	[Key(0)]
	public string Name;

	[IgnoreMember]
	public PACKET_ID PacketID => PACKET_ID.LoginReq;
}
```

### TestClient, DummyClient
- 테스트용으로 사용
- TestClient : 윈폼 기반으로 게임 진행
- DummyCient : 콘솔, 클라 여러개 연결

