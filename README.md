# Multi Minesweeeper

## 소개

멀티로 지뢰찾기 하는 게임입니다.

실행 영상 : <https://youtu.be/od36XPGzSlE>

버전 : .net 8.0

## 프로젝트 설명

### Packet
- 서버, 클라이언트 공용으로 사용하는 패킷 라이브러리
- MessagePack 사용

### Server
- 실제 게임 서버
- 클라이언트 요청 처리 및 게임 로직

### ServerCore
- 소켓 통신 관리하는 IOCP 라이브러리
- NetworkService를 상속 받아 사용 
- (예시: Server 프로젝트의 MainServer)

### TestClient
- 윈폼 기반 테스트용 클라이언트

