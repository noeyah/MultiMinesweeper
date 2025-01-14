# Multi Minesweeeper


## 소개

멀티로 지뢰찾기 하는 게임입니다.

실행 영상 : <https://youtu.be/od36XPGzSlE>

버전 : .net 8.0

![mm_1](https://github.com/user-attachments/assets/ea41c527-5acc-46cb-9434-992c32bc4ee6)



## 프로젝트 설명

### ServerCore
- 소켓 통신 관리하는 IOCP 기반 라이브러리
- NetworkService를 상속 받아서 이벤트 처리
```cs
public abstract class NetworkService
{
	public virtual void Init(int poolCount, int bufferSize) {}
	
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
	// 구현 필요
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
	// 추상 메소드 구현 필요
}
```

### Server
- 게임 로직이 있는 실제 게임 서버
- 클라이언트로부터 받은 패킷을 Channel을 사용하여 순차적으로 처리
- 패킷이 없을 때는 Channel을 사용해서 대기 상태로 전환되어 자원을 최소한으로 사용


### Packet
- 서버, 클라이언트 공용으로 사용하는 패킷 라이브러리
- MessagePack 사용

### TestClient
- 윈폼 기반 테스트용 클라이언트

