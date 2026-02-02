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
- 개발 인원: **5명**
- 제작 기간: **2025.03.14 ~ 2025.04.03 (15일)**
- 장르: **턴제 보드게임** (모노폴리/모두의 마블 계열)
- 서버/데이터: **Photon PUN2 + Firebase Auth / Realtime Database**
- 본 README는 포트폴리오 용도로 **구현된 시스템(코드 기준)** 중심으로 정리했습니다.

<br>

---

<br>

## 📌 목차
- [게임 소개](#game-intro)
- [핵심 기술](#key-tech)
- [내 역할](#my-role)
- [구현 시스템 (코드 기준)](#what-i-built)
  - [멀티플레이 로비/룸/씬 흐름](#network-flow)
  - [턴/주사위/이동](#turn-dice-move)
  - [맵/타일 데이터 및 생성](#map-tile)
  - [타일 구매/건설 및 클릭 UI](#tile-ui)
  - [특수 타일/이벤트](#special)
  - [결과/게임 종료](#player-result)
  - [Firebase 로그인/유저데이터](#firebase)
  - [핵심 스크립트 맵](#core-scripts)
- [기술 스택](#tech-stack)
- [개발자 소개](#developer)

<br>

---

<br>

<a name="game-intro"></a>
## 🎯 게임 소개
Fantasy Marble은 보드 위에서 **주사위(2개)**로 이동하고, 타일을 **구매/건설**하며  
**통행료 + 이벤트**를 통해 재화를 경쟁하는 **온라인 멀티플레이 턴제 보드게임**입니다.

- Photon PUN2 기반 턴 동기화 및 RPC 흐름
- Firebase 기반 로그인 및 유저 데이터 저장(Realtime Database)

<br>

---

<br>

<a name="key-tech"></a>
## 🧠 핵심 기술 (코드 기반)
아래 항목은 업로드된 스크립트들에서 확인되는 “실제 구현 요소”들 중, 요청하신 참여/담당 범위를 반영해 정리했습니다.

### 1) 네트워크/동기화 (Photon PUN2) - 참여
- 마스터 중심 권한 흐름 + RPC 기반 게임 진행 동기화
- `PunRPC`를 활용한 턴 진행 / UI 갱신 / 소유권 변경 / 이벤트 적용 동기화
- 방 생성/입장/퇴장, 대기실 Ready/Start, 씬 전환 플로우

### 2) 턴/주사위/이동 시스템 - 참여
- 주사위(2개) 결과 기반 이동 흐름
- 턴 진행(턴 카운트/다음 턴/정지/리셋 등)과 UI 이벤트 분리
- 플레이어 단위 인게임 상태(머니/이동/특수 상태) 처리 흐름

### 3) 맵/타일 시스템 - 전담
- `ScriptableObject` 기반 타일 데이터 구조 (`TileInfoData`)
- 보드 타일 생성/배치 로직 (`MapManager`)
- 타일 소유/가격/통행료/총액 계산 + RPC 소유권 동기화 (`TileController`)

### 4) UI/입력/이벤트 구조 - 전담
- 타일 클릭 → 정보 패널/구매 패널 분기 UI
- 이벤트/구독 기반 UI 갱신 구조 (`UIManagerP` 이벤트 구독)
- 타일 구매/건설/관광지 구매 UI 흐름 전반

### 5) Firebase (Auth + Realtime Database) - 참여
- 이메일/비밀번호 기반 로그인/회원가입/닉네임 설정 흐름
- 유저 데이터 저장/로드(제네릭) 구조로 확장성 확보

<br>

---

<br>

<a name="my-role"></a>
## 👤 내 역할
- 네트워크(Photon PUN2, RPC 동기화): **참여**
- 턴/주사위/이동 시스템: **참여**
- 맵/타일 시스템(데이터, 생성, 상태/소유/통행료 계산): **전담**
- UI/입력/이벤트 구조(타일 클릭/정보/구매/건설 UI 포함): **전담**
- Firebase(Auth/Realtime Database) 로그인 및 유저 데이터 저장/로드: **참여**

<br>

---

<br>

<a name="what-i-built"></a>
## ✅ 구현 시스템 (코드 기준)

<a name="network-flow"></a>
### 1) 멀티플레이 로비/룸/씬 흐름
- 접속/초기 진입: `PhotonNetworkMgr`
- 룸 생성/입장/나가기 및 씬 전환: `PhotonRoomMgr`, `LeaveRoomBtn`
- 대기실 Ready/Start 흐름: `WaitingRoomManager`
- 로비/룸 UI: `LobbyUI`, `RoomUI`, `LobbyProfileUI`, `PlayerRoomNickname`, `TitleUI`

<br>

<a name="turn-dice-move"></a>
### 2) 턴/주사위/이동
- 턴 시스템 동기화: `TurnMgr`, `TurnBasedManager`
- 주사위 생성/결과 처리/표시: `DiceManager`, `DiceButton`, `DiceNumText`
- 플레이어 인게임 상태(머니/이동/특수 상태 포함): `ServerIngamePlayer`

<br>

<a name="map-tile"></a>
### 3) 맵/타일 데이터 및 생성
- 타일 데이터(ScriptableObject): `TileInfoData`
- 보드 타일 생성/배치: `MapManager`
- 타일 소유/가격/통행료/총액 계산: `TileController` (partial + RPC 소유권 동기화 포함)

<br>

<a name="tile-ui"></a>
### 4) 타일 구매/건설 및 클릭 UI
- Ground 구매/건설: `TileBuyUI`, `TileBulidingScript`
- Sea(관광지) 구매: `TileSeaBuyUI`
- 타일 클릭/정보 UI: `TileClick`, `TileClickGroundUI`, `TileClickSeaUI`, `PropertyPanel`, `AreaUI`, `UIManagerP`
- 미완/임시 처리: `UnFinishedTileClick`

<br>

<a name="special"></a>
### 5) 특수 타일/이벤트
- 올림픽: `Olympic` (통행료 2배 처리 포함)
- 무인도: `IslandUI`
- 보너스 카드: `BonusCardManager`, `BonusCardUI`

<br>

<a name="player-result"></a>
### 6) 결과/게임 종료
- 게임오버/결과 UI: `GameOverUIScript`, `GameOverResultWindow`, `PlayerResult`
- 플레이어 UI 관리: `PlayerUIManager`, `PlayerUIController`
- 인게임 UI/서버 UI: `InGameUI`, `ServerUI`

<br>

<a name="firebase"></a>
### 7) Firebase 로그인/유저데이터
- Auth 로그인/회원가입/닉네임 플로우: `FirebaseLoginMgr`
- Realtime Database 저장/로드(제네릭): `FirebaseDataMgr`

<br>

---

<br>

<a name="core-scripts"></a>
## 🧩 핵심 스크립트 맵 (빠른 탐색)
- Networking / Lobby / Room
  - `PhotonNetworkMgr`, `PhotonRoomMgr`, `WaitingRoomManager`, `LeaveRoomBtn`
- Turn / Dice
  - `TurnMgr`, `TurnBasedManager`, `DiceManager`, `DiceButton`, `DiceNumText`
- Map / Tile
  - `MapManager`, `TileInfoData`, `TileController`
- Player
  - `ServerIngamePlayer`
- Tile UI
  - `UIManagerP`, `TileBuyUI`, `TileSeaBuyUI`, `TileClick*`, `PropertyPanel`, `AreaUI`
- Events
  - `BonusCardManager`, `BonusCardUI`, `Olympic`, `IslandUI`
- Firebase
  - `FirebaseLoginMgr`, `FirebaseDataMgr`

<br>

---

<br>

<a name="tech-stack"></a>
## 🛠️ 기술 스택
- 엔진: Unity 3D
- 언어: C#
- 네트워크: Photon PUN2 (RPC 기반 동기화)
- 백엔드: Firebase Authentication, Firebase Realtime Database
- UI: Unity UI
- 데이터/구조: ScriptableObject 기반 타일 데이터, partial 클래스 확장 구조

<br>

---

<br>

<a name="developer"></a>
## 👨‍💻 개발자 소개
- TODO: GitHub / Blog / YouTube / Portfolio 링크 추가
