
# 프로젝트 소개

비동기 소켓 라이브러리부터 게임 서버까지 구현한 프로젝트로, 포트폴리오용으로 제작되었습니다.

### 🛠 사용 기술 
.NET 8.0, C#

### 🎯 개발 목적 및 특징
- 멀티플레이 환경에서 동작하는 지뢰찾기 게임 서버 구현
- C# 비동기 소켓 서버
- 다중 클라이언트 지원 및 세션 관리
- 패킷 프로토콜 설계 및 패킷 처리 최적화
- 윈폼 기반 클라이언트 및 더미 클라이언트로 테스트 환경 구축

#### [🎥 실행 영상 바로가기](https://youtu.be/XJ2ABR74md4) : <https://youtu.be/XJ2ABR74md4>


---


# 게임 소개
여러 플레이어가 함께 참여하는 **실시간 지뢰찾기** 게임으로, 하나의 방에서 모든 플레이어가 동일한 게임을 진행합니다. 

### 게임 룰
- 방 내 모든 플레이어는 **하나의 동일한 지뢰찾기** 게임을 진행
- 난이도별 하나의 방이 있으며, 플레이어는 중간에 참여하거나 퇴장할 수 있음
- 게임이 **끝나야 초기화**할 수 있음
- **지뢰가 아닌 모든 칸을 열면 승리**
- 지뢰를 열면 패배


---


# 전체 구성도
- Server : 게임 로직과 패킷 처리하는 게임 서버
- ServerCore : 네트워크 통신과 세션 관리를 담당
- Packet : 클라이언트와 서버 간의 데이터 패킷, 공용 Enum 등 포함
- TestClient : 윈폼 기반 클라이언트. 게임 보드 및 인터페이스 제공
- DummyClient : 서버 부하 테스트용 클라이언트


![Image](https://github.com/user-attachments/assets/969652a3-6121-43c8-804f-53c06bc73f7f)



---


# 테스트 화면

- TestClient

![Image](https://github.com/user-attachments/assets/5db81d04-f422-4192-b5d8-eebdd6b237b8)

- DummyClient

![Image](https://github.com/user-attachments/assets/4349a217-f874-48f6-9fc6-f0de9d92dc9d)