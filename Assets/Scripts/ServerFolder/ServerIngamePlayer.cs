using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;
using System.ComponentModel;
using System;
using UnityEngine.SocialPlatforms;

/// <summary>
/// 게임 내 플레이어 상태 및 행동을 관리하는 클래스
/// </summary>
public class ServerIngamePlayer : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    #region 플레이어 기본 정보
    [Header("플레이어 기본 정보")]
    public int _playerNum;          // 플레이어 고유 번호
    public string _playerNickName;  // 닉네임 데이터
    public int _ranking;            // 플레이어 순위
    public Coroutine _moveCor;            // 플레이어 순위

    [Header("플레이어 자산 관련")]
    public double _money;         // 현재 현금 보유액
    public double _totalMoney;           // 총 자산 (현금 + 소유 부동산 자산 포함)

    public bool _isTravel; // 세계여행 중인지
    public int _travelClickTileNum; // 세계여행 중 클릭 타일 번호
    public bool _isTravelClickTile; // 세계여행 중 클릭 했는지
    public int _travelMoveNum; // 세계여행 몇 칸 이동할찌
    public bool _waitTravelTurn; // 세계여행 이동 가능한 턴 왔는지

    [Header("플레이어 상태")]
    bool _isLoan;                        // 대출 여부
    bool _isInstantiate;                        // 대출 여부
    bool _isTurn = true;                 // 현재 턴 여부
    bool _isCoolFinish = false;          // 주사위 쿨타임 완료 여부
    Coroutine runningCoroutine;          // 주사위 쿨타임 코루틴 참조
    Coroutine runningCoroutine2;          // 주사위 쿨타임 코루틴 참조
    float second = 5f;                   // 기본 쿨타임 시간
    public float currentDiceCooldown1 = 5f; // 주사위 쿨타임 (슬라이더용)
    [SerializeField] private float currentDiceCooldown2 = 5f; // UI 팝업창 쿨타임

    [Header("맵 및 위치 정보")]
    int _mapTurn;                        // 맵을 한 바퀴 돈 횟수
    int _playerPosIndex = 0;             // 현재 위치 인덱스
    Coroutine _playerMoveCor;            // 이동 중인 코루틴 참조

    [Header("소유 타일 정보")]
    public List<TileController> _ownedSeaTiles = new List<TileController>(); // 관광지

    int[] _playerOwnerTileViewArr;

    public int _SeaBuyCount = 0; // 관광지 보유 수

    [Header("Photon 관련")]
    public PhotonView _view;             // PhotonView 참조
    public static Dictionary<int, ServerIngamePlayer> _players = new Dictionary<int, ServerIngamePlayer>(); // 전체 플레이어 관리 딕셔너리

    [Header("게임 매니저 참조")]
    MapManager _mapInfo;                 // 맵 정보 참조
    TurnBasedManager _turnBasedManager; // 턴 관리 매니저
    PlayerManager _playerManager;       // 플레이어 매니저

    [Header("UI 관련")]
    Slider mySlider;                     // 주사위 쿨타임 슬라이더
    GameObject mySliderobj;             // 슬라이더 오브젝트
    Slider mySlider2;                     // 주사위 쿨타임 슬라이더
    GameObject mySliderobj2;             // 슬라이더 오브젝트

    double _tempTotalMoney;
    [Header("시작 돈")] [SerializeField] double _startMoney = 1000;

    //[Header("올림픽 관련")]
    public static event Action<int, int> OnPlayerPositionChanged;
    public static event Action<bool> OlympicCheck;  // 플레이어가 올림픽을 개최했으면 중복 안되게 해줄 이벤트
    #endregion

    #region Start문, Update문
    IEnumerator Start()
    {
        //여기에 돈 쓸거면 플레이어프리팹 안에 있는게 편함
        //나중에  생각하면 싱글톤도 생각해봐야할듯
        _waitTravelTurn = false;
        _money = _startMoney;
        _totalMoney = _money;
        _view = GetComponent<PhotonView>();
        _mapInfo = FindObjectOfType<MapManager>();
        _turnBasedManager = FindObjectOfType<TurnBasedManager>();
        _playerPosIndex = 0;

        mySliderobj = GameObject.Find("CoolTimeGameObject");
        mySlider = GameObject.Find("CoolTimeGameObject").transform.GetChild(0).GetComponent<Slider>();
        mySlider2 = GameObject.Find("CoolTimeGameObject").transform.GetChild(1).GetComponent<Slider>();

        yield return new WaitUntil(() => _isInstantiate == true);
        _players[_playerNum] = this;
    }

    void Update()
    {
        if (!TurnMgr.isGameStarted) return;

        //내턴 and (쿨타임 or 주사위)
        if (PhotonNetwork.LocalPlayer.ActorNumber == TurnMgr.CurrentTurn)
        {
            //내턴 될때 쿹타임 10초 
            if (runningCoroutine == null && _isCoolFinish == false)
            {
                //TODO: 테스트용 주석
                photonView.RPC("Dicecooltime", RpcTarget.All);
            }

            if ((Input.GetKeyDown(KeyCode.Space) || _isCoolFinish == true) && _isTravel == false)
            {
                if (photonView.IsMine && _isTurn == true)
                {
                    
                    photonView.RPC("HideSlider", RpcTarget.All);
                    _isTurn = false;
                    //int _diceNum = _turnBasedManager.Dice();
                    //photonView.RPC("RpcMovePlayer", RpcTarget.All, _diceNum);
                }
            }
            if (_isTravel == true) // 여행 상태
            {
                if (_view.IsMine == true && _isTurn == true)
                {
                    _waitTravelTurn = true;
                    if (_waitTravelTurn == true) // 이동 가능?
                    {
                        UIManagerP.instance.OnTravelUI();
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
        }

        //주사위 중복 방지 
        if (PhotonNetwork.LocalPlayer.ActorNumber != TurnMgr.CurrentTurn)
        {
            _isTurn = true;
            _isCoolFinish = false;
            currentDiceCooldown1 = second;
            currentDiceCooldown2 = second;
        }
    }
    #endregion

    // 리스트에 추가, 중복체크
    //[PunRPC]
    //public void AddPlayerOwnerTileList(TileController _currentTile)
    //{
    //    if (_playerOwnerTileList.Contains(_currentTile) == false)
    //    {
    //        _playerOwnerTileList.Add(_currentTile);
    //    }
    //}
    public List<TileController> _playerOwnerTileList = new List<TileController>(); // 내 소유의 모든 타일 저장
    public List<int> _playerOwnerTileViewList = new List<int>();

    // 플레이어 소유 타일에 새 타일을 추가하는 RPC
    [PunRPC]
    public void AddPlayerOwnerTileList(int _tileViewNum)
    {
        // 중복이 아닐 경우에만 추가
        if (_playerOwnerTileViewList.Contains(_tileViewNum) == false)
        {
            Debug.Log(_playerNum + " 에게 땅 추가" + (_tileViewNum));
            _playerOwnerTileViewList.Add(_tileViewNum); // 뷰 ID 저장
        }
    }

    // 플레이어 소유 타일에서 타일을 제거하는 RPC
    [PunRPC]
    public void MinusPlayerOwnerTileList(int _tileViewNum)
    {
        if (_playerOwnerTileViewList.Contains(_tileViewNum) == true)
        {
            Debug.Log(_playerNum + " 의 땅 없애" + (_tileViewNum));
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
        Debug.Log("부족한 금액 : " + _SaleAmount);

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

        Debug.Log("_TotalMyLandPrice : " + _TotalMyLandPrice);

        // 5. 소유 리스트 재갱신
        TileControllerListRecorder(_playerOwnerTileViewList.ToArray());

        // 6. 매각 금액만큼 내 돈 증가 + 통행료만큼 내 돈 차감
        IncreaseMoney(_TotalMyLandPrice);
        DecreaseMoney(currentTileTollPrice);

        // 7. 매각 가능한 자산이 부족했을 경우 → 파산 처리
        if (_tempTotalMoney < _SaleAmount)
        {
            Debug.Log("파산");
            TurnMgr.Instance.StopTurn(_playerNum, true); // 플레이어 턴 정지
            TurnMgr.Instance.endTurn(); // 턴 넘김
            FindPlayer(_tileOwner).IncreaseMoney(_tempTotalMoney); // 타일 주인에게 내가 가진 전 재산 지급
        }
        else
        {
            Debug.Log("파산 아닐 때");
            FindPlayer(_tileOwner).IncreaseMoney(currentTileTollPrice); // 정상적으로 통행료 지급
        }

        TotalMoney();
    }

    //게임 오버를 위한 조건 (나를 제외한 모든 플레이어가 파산했나?)
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
        }
        else
        {
            Debug.Log("아직 게임 중!");
            Debug.Log(aliveCount);
        }
    }

    [PunRPC]
    void HideSlider()
    {
        mySlider.gameObject.SetActive(false);
    }


    //각턴 위치값 변경, 주사위 쿨타임
    [PunRPC]
    void Dicecooltime()
    {
        //함수 너무 많아지는거 같아서 안에 넣어둠
        //임시 위치값 일일이 넣음
        if (TurnMgr.currentTurn == 1)
        {
            mySliderobj.transform.localPosition = new Vector3(-720, 269, 0);
        }
        else if (TurnMgr.currentTurn == 2)
        {
            mySliderobj.transform.localPosition = new Vector3(720, 269, 0);
        }
        else if (TurnMgr.currentTurn == 3)
        {
            mySliderobj.transform.localPosition = new Vector3(695, -269, 0);
        }
        else if (TurnMgr.currentTurn == 4)
        {
            mySliderobj.transform.localPosition = new Vector3(687, -287, 0);
        }
        runningCoroutine = StartCoroutine(Dicecooltimedelay(second));
    }

    [PunRPC]
    void StopCooldownSlider1()
    {
        StopCoroutine(runningCoroutine);
        runningCoroutine = null;
    }
    [PunRPC]
    void StopCooldownSlider2()
    {
        StopCoroutine(cooltimedelay(second));
    }



    #region 주사위 쿨타임
    //주사위 쿨타임
    IEnumerator Dicecooltimedelay(float Second)
    {
        mySlider.gameObject.SetActive(true);
        double startTime = PhotonNetwork.Time;
        double targetTime = startTime + Second;
        mySlider.maxValue = Second;

        currentDiceCooldown1 = Second;
        mySlider.value = Second;

        while (PhotonNetwork.Time < targetTime )
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                photonView.RPC("StopCooldownSlider1", RpcTarget.All);
                break;
            }
            currentDiceCooldown1 = (float)(targetTime - PhotonNetwork.Time);
            mySlider.value = currentDiceCooldown1;
            yield return null;
        }

        _isCoolFinish = true;
        currentDiceCooldown1 = Second;
        mySlider.gameObject.SetActive(false);

        runningCoroutine = null;
    }

    [PunRPC]
    void Slider2Value(float cooldown)
    {
        currentDiceCooldown2 = cooldown;
        mySlider2.value = currentDiceCooldown2;
    }
    [PunRPC]
    void SetSliderActive(bool isActive)
    {
        mySlider2.gameObject.SetActive(isActive);
    }



    //팝업창 쿨타임(구매, 취소등)
    IEnumerator cooltimedelay(float Second)
    {
        photonView.RPC("SetSliderActive", RpcTarget.All, true); 
        double startTime = PhotonNetwork.Time;
        double targetTime = startTime + Second;

        currentDiceCooldown2 = Second;

        while (PhotonNetwork.Time < targetTime)
        {
            if (Input.GetKeyDown(KeyCode.P)) //구매,취소 했을경우 정지 ***** 변수하나 넣어서 아래에 다시 바꾸면댐
            {
                photonView.RPC("StopCooldownSlider2", RpcTarget.All);
                break;
            }
            currentDiceCooldown2 = (float)(targetTime - PhotonNetwork.Time);
            photonView.RPC("Slider2Value", RpcTarget.All, (float)(targetTime - PhotonNetwork.Time));
            yield return null;
        }
        currentDiceCooldown2 = Second;
        photonView.RPC("SetSliderActive", RpcTarget.All, false); 
        
        TurnMgr.Instance.endTurn();//*** 중복되서 2개 턴날라감
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
        }
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

    /// <summary>
    /// 주사위 값(num)만큼 플레이어를 한 칸씩 이동시킨다
    /// </summary>
    IEnumerator MovePlayer(int num)
    {
        int count = 0;

        while (count < num)
        {
            if ((_playerPosIndex + count) >= 39)
            {
                _playerPosIndex -= 40;
            }

            // 도착 지점 처리 (필요 시 추가)
            else if (_playerPosIndex + count == 0)
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
                    Debug.Log($"{_playerNum} 통행료 빠져 나간 돈 : " + currentTileTollPrice);

                    _view.RPC("DecreaseMoney", RpcTarget.All, currentTileTollPrice);

                    // 토지주인의 돈 증가 함수 실행
                    FindPlayer(currentTile.GetOwner(0))._view.RPC("IncreaseMoney", RpcTarget.All, currentTileTollPrice);
                    Debug.Log($"{currentTile.GetOwner(0)}의 통행료 증가 된 돈 : " + currentTileTollPrice);

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
                        UIManagerP.instance.OnFactorWarningUI();
                    }
                }
            }
        }

        ALLPlayerBankruptcy();
        _playerMoveCor = null; // 코루틴이 끝났으므로 null로 초기화
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

        while (true)
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
    }

    [PunRPC]
    public void DecreaseMoney(double money)
    {
        _money -= money;

        if (_money < 0)
        {
            _money = 0;
            Debug.Log("DecreaseMoney에서 실행됨: 파산됨.");
        }
        TotalMoney();
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

    private void OnEnable()
    {
        Olympic.AlreadyCheck += IsSelect;
    }
    private void OnDisable()
    {
        Olympic.AlreadyCheck -= IsSelect;
    }
}
