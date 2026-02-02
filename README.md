<!-- ===== HEADER ===== -->
<h1 align="center">Fantasy Marble 🎲🏙️</h1>
<p align="center">
  모두의 마블/모노폴리 규칙을 기반으로 한 <b>온라인 멀티플레이 턴제 보드게임</b><br/>
  (Unity 3D + Photon PUN2 + Firebase)
</p>

<br>

<!-- 링크 버튼 영역 -->
<p align="center">
  <a href="https://youtu.be/YEbacugrzeM?si=ASp_GeizJbAd5K-r">
    <img src="https://img.shields.io/badge/기능 소개 영상%20-YouTube-red?logo=youtube&logoColor=white" />
  </a>
  <a href="https://www.canva.com/design/DAGusJR6Rj8/oqtCCGhOprGTfJjlf6Ingw/edit?ui=eyJEIjp7IlQiOnsiQSI6IlBCajJ4MVB0RzRsUDRiY1gifX19">
    <img src="https://img.shields.io/badge/Portfolio%20Canva-핵심%20기술%20Canva-blue" />
  </a>
   <a href="https://www.canva.com/design/DAGjjZDsobQ/45St13yKBRlo2rLJ5f36Fg/edit?utm_content=DAGjjZDsobQ&utm_campaign=designshare&utm_medium=link2&utm_source=sharebutton">
    <img src="https://img.shields.io/badge/발표용%20Canva-발표용%20Canva-blue" />
  </a>
  <a href="https://www.notion.so/1ad60a26095a80a9bd69dc8d58218eeb?source=copy_link">
    <img src="https://img.shields.io/badge/Dev%20Notes-Notion-darkgray?logo=notion&logoColor=white" />
  </a>
</p>

<br>

<!-- ===== SCREENSHOTS (2x2) ===== -->
<table align="center">
  <tr>
    <td width="50%">
      <img src="https://github.com/user-attachments/assets/3cad22cf-1366-4e8f-b582-9c364c9c58da" alt="Fantasy Marble 1" width="100%"/>
    </td>
    <td width="50%">
      <img src="https://github.com/user-attachments/assets/b50024d5-db83-4c1f-ba68-6c3f2842b068" alt="Fantasy Marble 2" width="100%"/>
    </td>
  </tr>
  <tr>
    <td width="50%">
       <img src="https://github.com/user-attachments/assets/f0a374ac-6a96-4332-a446-aa3e78396373" alt="Fantasy Marble 4" width="100%"/>
    </td>
    <td width="50%">
       <img src="https://github.com/user-attachments/assets/1e184dff-a083-4ae0-a687-b5110bbce984" alt="Fantasy Marble 3" width="100%"/>
    </td>
  </tr>
</table>

<br>

## 🧾 프로젝트 정보
- 👥 개발 인원: **5명**
- 🗓️ 제작 기간: **2025.03.14 ~ 2025.04.03 (15일)**
- 🎮 장르: **턴제 보드게임** (모노폴리/모두의 마블 계열)
- 🧠 서버/데이터: **Photon PUN2 + Firebase Auth / Realtime Database**
- 📌 본 README는 포트폴리오 용도로 **구현된 시스템(코드 기준)** 중심으로 정리했습니다.

<br>

---

<br>

## 📋 목차
- [🎯 게임 소개](#game-intro)
- [🧠 핵심 기술](#key-tech)
- [👤 내 역할](#my-role)
- [✅ 구현 시스템 (코드 기준)](#what-i-built)
  - [🌐 멀티플레이 로비/룸/씬 흐름](#network-flow)
  - [⏳ 인게임 로딩 동기화](#loading-sync)
  - [🔁 턴/주사위/이동](#turn-dice-move)
  - [🗺️ 맵/타일 데이터 및 생성](#map-tile)
  - [🏗️ 타일 구매/건설 및 클릭 UI](#tile-ui)
  - [🎁 특수 타일/이벤트](#special)
  - [🏁 결과/게임 종료](#player-result)
  - [🔐 Firebase 로그인/유저데이터](#firebase)
  - [🧩 핵심 스크립트 맵](#core-scripts)
- [🛠️ 기술 스택](#tech-stack)
- [👨‍💻 개발자 소개](#developer)

<br>

---

<br>

<a name="game-intro"></a>
## 🎯 게임 소개
Fantasy Marble은 보드 위에서 **주사위(2개)**로 이동하고, 타일을 **구매/건설**하며  
**통행료 + 이벤트**를 통해 재화를 경쟁하는 **온라인 멀티플레이 턴제 보드게임**입니다. 🎲💸

- 🛰️ Photon PUN2 기반 턴 동기화 및 RPC 흐름
- 🔥 Firebase 기반 로그인 및 유저 데이터 저장(Realtime Database)

<br>

---

<br>

<a name="key-tech"></a>
## 🧠 핵심 기술 (코드 기반으로 확인된 구현)
아래 항목은 업로드된 스크립트들에서 확인되는 “실제 구현 요소”들만 모았습니다. ✅

### 🌐 네트워크/동기화 (Photon PUN2)
- 👑 **마스터 중심 권한 흐름** + RPC 기반 게임 진행 동기화
- 📡 `PunRPC`를 활용한 **턴 진행 / UI 갱신 / 소유권 변경 / 이벤트 적용** 동기화
- 🏠 방 생성/입장/퇴장, 대기실 Ready/Start, 씬 전환 플로우

### ⏳ 로딩/게임 시작 안정화
- 🧷 인게임 로딩 완료를 마스터에게 집계 → **전원 로딩 완료 후 턴 시스템 시작**
- 🧵 코루틴(`IEnumerator`) 기반 로딩/연출/상태 전환 처리

### 🔁 턴/주사위/이동 시스템
- 🎲 **주사위 2개 결과 기반 이동**
- 🔄 턴 카운트/라운드/다음 턴/정지/리셋 등 **턴 시스템 이벤트 분리**
- 🧠 이동/머니/파산 등 인게임 상태를 플레이어 단위로 관리

### 🗺️ 맵/타일 시스템
- 🧱 `ScriptableObject` 기반 타일 데이터 구조 (`TileInfoData`)
- 🧩 `partial` 기반 타일/데이터 확장 구조 (`TileController`, `TileInfoData`)
- 🏙️ 타일 소유/가격/통행료/총액 계산 로직 + RPC 소유권 동기화

### 🖱️ UI/입력/이벤트 드리븐 구조
- 🧾 타일 클릭 → 정보 패널/구매 패널 분기 UI
- 🧷 델리게이트/이벤트 방식으로 UI 텍스트/패널 갱신 연결 (`UIManagerP` 이벤트 구독)
- 🎛️ 옵션/토글/슬라이더 연동 UI 구성

### 🔐 Firebase (Auth + Realtime Database)
- 🧑‍💻 이메일/비밀번호 기반 로그인 & 회원가입 플로우
- 💾 제네릭 기반 저장/불러오기 구조(`SaveUserData<T>`, `LoadUserDataAsync<T>`)로 확장성 확보

### 🔊 사운드/설정 저장
- 🎚️ AudioMixer + 슬라이더(마스터/BGM/SFX) 연동
- 💽 사운드 설정을 **로컬 JSON 파일로 저장/로드** (IO 기반)

<br>

---

<br>

<a name="my-role"></a>
## 👤 내 역할
- 🧠 전체 시스템 구현(턴/주사위/이동, 타일/구매/건설, 이벤트 타일, UI, 네트워크 동기화)
- 🌐 Photon PUN2 기반 로비/룸/대기실/인게임 동기화
- 🔐 Firebase 로그인 및 유저 데이터 저장/로드 구조 구현
- 🔊 사운드 옵션 및 설정 저장 기능 구현

<br>

---

<br>

<a name="what-i-built"></a>
## ✅ 구현 시스템 (코드 기준)

<a name="network-flow"></a>
### 🌐 1) 멀티플레이 로비/룸/씬 흐름
- 접속/초기 진입: `PhotonNetworkMgr`
- 룸 생성/입장/나가기 및 씬 전환: `PhotonRoomMgr`, `LeaveRoomBtn`
- 대기실 Ready/Start 흐름: `WaitingRoomManager`
- 로비/룸 UI: `LobbyUI`, `RoomUI`, `LobbyProfileUI`, `PlayerRoomNickname`, `TitleUI`

<br>

<a name="loading-sync"></a>
### ⏳ 2) 인게임 로딩 동기화
- 로딩 완료 집계 후 턴 시작:
  - `InGameLoadingScript` → `TurnMgr.NotifyMasterPlayerLoaded()` → `TurnMgr.StartTurnSystem()`

<br>

<a name="turn-dice-move"></a>
### 🔁 3) 턴/주사위/이동
- 턴 시스템 동기화: `TurnMgr`, `TurnBasedManager`
- 주사위 생성/결과 처리/표시: `DiceManager`, `DiceButton`, `DiceNumText`
- 플레이어 인게임 상태(머니/이동/특수 상태 포함): `ServerIngamePlayer`

<br>

<a name="map-tile"></a>
### 🗺️ 4) 맵/타일 데이터 및 생성
- 타일 데이터(ScriptableObject): `TileInfoData`  
  - 타일 타입: `Ground`, `Sea`, `Item(Card)`, `Start`, `Island`, `Olympics`, `Travel`, `revenue`, `casino`
- 보드 타일 생성/배치: `MapManager`
- 타일 소유/가격/통행료/총액 계산: `TileController` (partial + RPC 소유권 동기화 포함)

<br>

<a name="tile-ui"></a>
### 🏗️ 5) 타일 구매/건설 및 클릭 UI
- Ground 구매/건설: `TileBuyUI`, `TileBulidingScript`
- Sea(관광지) 구매: `TileSeaBuyUI`
- 타일 클릭/정보 UI: `TileClick`, `TileClickGroundUI`, `TileClickSeaUI`, `PropertyPanel`, `AreaUI`, `UIManagerP`
- 미완/임시 처리: `UnFinishedTileClick`

<br>

<a name="special"></a>
### 🎁 6) 특수 타일/이벤트
- 🏅 올림픽: `Olympic` (통행료 2배 처리 포함)
- 🏝️ 무인도: `IslandUI` (턴 스킵 관련 UI)
- 🎴 보너스 카드: `BonusCardManager`, `BonusCardUI`  
  - 효과: **돈 증가 / 돈 감소 / 플레이어 이동**

<br>

<a name="player-result"></a>
### 🏁 7) 결과/게임 종료
- 게임오버/결과 UI: `GameOverUIScript`, `GameOverResultWindow`, `PlayerResult`
- 플레이어 UI 관리: `PlayerUIManager`, `PlayerUIController`
- 인게임 UI/서버 UI: `InGameUI`, `ServerUI`

<br>

<a name="firebase"></a>
### 🔐 8) Firebase 로그인/유저데이터
- Auth 로그인/회원가입/닉네임 플로우: `FirebaseLoginMgr`
- Realtime Database 저장/로드(제네릭): `FirebaseDataMgr`

<br>

<a name="core-scripts"></a>
## 🧩 핵심 스크립트 맵 (빠른 탐색)
- 🌐 Networking / Lobby / Room  
  - `PhotonNetworkMgr`, `PhotonRoomMgr`, `WaitingRoomManager`, `LeaveRoomBtn`
- 🔁 Turn / Dice  
  - `TurnMgr`, `TurnBasedManager`, `DiceManager`, `DiceButton`, `DiceNumText`
- 🗺️ Map / Tile  
  - `MapManager`, `TileInfoData`, `TileController`
- 🧑 Player  
  - `ServerIngamePlayer`
- 🏗️ Tile UI  
  - `UIManagerP`, `TileBuyUI`, `TileSeaBuyUI`, `TileClick*`, `PropertyPanel`, `AreaUI`
- 🎁 Events  
  - `BonusCardManager`, `BonusCardUI`, `Olympic`, `IslandUI`
- 🔐 Firebase  
  - `FirebaseLoginMgr`, `FirebaseDataMgr`
- 🔊 Sound / Option  
  - `SoundManager`, `GameSound`, `IngameSound`, `ClickSound`, `OptionButton`, `ToggleButtonImage`
- 🎥 Camera / Loading  
  - `CameraMove`, `LoadingText`, `LodingScripts`, `InGameLoadingScript`

<br>

---

<br>

<a name="tech-stack"></a>
## 🛠️ 기술 스택
- 🎮 **엔진:** Unity 3D  
- 💻 **언어:** C#  
- 🌐 **네트워크:** Photon PUN2 (RPC 기반 동기화)  
- 🔥 **백엔드:** Firebase Authentication, Firebase Realtime Database  
- 🧾 **UI:** Unity UI (버튼/패널/슬라이더/텍스트 이벤트 구독 구조 포함)  
- 🔊 **사운드:** AudioMixer + 슬라이더 설정, 로컬 JSON 저장/로드  
- 🧩 **데이터 구조:** ScriptableObject 기반 타일 데이터, partial 클래스 확장 구조  

<br>

---

<br>

<a name="developer"></a>
## 👨‍💻 개발자 소개
- TODO: GitHub / Blog / YouTube / Portfolio 링크 추가

<br>

---

## ✅ 참고
- 본 README는 업로드된 **54개 C# 스크립트 기준**으로 정리했습니다.
- 시스템상 예전에 업로드했던 일부 파일은 만료될 수 있습니다. (추가 반영이 필요하면 해당 파일만 다시 업로드하면 됩니다.)
