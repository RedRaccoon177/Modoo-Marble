<!-- ===== HEADER ===== -->
<h1 align="center">🎲 Fantasy Marble 🎲</h1>
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

## 프로젝트 정보
- 개발 인원: **5명**
- 제작 기간: **2025.03.14 ~ 2025.04.03 (15일)**
- 장르: **턴제 보드게임** (모노폴리/모두의 마블 계열)
- 서버/데이터: **Photon PUN2 + Firebase Auth / Realtime Database**
- 본 README는 포트폴리오 용도로 **구현된 시스템(코드 기준)** 중심으로 정리했습니다.

<br>

---

<br>

## 목차
- [게임 소개](#game-intro)
- [핵심 기술](#key-tech)
- [내 역할](#my-role)
- [구현 시스템 (코드 기준)](#what-i-built)
  - [멀티플레이 로비/룸 흐름](#network-flow)
  - [턴/주사위/이동](#turn-dice-move)
  - [맵/타일 데이터 및 생성](#map-tile)
  - [부동산(땅/건물) 구매 및 자산/통행료 계산](#real-estate)
  - [타일 클릭 UI / 구매 UI / 이벤트 UI](#ui-system)
  - [특수 타일/이벤트](#special)
  - [결과/게임 종료](#player-result)
  - [Firebase 로그인/유저데이터](#firebase)
- [기술 스택](#tech-stack)
- [개발자 소개](#developer)

<br>

---

<br>

<a name="game-intro"></a>
## 🎯 게임 소개
Fantasy Marble은 보드 위에서 <strong>주사위(2개)</strong>로 이동하고, 타일을 **구매/건설**하며  
**통행료 + 이벤트**로 재화를 경쟁하는 **온라인 멀티플레이 턴제 보드게임**입니다.

- Photon PUN2 기반 동기화(권한/턴/RPC)
- Firebase 기반 로그인 및 유저 데이터 저장(Realtime Database)

<br>

---

<br>

<a name="key-tech"></a>
## 🧠 핵심 기술 (코드 기반)

### 1) Photon PUN2 기반 권한/동기화 설계
멀티 턴제 보드게임에서 가장 중요한 목표는 <strong>상태의 단일성(Consistency)</strong>입니다.  
이 프로젝트는 **마스터 권한 + RPC 커밋** 중심으로, 게임 규칙에 영향을 주는 상태를 일관되게 확정하도록 구성했습니다.

- **권한 모델: 마스터 중심 진행**
  - 맵/타일 상태, 턴 진행, 소유권 변경 같은 “규칙 핵심 상태”는 **마스터를 기준점**으로 확정되도록 설계했습니다.
  - 클라이언트는 UI 입력을 발생시키되, 최종 반영은 RPC로 커밋되어 모든 클라이언트에 동일하게 적용되는 형태입니다.

- **RPC 기반 커밋(Commit) 전략**
  - `PunRPC`를 통해 **소유권 변경**, **이벤트 효과 적용**, **플레이어 상태(머니/파산/정산)** 등을 동기화합니다.
  - “값만 바꾸는 동기화”가 아니라, 값 변경 이후의 화면 반영까지 동일 로직으로 이어지도록 구성되어 멀티 환경에서 시각 불일치를 줄입니다.

- **로비/룸/대기실 플로우 분리**
  - 로비/룸 관련 책임(`PhotonNetworkMgr`, `PhotonRoomMgr`, `WaitingRoomManager`)과 인게임 로직(턴/맵/타일)을 분리해
    네트워크 이벤트가 인게임 규칙 코드에 직접 얽히지 않도록 구성했습니다.
  - 결과적으로 룸 UI 변경/추가가 생겨도 인게임 규칙 영향이 최소화됩니다.

- **대표 책임 스크립트**
  - 접속/초기 진입: `PhotonNetworkMgr`
  - 룸 생성/입장/나가기/씬 전환: `PhotonRoomMgr`, `LeaveRoomBtn`
  - 대기실 Ready/Start: `WaitingRoomManager`

<br>

### 2) 턴/주사위/이동의 “턴제 보드게임” 파이프라인
턴제 보드게임의 핵심은 <strong>입력 타이밍 제어(중복 입력 방지)</strong>와 **단계별 처리의 명확성**입니다.  
이 프로젝트는 “턴 진행 → 주사위 → 이동 → 도착 처리 → 턴 종료”를 파이프라인으로 고정하고, UI와 규칙 로직을 분리했습니다.

- **턴 진행의 중앙 제어**
  - `TurnMgr`, `TurnBasedManager` 중심으로 턴 상태(내 턴/진행 단계/종료/다음 턴)를 관리해,
    멀티에서 흔한 “동시 입력/중복 실행” 문제를 구조적으로 줄이는 방향입니다.

- **주사위: 입력/처리/표기 분리**
  - 입력(`DiceButton`), 생성/계산(`DiceManager`), 표기(`DiceNumText`)로 분리하여
    UI가 바뀌어도 주사위 규칙은 유지되고, 반대로 규칙이 바뀌어도 UI 수정 범위를 최소화합니다.

- **이동/정산과 플레이어 상태의 단일화**
  - 플레이어의 돈/자산/파산/정산 관련 값은 `ServerIngamePlayer`가 중심이 되어 관리되며,
    변경 시 동기화가 끊기지 않도록 RPC 기반으로 맞추는 구조를 취합니다.
  - 보드게임 특성상 “돈이 바뀌는 경로”가 여러 곳으로 퍼지면 버그가 급증하는데,
    이를 한 축으로 모으는 방식으로 설계 포인트를 잡았습니다.

- **대표 책임 스크립트**
  - 턴: `TurnMgr`, `TurnBasedManager`
  - 주사위: `DiceManager`, `DiceButton`, `DiceNumText`
  - 플레이어 상태/정산: `ServerIngamePlayer`

<br>

### 3) 데이터 기반 맵/타일 구조
보드게임의 맵/타일은 양이 많고 규칙이 다양해 **하드코딩이 곧 유지보수 비용**이 됩니다.  
이 프로젝트는 ScriptableObject 기반 데이터 모델로 <strong>“타일 추가 = 데이터/프리팹 확장”</strong>이 되도록 구성했습니다.

- **타일 데이터 모델: ScriptableObject**
  - `TileInfoData`에 타일의 타입/서브타입/가격/통행료/소유 상태 등 규칙 데이터를 담는 형태로 설계했습니다.
  - 데이터 중심 설계 덕분에 타일 종류/밸런스 조정이 코드 수정 없이도 확장 가능한 구조입니다.

- **생성 파이프라인: 마스터 생성 + 데이터 주입**
  - `MapManager`가 마스터에서 타일을 생성(`PhotonNetwork.Instantiate("Tile", ...)`)하고,
    생성된 타일 오브젝트에 `SetTileData()`로 데이터를 주입하는 흐름입니다.
  - 이 방식은 “보드 상태의 원본”을 통일시키는 데 유리하고,
    멀티에서 생성 순서/기준점이 달라져 생기는 문제를 줄입니다.

- **표현 분기: SubTileType 기반 프리팹 활성화**
  - `MapManager.TileSetting()`에서 `SubTileType`에 따라 타일 내부 오브젝트를 켜는 방식으로,
    새 타일 타입이 늘어나도 **로직 수정 범위를 최소화**하도록 구성했습니다.

- **대표 책임 스크립트**
  - 데이터: `TileInfoData`
  - 생성/배치: `MapManager`
  - 타일 상태/소유/가격/통행료: `TileController` (partial 구조 포함)

<br>

### 4) 부동산 시스템(땅/건물) 설계와 거래 UX
이 프로젝트에서 가장 공을 들인 파트로, “구매/건설 → 소유권 반영 → 통행료/자산 계산”을 **일관된 데이터 구조**로 묶었습니다.

- **하나의 타일에 다단계 소유 슬롯**  
  `TileController`는 타일을 단일 소유로 끝내지 않고, 인덱스로 분리된 소유 슬롯을 가집니다.  
  - 0: Land(땅), 1: Pension, 2: Condo, 3: Hotel
  - `GetPrice(index)`, `GetOwner(index)`, `GetTollPrice(index)`로 단계별 규칙을 동일 인터페이스로 처리합니다.

- **구매 UI의 ‘거래 프리뷰(시뮬레이션) → 확정 커밋’ 구조**  
  `TileBuyUI`는 선택 단계에서 구매 가능 여부/합계를 실시간으로 갱신하고, 확정 시점에만 커밋하도록 구성했습니다.
  - 선택 단계: 체크/토글 → 비용 가능 여부 판단 → UI 상태 및 합계 갱신
  - 확정 단계: `BuyButtonClick()`에서 선택 슬롯만 소유권 커밋(RPC)
  - 취소 단계: `CancelBtnClick()`로 임시 선택/금액을 원복하는 UX 흐름

- **소유권 반영 + 시각 피드백 일원화**  
  `TileController.SetOwner(index, owner)`에서 머티리얼/오브젝트 활성화까지 함께 처리하여  
  “네트워크 동기화 = 화면 동기화”가 되도록 구성했습니다.

- **통행료/자산 합계 계산 루틴**
  - 타일 단위 합계: `TotalTollPrice()` / `TotalBuyPrice()`
  - 플레이어 단위 총 자산: `ServerIngamePlayer.TotalMoney()`에서 “현금 + 부동산 가치” 합산

> 참고: 자동 매각/정렬 기반 비용 충당 로직은 `ServerIngamePlayer` 쪽에서 확인된 바 있습니다(정렬/리스트 관리 포함).  
> 다만 세부 매각 기준을 README에 더 “근거 코드 중심”으로 박으려면 관련 파일이 전부 열려 있어야 해서, 필요 시 해당 파일만 재업로드 받으면 더 강화 가능합니다.

<br>

### 5) UI 입력 이벤트/데이터 바인딩 구조
UI가 커질수록 “직접 참조”는 유지보수를 급격히 어렵게 만듭니다.  
이 프로젝트는 **이벤트 기반 갱신**으로 UI 의존도를 낮추고, 입력-데이터-표시의 경로를 정리했습니다.

- **UIManagerP를 이벤트 허브로 사용**
  - `UIManagerP`에서 `Action` 이벤트를 제공하고, 각 UI가 이를 구독하여 화면을 갱신합니다.
  - 클릭/구매/주사위/이벤트 등 “상태 변화 트리거”를 이벤트로 모아,
    UI 스크립트들이 서로의 내부를 직접 참조하지 않도록 구성했습니다.

- **클릭 정보 UI vs 거래 UI 역할 분리**
  - 클릭/정보(열람): `TileClick`, `TileClickGroundUI`, `TileClickSeaUI`, `PropertyPanel`, `AreaUI`
  - 거래(구매/건설): `TileBuyUI`, `TileSeaBuyUI`
  - 정보 표시와 거래 로직이 섞이지 않도록 분리해서, UI 복잡도가 올라가도 유지보수성이 유지되게 구성했습니다.

- **입력 이벤트 기반 갱신의 장점**
  - “타일 클릭 → 정보 패널 갱신”과 “구매 UI 갱신”이 같은 클릭에서 일어나더라도,
    이벤트로 분리되어 충돌/순서 문제를 줄입니다.
  - UI가 늘어나도 이벤트를 추가 구독하는 방식으로 확장 가능해, 확장성에 강합니다.

<br>

### 6) Firebase Auth + Realtime Database
멀티플레이 게임에서 “계정/유저 데이터”는 필수이며, 이 프로젝트는 Auth와 Realtime DB를 결합해 구조화했습니다.

- **Auth: 로그인/회원가입/닉네임 설정의 단일 플로우**
  - `FirebaseLoginMgr`에서 이메일/비밀번호 기반 로그인과 회원가입, 닉네임 설정 흐름을 관리합니다.
  - 로그인 단계별 UI 패널을 분리하고(로그인/회원가입/닉네임) 각 단계의 입력/검증/전환을 명확히 처리하는 구조입니다.

- **Realtime Database: 저장/로드의 확장 가능한 패턴**
  - `FirebaseDataMgr`에서 **제네릭 기반 저장/로드 구조**를 제공하여, 유저 데이터 구조가 늘어나도 동일 패턴으로 확장 가능합니다.
  - 비동기 로드(`LoadUserDataAsync<T>`)를 통해 네트워크 I/O가 UI 프레임을 막지 않도록 설계 방향을 잡았습니다.

- **포트폴리오 포인트(설계 관점)**
  - “로그인(Auth)으로 사용자 식별 → DB 경로로 유저 데이터 저장/조회”의 기본 아키텍처를 갖추고,
    추후 스킨/재화/승패 기록/칭호 등 데이터가 늘어날 때도 구조를 유지할 수 있게 설계했습니다.


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
### 1) 멀티플레이 로비/룸 흐름 (Photon)
- 접속/초기 진입: `PhotonNetworkMgr`
- 룸 생성/입장/나가기 및 씬 전환: `PhotonRoomMgr`, `LeaveRoomBtn`
- 대기실 Ready/Start 흐름: `WaitingRoomManager`
- 로비/룸 UI: `LobbyUI`, `RoomUI`, `LobbyProfileUI`, `PlayerRoomNickname`, `TitleUI`

핵심 포인트
- 룸 상태 변화와 UI 전환이 분리되어 있어, 네트워크 이벤트가 UI에 직접 하드코딩되지 않게 구성되어 있습니다.
- 마스터 기준 흐름을 토대로 인게임 시스템(턴/맵)이 동작하도록 연결됩니다.

<br>

<a name="turn-dice-move"></a>
### 2) 턴/주사위/이동
- 턴 시스템 동기화: `TurnMgr`, `TurnBasedManager`
- 주사위 생성/결과 처리/표시: `DiceManager`, `DiceButton`, `DiceNumText`
- 플레이어 인게임 상태(머니/자산/파산 포함): `ServerIngamePlayer`

강화 포인트(포트폴리오 관점)
- UI 입력(버튼) → 턴 로직 → 이동/도착 처리 → 턴 종료로 이어지는 “턴제 보드게임” 기본 파이프라인이 구성되어 있습니다.
- 플레이어 머니 증감/정산/파산 체크가 `ServerIngamePlayer` 중심으로 동작해,
  보드게임에서 흔한 “누적 계산 오류”를 줄이도록 설계되어 있습니다.

<br>

<a name="map-tile"></a>
### 3) 맵/타일 데이터 및 생성 (데이터 기반)
- 타일 데이터: `TileInfoData(ScriptableObject)`
- 보드 생성/배치(마스터): `MapManager` (PhotonNetwork.Instantiate + SetTileData)
- 타일 상태/소유/가격/통행료: `TileController` (partial + RPC SetOwner)

강화 포인트
- 맵은 “코드에 하드코딩된 타일 나열”이 아니라, 데이터 배열(`_tiledates`)을 바탕으로 생성됩니다.
- `SubTileType`에 따라 프리팹을 활성화하는 방식이라, 타일 종류가 늘어나도 유지보수가 쉽습니다.

<br>

<a name="real-estate"></a>
### 4) 부동산(땅/건물) 구매 및 자산/통행료 계산 (가장 집중한 파트)
관련 스크립트
- 구매 UI/거래 UX: `TileBuyUI`, `TileSeaBuyUI`
- 타일 소유권/가격/통행료/시각 반영: `TileController`
- 플레이어 자산/정산: `ServerIngamePlayer`

구현 상세(강화)
- **단계별 소유/가격/통행료 모델**
  - `TileController`는 건물 단계를 인덱스로 관리합니다.  
    `GetPrice(index)`, `GetOwner(index)`, `GetTollPrice(index)`로 동일한 방식으로 접근합니다.
  - 단계가 올라갈수록 타일 가치/통행료가 자연스럽게 누적되도록 합계 계산 함수가 준비되어 있습니다.  
    (`TotalBuyPrice`, `TotalTollPrice`)

- **구매 UX: 프리뷰(선택) 단계와 확정(커밋) 단계 분리**
  - `TileBuyUI`에서 건물 버튼(땅/펜션/콘도/호텔)을 선택하면,
    플레이어 소지금 기준으로 즉시 가능 여부를 판단하고 체크 상태/색상/UI 합계를 갱신합니다.
  - 확정 시 `BuyButtonClick()`에서 선택된 슬롯만 `SetOwner` RPC로 커밋하여
    “네트워크 소유권 동기화”까지 한 번에 완료됩니다.
  - 취소 흐름이 존재하여, 실수로 선택한 거래를 되돌리는 UX도 제공합니다.

- **소유권 동기화와 화면 반영을 한 함수로 묶음**
  - `TileController.SetOwner(index, owner)`는 값만 바꾸지 않고,
    머티리얼/오브젝트 활성화까지 연쇄 적용해 멀티 환경에서 시각 불일치를 줄입니다.

- **플레이어 총 자산(현금 + 부동산 가치) 산출**
  - `ServerIngamePlayer.TotalMoney()`에서 보드 전체 타일을 순회하며
    본인 소유 단계(0~3)의 가격을 합산해 총 자산을 계산합니다.
  - 보드게임 특성상 “통행료/파산” 판단의 기준이 되는 값이므로,
    자산 산출 루틴을 분리해 재사용 가능하게 구성한 점이 포인트입니다.

<br>

<a name="ui-system"></a>
### 5) 타일 클릭 UI / 구매 UI / 이벤트 UI (입력 이벤트 기반)
- 타일 클릭/정보 UI: `TileClick`, `TileClickGroundUI`, `TileClickSeaUI`, `PropertyPanel`, `AreaUI`
- UI 이벤트 허브: `UIManagerP`
- 구매 UI: `TileBuyUI`, `TileSeaBuyUI`
- 임시/예외 처리: `UnFinishedTileClick`

강화 포인트
- 타일 클릭(정보 열람)과 구매(거래)를 분리해서 UI 복잡도를 낮췄습니다.
- `UIManagerP`의 이벤트(클릭 변경/구매 변경/주사위 이벤트 등)를 통해
  UI 갱신이 “직접 참조 난사”로 커지지 않도록 구성되어 있습니다.

<br>

<a name="special"></a>
### 6) 특수 타일/이벤트
- 올림픽: `Olympic` (통행료 2배 처리 포함)
- 무인도: `IslandUI`
- 보너스 카드: `BonusCardManager`, `BonusCardUI`

<br>

<a name="player-result"></a>
### 7) 결과/게임 종료
- 게임오버/결과 UI: `GameOverUIScript`, `GameOverResultWindow`, `PlayerResult`
- 플레이어 UI 관리: `PlayerUIManager`, `PlayerUIController`
- 인게임 UI/서버 UI: `InGameUI`, `ServerUI`
- 파산/생존자 체크 흐름: `ServerIngamePlayer` 내 파산 관련 RPC 및 집계 로직

<br>

<a name="firebase"></a>
### 8) Firebase 로그인/유저데이터
- Auth 로그인/회원가입/닉네임 플로우: `FirebaseLoginMgr`
- Realtime Database 저장/로드(제네릭): `FirebaseDataMgr`

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
