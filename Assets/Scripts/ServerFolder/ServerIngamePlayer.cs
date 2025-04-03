using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;
using System.ComponentModel;
using System;
using UnityEngine.SocialPlatforms;
using JetBrains.Annotations;

// 옵저버 인터페이스: 플레이어 정보가 바뀌면 이걸 통해 UI 등에 알림
public interface IPlayerDataObserver
{
    void OnPlayerDataChanged(int actorNumber);
}

public class ServerIngamePlayer : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    #region 플레이어 기본 정보
    // ========================= 플레이어 기본 정보 =========================
    [Header("플레이어 기본 정보")]
    public int _playerNum;          // 플레이어 고유 번호
    public string _playerNickName;  // 닉네임 데이터
    public int _ranking;            // 플레이어 순위
    public bool _isBtnClicked;            // 버튼 클릭했는지
    public Coroutine _moveCor;            // 플레이어 순위

    // ========================= 자산 관련 정보 =========================
    [Header("플레이어 자산 관련")]
    public double _money;                      // 보유 현금
    public double _totalMoney;                 // 총 자산 (현금 + 건물 자산)

    // ========================= 세계여행 관련 상태 =========================
    public bool _isTravel;                     // 세계여행 중인지
    public int _travelClickTileNum;            // 클릭한 타일 번호
    public bool _isTravelClickTile;            // 타일 클릭 여부
    public int _travelMoveNum;                 // 이동할 칸 수
    public bool _waitTravelTurn;               // 이동 가능 턴 여부
    public bool _isBankruptcy;               // 이동 가능 턴 여부

    // ========================= 플레이어 상태 정보 =========================
    [Header("플레이어 상태")]
    bool _isLoan;                              // 대출 여부
    bool _isInstantiate;                       // 인스턴스화 여부
    bool _isTurn = true;                       // 현재 턴 여부
    bool _isCoolFinish = false;                // 주사위 쿨타임 완료 여부

    Coroutine runningCoroutine;                // 주사위 쿨타임 코루틴
    Coroutine runningCoroutine2;               // 팝업 쿨타임 코루틴
    float second = 5f;                         // 기본 쿨타임 시간
    public float currentDiceCooldown1 = 5f;    // 쿨타임 (슬라이더1)
    [SerializeField] private float currentDiceCooldown2 = 5f; // 쿨타임 (슬라이더2)
    public bool _isSecondCoolTimeG = false;

    // ========================= 맵 및 위치 정보 =========================
    [Header("맵 및 위치 정보")]
    int _mapTurn;                              // 맵 순환 횟수
    int _playerPosIndex = 0;                   // 현재 타일 인덱스
    Coroutine _playerMoveCor;                  // 이동 코루틴 참조

    // ========================= 타일 소유 정보 =========================
    [Header("소유 타일 정보")]
    public List<TileController> _ownedSeaTiles = new List<TileController>(); // 소유 관광지
    public int _SeaBuyCount = 0;               // 관광지 보유 수

    public List<TileController> _playerOwnerTileList = new List<TileController>(); // 소유한 일반 타일들
    public List<int> _playerOwnerTileViewList = new List<int>();                   // 해당 타일들의 ViewID

    // ========================= 네트워크 관련 =========================
    [Header("Photon 관련")]
    public PhotonView _view;                   // PhotonView 참조
    public static Dictionary<int, ServerIngamePlayer> _players = new Dictionary<int, ServerIngamePlayer>(); // 전체 플레이어 목록

    // ========================= 매니저 참조 =========================
    [Header("게임 매니저 참조")]
    MapManager _mapInfo;                       // 맵 매니저
    TurnBasedManager _turnBasedManager;       // 턴 매니저

    // ========================= UI 요소 =========================
    [Header("UI 관련")]
    Slider mySlider;                           // 쿨타임 슬라이더 1
    GameObject mySliderobj;                    // 슬라이더 1 오브젝트
    Slider mySlider2;                          // 쿨타임 슬라이더 2

    double _tempTotalMoney;                   // 임시 자산 저장용

    [Header("시작 돈")]
    public double _startMoney = 1000; // 초기 자금

    // ========================= 이벤트 관련 =========================
    // 플레이어 위치 변경시 호출됨 (플레이어 번호, 위치 인덱스)
    public static event Action<int, int> OnPlayerPositionChanged;

    // 올림픽 개최 중복 방지 이벤트
    public static event Action<bool> OlympicCheck;

    // ========================= 옵저버 관련 =========================
    private static List<IPlayerDataObserver> _observers = new List<IPlayerDataObserver>();

    // ========================= 무인도 관련 =========================
    public bool _isInIsland = false;       // 무인도에 있는지 여부
    public int _islandSkipCount = 0;       // 스킵할 턴 수
    public bool _willEscapeIsland = false; // 다음 턴에 탈출 여부
    #endregion

    #region Start문, Update문
    IEnumerator Start()
    {
        _isBtnClicked = false;
        _waitTravelTurn = false;
        _money = _startMoney;
        _totalMoney = _money;
        _view = GetComponent<PhotonView>();
        _mapInfo = FindObjectOfType<MapManager>();
        _turnBasedManager = FindObjectOfType<TurnBasedManager>();
        _playerPosIndex = 0;

        mySliderobj = GameObject.Find("CoolTimeGameObject");
        mySlider = mySliderobj.transform.GetChild(0).GetComponent<Slider>();
        mySlider2 = mySliderobj.transform.GetChild(1).GetComponent<Slider>();

        yield return new WaitUntil(() => _isInstantiate == true);
        _players[_playerNum] = this;
        NotifyPlayerDataChanged(_playerNum);
    }

    void Update()
    {
        if (!TurnMgr.isGameStarted) return;

        // 턴인데 무인도에 있는 상태라면
        if (_isInIsland && PhotonNetwork.LocalPlayer.ActorNumber == TurnMgr.CurrentTurn)
        {
            if (photonView.IsMine)
            {
                TurnMgr.Instance.endTurn();
                _islandSkipCount--;

                if (_islandSkipCount <= 0)
                {
                    _isInIsland = false;
                }
            }

            return; // 이후 쿨타임 및 주사위 로직 전부 생략
        }

        //내턴 and (쿨타임 or 주사위)
        if (PhotonNetwork.LocalPlayer.ActorNumber == TurnMgr.CurrentTurn)
        {
            //내턴 될때 쿹타임 10초 
            if (runningCoroutine == null && _isCoolFinish == false && photonView.IsMine)
            {
                photonView.RPC("Dicecooltime", RpcTarget.All);
            }

            if ((_isBtnClicked == true || _isCoolFinish == true) && _isTravel == false)
            {
                
                if (photonView.IsMine && _isTurn == true)
                {
                    photonView.RPC("HideSlider", RpcTarget.All);
                    _isTurn = false;
                    _turnBasedManager.Dice();
                }
            }

            if (_isTravel == true) // 여행 상태
            {
                _waitTravelTurn = true;
                if (_waitTravelTurn == true) // 이동 가능?
                {
                    UIManagerP.instance.OnTravelUI();
                    UIManagerP.instance.OffDiceUI();
                    _waitTravelTurn = false;
                }
                if (_isTravelClickTile == true) // 타일 클릭 했는지
                {
                    UIManagerP.instance.OffTravelUI();
                    if ((_travelClickTileNum - 30) > 0) // 30 = 세계여행 위치
                    {
                        _travelMoveNum = _travelClickTileNum - 30;
                    }
                    else
                    {
                        _travelMoveNum = (_travelClickTileNum - 30) + 40;
                    }
                    photonView.RPC("RpcMovePlayer", RpcTarget.All, _travelMoveNum);
                    _isTravel = false;
                    _isTravelClickTile = false;
                }
            }
        }

        //내 턴이 아닐 경우
        if (PhotonNetwork.LocalPlayer.ActorNumber != TurnMgr.CurrentTurn)
        {
            _isTurn = true;
            _isCoolFinish = false;

            // 쿨타임이 진행 중이 아닐 때만 초기화 및 꺼짐 처리
            if (!_isSecondCoolTimeG)
            {
                _isSecondCoolTimeG = false;
                currentDiceCooldown2 = second;
            }

            currentDiceCooldown1 = second;
        }
    }
    #endregion

    // 플레이어 소유 타일에 새 타일을 추가하는 RPC
    [PunRPC]
    public void AddPlayerOwnerTileList(int _tileViewNum)
    {
        // 중복이 아닐 경우에만 추가
        if (_playerOwnerTileViewList.Contains(_tileViewNum) == false)
        {
            _playerOwnerTileViewList.Add(_tileViewNum); // 뷰 ID 저장
        }
    }

    // 플레이어 소유 타일에서 타일을 제거하는 RPC
    [PunRPC]
    public void MinusPlayerOwnerTileList(int _tileViewNum)
    {
        if (_playerOwnerTileViewList.Contains(_tileViewNum) == true)
        {
            _playerOwnerTileViewList.Remove(_tileViewNum); // 뷰 ID 제거
        }
    }

    // ViewID 배열을 기반으로 실제 타일 객체들을 리스트에 저장
    public void TileControllerListRecorder(int[] _playerOwnerTileViewArr)
    {
        _playerOwnerTileList.Clear(); // 기존 리스트 초기화

        for (int i = 0; i < _playerOwnerTileViewArr.Length; i++)
        {
            PhotonView _tileViewId = PhotonView.Find(_playerOwnerTileViewArr[i]); // ViewID -> PhotonView 찾기
            TileController _currentTileController = _tileViewId.GetComponent<TileController>(); // 실제 TileController 컴포넌트 얻기
            _playerOwnerTileList.Add(_currentTileController); // 리스트에 추가
        }
    }

    /// <summary>
    /// 타일 ViewID 배열을 받아 소유 타일 리스트를 갱신하고,
    /// 가격이 낮은 순으로 정렬한다 (버블 정렬)
    /// </summary>
    public void LowPriceSorting(int[] _playerOwnerTileViewArr)
    {
        TileControllerListRecorder(_playerOwnerTileViewList.ToArray());
        // 버블 정렬 (낮은 가격 순)
        for (int i = 0; i < _playerOwnerTileList.Count() - 1; i++)
        {
            for (int j = 0; j < _playerOwnerTileList.Count() - i - 1; j++)
            {
                if (_playerOwnerTileList[i].TotalBuyPrice(_playerOwnerTileList[i]) > _playerOwnerTileList[i + 1].TotalBuyPrice(_playerOwnerTileList[i + 1]))
                {
                    var _temp = _playerOwnerTileList[i];
                    _playerOwnerTileList[i] = _playerOwnerTileList[i + 1];
                    _playerOwnerTileList[i + 1] = _temp;
                }
            }
        }
    }

    /// <summary>
    /// 자산이 부족할 경우, 플레이어가 소유한 땅을 자동으로 매각하여 부족한 금액을 충당한다
    /// </summary>
    /// <param name="_SaleAmount">부족한 금액 (지불해야 하는 통행료 - 현재 보유 현금)</param>
    /// <param name="currentTileTollPrice">현재 타일의 통행료 (최종 지불 금액)</param>
    /// <param name="_tileOwner">현재 타일의 주인 (통행료를 받을 사람)</param>
    [PunRPC]
    public void AutomaticSale(double _SaleAmount, double currentTileTollPrice, int _tileOwner)
    {
        // 1. 현재 소유 타일들을 가격 기준으로 정렬 (저가순 → 고가순)
        LowPriceSorting(_playerOwnerTileViewList.ToArray());

        _tempTotalMoney = _totalMoney;
        double _TotalMyLandPrice = 0;
        int i = 0;

        // 2. 필요한 돈이 확보될 때까지 타일 가격을 누적하며 매각할 개수 계산
        for (; i < _playerOwnerTileList.Count; i++)
        {
            if (_SaleAmount <= _TotalMyLandPrice)
            {
                break; // 누적 금액이 부족한 금액을 넘었으면 탈출
            }
            else
            {
                _TotalMyLandPrice += _playerOwnerTileList[i].TotalBuyPrice(_playerOwnerTileList[i]);
            }
        }

        // 3. 실제 매각 처리: 소유자 → 은행(0)으로 변경
        for (int j = 0; j < i; j++)
        {
            for (int k = 0; k < 4; k++)
            {
                if (_playerOwnerTileList[j].GetOwner(k) == _playerNum)
                {
                    _playerOwnerTileList[j].SetOwner(k, 0);
                }
            }
        }

        // 4. 소유 리스트에서 매각한 타일 제거
        for (int h = 0; h < i; h++)
        {
            var viewID = _playerOwnerTileList[h].photonView.ViewID;
            _playerOwnerTileViewList.Remove(viewID);
        }

        // 5. 소유 리스트 재갱신
        TileControllerListRecorder(_playerOwnerTileViewList.ToArray());

        // 6. 매각 금액만큼 내 돈 증가 + 통행료만큼 내 돈 차감
        IncreaseMoney(_TotalMyLandPrice);
        DecreaseMoney(currentTileTollPrice);

        // 7. 매각 가능한 자산이 부족했을 경우 → 파산 처리
        if (_tempTotalMoney < _SaleAmount)
        {
            _isBankruptcy = true;
            TurnMgr.Instance.StopTurn(_playerNum, true); // 플레이어 턴 정지
            TurnMgr.Instance.endTurn(); // 턴 넘김
            FindPlayer(_tileOwner).IncreaseMoney(_tempTotalMoney); // 타일 주인에게 내가 가진 전 재산 지급
        }
        else
        {
            _isBankruptcy = false;
            FindPlayer(_tileOwner).IncreaseMoney(currentTileTollPrice); // 정상적으로 통행료 지급
        }

        TotalMoney();
    }

    [PunRPC]
    public void ALLPlayerBankruptcy()
    {
        // 살아있는 플레이어 수 체크
        int aliveCount = 0;
        ServerIngamePlayer lastAlivePlayer = null;

        foreach (var playerData in ServerIngamePlayer._players.Values)
        {
            if (playerData._totalMoney > 0)
            {
                aliveCount++;
                lastAlivePlayer = playerData; // 마지막으로 살아남은 사람 저장
            }
        }

        // 결과 판단
        if (aliveCount == 1)
        {
            GameOverResultWindow gameoverwindow = FindObjectOfType<GameOverResultWindow>();
            gameoverwindow.CreateResultUIs(_startMoney);
            UIManagerP.instance.OffBuyUIPanel();
            UIManagerP.instance.OffClickUI();
            UIManagerP.instance.OffDiceUI();
            UIManagerP.instance.OffFactorUI();
            UIManagerP.instance.OffFactorWarningUI();
            UIManagerP.instance.OffTravelUI();
            Time.timeScale = 0f;
        }
    }

    #region 주사위 및 팝업 쿨타임 관련 함수
    // 1. 주사위 쿨타임 시작
    // 현재 턴 기준으로 슬라이더1 UI의 위치를 옮기고, 주사위 쿨타임 코루틴 시작
    [PunRPC]
    void Dicecooltime()
    {
        if (TurnMgr.currentTurn == 1)
            mySliderobj.transform.localPosition = new Vector3(-720, 310, 0);
        else if (TurnMgr.currentTurn == 2)
            mySliderobj.transform.localPosition = new Vector3(720, 310, 0);
        else if (TurnMgr.currentTurn == 3)
            mySliderobj.transform.localPosition = new Vector3(720, -310, 0);
        else if (TurnMgr.currentTurn == 4)
            mySliderobj.transform.localPosition = new Vector3(-720, -310, 0);

        runningCoroutine = StartCoroutine(Dicecooltimedelay(second));
    }

    // 2. 주사위 쿨타임 진행
    // 슬라이더1을 점점 줄여가며 쿨타임을 시각적으로 표현하고, 완료되면 _isCoolFinish 설정
    IEnumerator Dicecooltimedelay(float Second)
    {
        mySlider.gameObject.SetActive(true); // 슬라이더 UI 보이기

        double startTime = PhotonNetwork.Time;
        double targetTime = startTime + Second;

        mySlider.maxValue = Second;
        currentDiceCooldown1 = Second;
        mySlider.value = Second;

        while (!TurnMgr._isGameOver && PhotonNetwork.Time < targetTime)
        {
            if (_isBtnClicked == true) // 사용자가 주사위 클릭한 경우
            {
                photonView.RPC("StopCooldownSlider1", RpcTarget.All);
                break;
            }

            currentDiceCooldown1 = (float)(targetTime - PhotonNetwork.Time);
            mySlider.value = currentDiceCooldown1;
            yield return null;
        }

        _isCoolFinish = true; // 쿨타임 완료
        currentDiceCooldown1 = Second;
        mySlider.gameObject.SetActive(false);
        runningCoroutine = null;
    }

    // 3. 주사위 쿨타임 중단
    // 사용자가 주사위를 누른 경우 슬라이더1을 강제로 중단하고 숨김 처리
    [PunRPC]
    void StopCooldownSlider1()
    {
        _isBtnClicked = false;
        StopCoroutine(runningCoroutine);
        runningCoroutine = null;
    }

    // 4. 팝업 쿨타임 시작
    // 건물 구매 등 UI 팝업 시 슬라이더2가 시작됨. 시간이 다 되면 자동으로 턴 종료
    IEnumerator cooltimedelay(float Second)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != TurnMgr.CurrentTurn)
        {
            yield break;
        }

        _isSecondCoolTimeG = false; // 초기화
        photonView.RPC("SetSliderActive", RpcTarget.All, true); // 슬라이더2 UI 보이기

        double startTime = PhotonNetwork.Time;
        double targetTime = startTime + Second;
        currentDiceCooldown2 = Second;

        while (!TurnMgr._isGameOver && PhotonNetwork.Time < targetTime)
        {
            if (_isSecondCoolTimeG) // 유저가 중간에 행동한 경우
            {
                photonView.RPC("SetSliderActive", RpcTarget.All, false);
                yield break;
            }

            float remainingTime = (float)(targetTime - PhotonNetwork.Time);
            currentDiceCooldown2 = remainingTime;

            photonView.RPC("Slider2Value", RpcTarget.All, remainingTime, _playerNum); // 슬라이더 값 전파
            yield return null;
        }

        // 쿨타임 완료 시 턴 넘김
        _isSecondCoolTimeG = false;
        currentDiceCooldown2 = second;

        photonView.RPC("Slider2Value", RpcTarget.All, second, _playerNum);
        photonView.RPC("SetSliderActive", RpcTarget.All, false);

        if (PhotonNetwork.LocalPlayer.ActorNumber == TurnMgr.CurrentTurn)
        {
            TurnMgr.Instance.endTurn();
        }
    }

    // 5. 팝업 쿨타임 중단
    // 유저가 구매 or 취소 버튼을 눌렀을 때 슬라이더2를 중단하고 턴을 넘김
    [PunRPC]
    void StopCooldownSlider2()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != TurnMgr.CurrentTurn)
            return;

        _isSecondCoolTimeG = true;

        if (runningCoroutine2 != null)
        {
            StopCoroutine(runningCoroutine2);
            runningCoroutine2 = null;
        }

        mySlider2.gameObject.SetActive(false);
        currentDiceCooldown2 = second;
        mySlider2.value = second;

        TurnMgr.Instance.endTurn(); // 직접 턴 넘김
    }

    // 6. 슬라이더2 동기화
    // 모든 클라이언트에서 동일한 슬라이더2 값을 보이도록 RPC로 동기화
    [PunRPC]
    void Slider2Value(float cooldown, int targetActorNumber)
    {
        if (!ServerIngamePlayer._players.ContainsKey(targetActorNumber)) return;

        ServerIngamePlayer targetPlayer = ServerIngamePlayer._players[targetActorNumber];
        targetPlayer.currentDiceCooldown2 = cooldown;
        targetPlayer.mySlider2.value = cooldown;
    }

    // 7. 슬라이더2 UI 활성화/비활성화
    // 슬라이더2를 보이거나 숨길 때 사용하는 RPC
    [PunRPC]
    void SetSliderActive(bool isActive)
    {
        mySlider2.gameObject.SetActive(isActive);
    }

    // 8. 슬라이더1 숨김 처리
    // 주사위 슬라이더를 강제로 숨길 때 사용됨
    [PunRPC]
    void HideSlider()
    {
        mySlider.gameObject.SetActive(false);
    }
    #endregion

    /// <summary>
    /// 플레이어의 총 자산(현금 + 보유 토지)
    /// </summary>
    [PunRPC]
    public void TotalMoney()
    {
        double tileAssetTotal = 0;

        // 전체 타일 순회
        for (int i = 0; i < _mapInfo._tiles.Length; i++)
        {
            TileController currentTile = _mapInfo._tiles[i].GetComponent<TileController>();

            // 땅(0) ~ 호텔(3)까지 확인
            for (int index = 0; index <= 3; index++)
            {
                // 이 타일의 이 건물이 내 소유인가?
                if (currentTile.GetOwner(index) == _playerNum)
                {
                    tileAssetTotal += currentTile.GetPrice(index);
                }
            }
        }

        _totalMoney = 0;
        _totalMoney = _money + tileAssetTotal;

        if (_totalMoney < 0)
        {
            _totalMoney = 0;

            //TODO: 캐릭터 비활성 및 파산UI 출력
        }

        NotifyPlayerDataChanged(_playerNum);
    }

    /// <summary>
    /// 플레이어가 소유한 Sea 타일을 추가한다
    /// </summary>
    [PunRPC]
    public void AddSeaTile(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view == null) return;

        TileController tile = view.GetComponent<TileController>();
        if (tile != null && tile._tileType == TileType.Sea && !_ownedSeaTiles.Contains(tile))
        {
            _ownedSeaTiles.Add(tile);
        }
    }

    /// <summary>
    /// 현재 소유 중인 Sea 타입 타일을 다시 찾아와 리스트를 갱신한다
    /// </summary>
    public void RefreshOwnedSeaTiles()
    {
        int myActorNumber = photonView.OwnerActorNr;

        _ownedSeaTiles = FindObjectsOfType<TileController>()
            .Where(t => t._tileType == TileType.Sea && t._tileLandOwner == myActorNumber)
            .ToList();
    }

    /// <summary>
    /// 현재 보유 중인 관광지 수를 반환한다
    /// </summary>
    public int GetOwnedSeaTileCount()
    {
        return _ownedSeaTiles.Count;
    }

    [PunRPC]
    public void RpcMovePlayer(int num)
    {
        if (_playerMoveCor == null)
        {
            _playerMoveCor = StartCoroutine(MovePlayer(num));
        }
        else
        {
            StopCoroutine(_playerMoveCor);
        }
    }

    [PunRPC]
    public void EnterIsland()
    {
        _isInIsland = true;
        _islandSkipCount = 3; // 2턴 스킵
    }

    /// <summary>
    /// 주사위 값(num)만큼 플레이어를 한 칸씩 이동시킨다
    /// </summary>
    IEnumerator MovePlayer(int num)
    {
        int count = 0;
        while (!TurnMgr._isGameOver && count < num)
        {
            if ((_playerPosIndex + count) >= 39)
            {
                _playerPosIndex -= 40;
            }
            else if (_playerPosIndex + count == 0) // 도착 지점 처리 (필요 시 추가)
            {
                // StartPointPass();
            }

            count++;
            transform.position = _mapInfo._tiles[_playerPosIndex + count].transform.position;

            yield return new WaitForSeconds(0.1f);
        }

        _playerPosIndex += count;

        TileController currentTile = _mapInfo._tiles[_playerPosIndex].GetComponent<TileController>();

        if (currentTile.GetOwner(0) == 0)
        {
            if (photonView.IsMine)
            {
                if (currentTile._tileType == TileType.Travel)
                {
                    TurnMgr.Instance.endTurn();
                    _isTravel = true;
                    _waitTravelTurn = false;
                }
                else if (currentTile._tileType == TileType.Island)
                {
                    photonView.RPC("EnterIsland", RpcTarget.All); // 무인도 상태로 진입
                    TurnMgr.Instance.endTurn();                   // 바로 턴 종료
                }

                //쿨타임
                StartCoroutine(cooltimedelay(second));
                UIManagerP.instance.OnBuyUI(currentTile._tileType);
                UIManagerP.instance.InvokeBuyUI(currentTile, currentTile._tileType, this);
            }
        }
        // 주인 = 나, 타일 타입 = 그라운드
        else if (currentTile.GetOwner(0) == _playerNum)
        {
            if (currentTile._tileType == TileType.Ground)
            {
                if (photonView.IsMine)
                {
                    //쿨타임
                    StartCoroutine(cooltimedelay(second));
                    UIManagerP.instance.OnBuyUI(currentTile._tileType);
                    UIManagerP.instance.InvokeBuyUI(currentTile, currentTile._tileType, this);
                }
            }
            else
            {
                TurnMgr.Instance.endTurn();
            }
        }
        // 주인 = 다른 사람
        else if (currentTile.GetOwner(0) != _playerNum)
        {
            double currentTileTollPrice = currentTile.TotalTollPrice(currentTile);
            double currentTileBuyPrice = currentTile.TotalBuyPrice(currentTile);
            if (_view.IsMine == true)
            {
                // 통행료 지불 가능 상태라면
                if (_money > currentTileTollPrice)
                {

                    _view.RPC("DecreaseMoney", RpcTarget.All, currentTileTollPrice);

                    // 토지주인의 돈 증가 함수 실행
                    FindPlayer(currentTile.GetOwner(0))._view.RPC("IncreaseMoney", RpcTarget.All, currentTileTollPrice);

                    if (currentTile._tileType == TileType.Ground)
                    {
                        StartCoroutine(cooltimedelay(second));
                        if (_money >= currentTileBuyPrice) // 인수 가능 상태라면
                        {
                            UIManagerP.instance.OnFactorUI(currentTile, FindPlayer(_playerNum), FindPlayer(currentTile.GetOwner(0)));
                        }
                        else
                        {
                            // 인수 불가 Ui 출력
                            UIManagerP.instance.OnFactorWarningUI();
                        }
                    }
                    else
                    {
                        TurnMgr.Instance.endTurn();
                    }
                }
                else // 통행료 지불이 불가한 상태라면
                {
                    int _tileOwner = currentTile.GetOwner(0);
                    _view.RPC("AutomaticSale", RpcTarget.All, currentTileTollPrice - _money, currentTileTollPrice, _tileOwner);
                    _view.RPC("TotalMoney", RpcTarget.All);
                    FindPlayer(currentTile.GetOwner(0))._view.RPC("TotalMoney", RpcTarget.All);
                    if (_money >= currentTileBuyPrice) // 인수 가능 상태라면
                    {
                        UIManagerP.instance.OnFactorUI(currentTile, FindPlayer(_playerNum), FindPlayer(currentTile.GetOwner(0)));
                    }
                    else
                    {
                        // 인수 불가 Ui 출력
                        if (_isBankruptcy == false)
                        {
                            UIManagerP.instance.OnFactorWarningUI();
                        }
                        else
                        {
                            TurnMgr.Instance.endTurn();
                        }
                    }
                }
            }
        }

        _view.RPC("ALLPlayerBankruptcy", RpcTarget.All);

        // 코루틴이 끝났으므로 null로 초기화
        _playerMoveCor = null;
        OnPlayerPositionChanged?.Invoke(_playerNum, _playerPosIndex);
    }

    [PunRPC]
    public void BonusCardMovePlayer()
    {
        StartCoroutine(BCMovePlayer());
    }

    IEnumerator BCMovePlayer()
    {
        int currentIndex = _playerPosIndex;

        while (!TurnMgr._isGameOver && true)
        {
            // 1칸 이동
            currentIndex++;

            if (currentIndex >= _mapInfo._tiles.Length)
            {
                currentIndex = 0; // 보드판 루프
            }

            transform.position = _mapInfo._tiles[currentIndex].transform.position;

            yield return new WaitForSeconds(0.01f);

            // 도착 지점: 인덱스 0
            if (currentIndex == 0)
            {
                break;
            }
        }

        _playerPosIndex = currentIndex;
    }

    public ServerIngamePlayer FindPlayer(int _actorNum)
    {
        return _players[_actorNum];
    }

    //public void StartPointPass()
    //{
    //    _playerManager.IncreaseMoney(1000);
    //}

    [PunRPC]
    public void IncreaseMoney(double money)
    {
        _money += money;
        TotalMoney();

        NotifyPlayerDataChanged(_playerNum);
    }

    [PunRPC]
    public void DecreaseMoney(double money)
    {
        _money -= money;

        if (_money < 0)
        {
            _money = 0;
        }

        TotalMoney();
        NotifyPlayerDataChanged(_playerNum);
    }

    [PunRPC]
    public void SyncMoney(double updatedMoney)
    {
        if (!photonView.IsMine)
        {
            _money = updatedMoney;
        }
    }

    [PunRPC]
    public void MoneyReturn(double money)
    {
        _money = money;
    }

    /// <summary>
    /// 현재 플레이어의 자금 반환
    /// </summary>
    public double GetMoney()
    {
        return _money;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            _playerNum = (int)data[0];
            _playerNickName = (string)data[1];
            _isInstantiate = true;
        }
    }

    public void IsSelect(bool check)
    {
        if (check == false)
        {
            OlympicCheck?.Invoke(false);
        }
    }

    public int PlayerPosIndex
    {
        get => _playerPosIndex;
        set
        {
            if (_playerPosIndex != value) // 값이 변경될 때만 실행
            {
                _playerPosIndex = value;
            }
        }
    }

    public static void RegisterObserver(IPlayerDataObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public static void UnregisterObserver(IPlayerDataObserver observer)
    {
        if (_observers.Contains(observer))
            _observers.Remove(observer);
    }

    public static void NotifyPlayerDataChanged(int actorNumber)
    {
        foreach (var observer in _observers)
        {
            observer.OnPlayerDataChanged(actorNumber);
        }
    }

    private void OnEnable()
    {
        Olympic.AlreadyCheck += IsSelect;
    }
    private void OnDisable()
    {
        Olympic.AlreadyCheck -= IsSelect;
    }
}
