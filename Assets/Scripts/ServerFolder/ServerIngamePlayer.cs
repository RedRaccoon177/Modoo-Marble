using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using UnityEngine.UI;
using Photon.Realtime;
using System;
using System.Linq;
using System.ComponentModel;
using Unity.VisualScripting;

/// <summary>
/// 게임 내 플레이어 상태 및 행동을 관리하는 클래스
/// </summary>
public class ServerIngamePlayer : MonoBehaviourPunCallbacks,IPunInstantiateMagicCallback
{
    #region 플레이어 기본 정보
    [Header("플레이어 기본 정보")]
    public int _playerNum; // 플레이어 고유 번호
    int _playerNickName; // (미사용 중) 닉네임 데이터

    [Header("플레이어 자산 관련")]
    public double _money = 1000;         // 현재 현금 보유액
    public double _totalMoney;           // 총 자산 (현금 + 소유 부동산 자산 포함)

    [Header("플레이어 상태")]
    bool _isLoan;                        // 대출 여부
    bool _isTurn = true;                 // 현재 턴 여부
    bool _isCoolFinish = false;          // 주사위 쿨타임 완료 여부
    Coroutine runningCoroutine;          // 주사위 쿨타임 코루틴 참조
    float second = 5f;                   // 기본 쿨타임 시간
    public float currentDiceCooldown1 = 5f; // 주사위 쿨타임 (슬라이더용)
    [SerializeField] private float currentDiceCooldown2 = 5f; // UI 팝업창 쿨타임

    [Header("맵 및 위치 정보")]
    int _mapTurn;                        // 맵을 한 바퀴 돈 횟수
    int _playerPosIndex = 0;             // 현재 위치 인덱스
    Coroutine _playerMoveCor;            // 이동 중인 코루틴 참조

    [Header("소유 타일 정보")]
    List<TileController> _playerGroundLists = new List<TileController>(); // 일반 땅 리스트
    public List<TileController> _ownedSeaTiles = new List<TileController>(); // 관광지(Sea타일) 리스트
    public int _SeaBuyCount = 0;         // 보유한 관광지 수

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
    #endregion

    #region Start문, Update문
    void Start()
    {
        //여기에 돈 쓸거면 플레이어프리팹 안에 있는게 편함
        //나중에  생각하면 싱글톤도 생각해봐야할듯
        _money = 1000;
        _totalMoney = _money;
        _players[_playerNum] = this; 
        _view = GetComponent<PhotonView>();
        _mapInfo = FindObjectOfType<MapManager>();
        _turnBasedManager = FindObjectOfType<TurnBasedManager>();
        _playerPosIndex = 0;

        mySliderobj = GameObject.Find("CoolTimeGameObject");
        mySlider = GameObject.Find("CoolTimeGameObject").transform.GetChild(0).GetComponent<Slider>();
        //mySlider = GameObject.Find("CoolTimeGameObject").transform.GetChild(PhotonNetwork.LocalPlayer.ActorNumber-1).GetComponent<Slider>();
        //int sdsd = PhotonNetwork.LocalPlayer.ActorNumber * 100;
        //mySlider.transform.position = new Vector3(mySlider.transform.position.x, mySlider.transform.position.y + sdsd, mySlider.transform.position.z);

    }

    void Update()
    {
        //내턴 and 스페이스바 or 쿨타임끝남 
        if (PhotonNetwork.LocalPlayer.ActorNumber == PlayerMoveTest.CurrentTurn)
        {
            ////내턴 될때 쿹타임 10초 
            //if (runningCoroutine == null && _isCoolFinish == false)
            //{
            //    //runningCoroutine = StartCoroutine(Dicecooltimedelay(second));
            //    photonView.RPC("ds", RpcTarget.All);
            //}

            if (Input.GetKeyDown(KeyCode.Space) || _isCoolFinish == true)
            {
                if (photonView.IsMine && _isTurn == true)
                {
                    currentDiceCooldown1 = second;
                    //여기에 기능  //currentDiceCooldown1 주사위굴러가면 초기화 안보이게? 넣으면댐 
                    //ui를 숨기다던지 //처음에 find로 찾아놓고 바꿔야댐 
                    photonView.RPC("ds1", RpcTarget.All);

                    //_isTurn = false;
                    int _diceNum = _turnBasedManager.Dice();
                    //TODO: 테스트를 위한 임시 주석
                    //photonView.RPC("RpcMovePlayer", RpcTarget.All, _diceNum);
                    photonView.RPC("RpcMovePlayer", RpcTarget.All, 20);
                    Debug.Log(_playerPosIndex);
                }
            }
        }

        //주사위 중복 방지 
        // 또는 버튼기능클릭시 ****
        // 구매 쿨타이도 넣어아함 ****
        // 안전빵으로 쿨타임 메서드 두개만들자
        if (PhotonNetwork.LocalPlayer.ActorNumber != PlayerMoveTest.CurrentTurn)
        {
            _isTurn = true;
            _isCoolFinish = false;
        }
        if (Input.GetKeyDown(KeyCode.Q) && _view.IsMine)
        {
            PrintPlayerGroundLists();
        }
    }
    #endregion

    #region 뭔지 모를 함수들
    [PunRPC]
    void ds1()
    {
        mySlider.gameObject.SetActive(false);
    }

    [PunRPC]
    void ds()
    {
        if (PlayerMoveTest.currentTurn == 1)
        {
            mySliderobj.transform.localPosition = new Vector3(-720, 269, 0);
        }
        else if (PlayerMoveTest.currentTurn == 2)
        {
            mySliderobj.transform.localPosition = new Vector3(720, 269, 0);
        }
        else if (PlayerMoveTest.currentTurn == 3)
        {
            mySliderobj.transform.localPosition = new Vector3(695, -269, 0);
        }
        else if (PlayerMoveTest.currentTurn == 4)
        {
            mySliderobj.transform.localPosition = new Vector3(687, -287, 0);
        }

        runningCoroutine = StartCoroutine(Dicecooltimedelay(second));
    }
    #endregion

    #region 주사위 쿨타임
    //주사위 쿨타임
    IEnumerator Dicecooltimedelay(float Second)
    {
        currentDiceCooldown1 = Second;
        mySlider.gameObject.SetActive(true);

        //currentDiceCooldown1 주사위굴러가면 초기화 안보이게?
        while (currentDiceCooldown1 > 0f)
        {
            currentDiceCooldown1 -= Time.deltaTime;
            mySlider.value = currentDiceCooldown1;
            Debug.Log("currentDiceCooldown1 : " + currentDiceCooldown1);
            yield return null;
        }
        mySlider.gameObject.SetActive(false);
        //yield return new WaitForSeconds(Second);
        _isCoolFinish = true;
        currentDiceCooldown1 = Second;
        runningCoroutine = null; //코루틴 중복 방지
    }

    //팝업창 쿨타임(구매, 취소등)
    IEnumerator cooltimedelay(float Second)
    {
        while (currentDiceCooldown2 > 0f)
        {
            currentDiceCooldown2 -= Time.deltaTime;
            Debug.Log("currentDiceCooldown2 : " + currentDiceCooldown1);
            yield return null;
        }
        //yield return new WaitForSeconds(Second);
        currentDiceCooldown2 = Second;
        PlayerMoveTest.Instance.endTurn();
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
        _totalMoney = _money + tileAssetTotal;
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
        StartCoroutine(MovePlayer(num));
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

        // 주인 없을때
        if (currentTile.GetOwner(0) == 0)
        {
            if (photonView.IsMine)
            {
                //쿨타임
                //TODO: 테스트를 위한 주석
                //StartCoroutine(cooltimedelay(second));
                UIManagerP.instance.OnBuyUI(currentTile._tileType);
                UIManagerP.instance.InvokeBuyUI(currentTile, currentTile._tileType);
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
                    //TODO: 테스트를 위한 주석
                    //StartCoroutine(cooltimedelay(second));
                    UIManagerP.instance.OnBuyUI(currentTile._tileType);
                    UIManagerP.instance.InvokeBuyUI(currentTile, currentTile._tileType);
                }
            }
        }
        // 주인 = 다른 사람
        else if(currentTile.GetOwner(0) != _playerNum)
        {
            double currentTileTollPrice = currentTile.TotalTollPrice(currentTile);
            double currentTileBuyPrice = currentTile.TotalBuyPrice(currentTile);
            if (_view.IsMine == true)
            {
                // 통행료 지불 가능 상태라면
                if (_money >= currentTileTollPrice)
                {
                    Debug.Log($"{_playerNum} 통행료 빠져 나간 돈 : " + currentTileTollPrice);
                    
                    _view.RPC("DecreaseMoney", RpcTarget.All, currentTileTollPrice);

                    // 총 자산 확인
                    _view.RPC("TotalMoney", RpcTarget.All);

                    // 토지주인의 돈 증가 함수 실행
                    FindPlayer(currentTile.GetOwner(0))._view.RPC("IncreaseMoney", RpcTarget.All, currentTileTollPrice);
                    Debug.Log($"{currentTile.GetOwner(0)}의 통행료 증가 된 돈 : " + currentTileTollPrice);

                    // 총 자산 확인
                    FindPlayer(currentTile.GetOwner(0))._view.RPC("TotalMoney", RpcTarget.All);

                    if (currentTile._tileType == TileType.Ground)
                    {
                        //TODO: 테스트를 위한 주석
                        //StartCoroutine(cooltimedelay(5f));
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
                else
                {
                    Debug.Log("파산");
                }
            }
        }
        _playerMoveCor = null; // 코루틴이 끝났으므로 null로 초기화
        OnPlayerPositionChanged?.Invoke(_playerPosIndex);//코루틴 끝나고 플레이위치를 받아서위치(SMW)
        Debug.Log(_playerPosIndex +" 이것는 테스트를 위한 것이요.");
    }

    public ServerIngamePlayer FindPlayer(int _actorNum)
    {
        return _players[_actorNum];
    }

    public void StartPointPass()
    {
        _playerManager.IncreaseMoney(1000);
    }

    [PunRPC]
    public void IncreaseMoney(double money)
    {
        _money += money;
    }

    [PunRPC]
    public void DecreaseMoney(double money)
    {
        _money -= money;
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

    /// <summary>
    /// 소유한 일반 땅을 모두 출력 (디버깅용)
    /// </summary>
    public void PrintPlayerGroundLists()
    {
        foreach (var item in _playerGroundLists)
        {
            Debug.Log(item);
        }
    }

    /// <summary>
    /// 소유한 모든 땅의 총 가격 반환
    /// </summary>
    public double TotalLandCost()
    {
        double _totalPrice = 0;

        foreach (var tile in _playerGroundLists)
        {
            _totalPrice += tile._tileLandPrice;
            _totalPrice += tile._tilePensionPrice;
            _totalPrice += tile._tileCondoPrice;
            _totalPrice += tile._tileHotelPrice;
        }

        return _totalPrice;
    }

    /// <summary>
    /// 일반 타일을 구매했을 때 리스트에 추가
    /// </summary>
    public void AddPlayerGroundLists(TileController tileController)
    {
        _playerGroundLists.Add(tileController);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            _playerNum = (int)data[0];
            Debug.Log(_playerNum + "생성");
        }
    }


    public static event Action<int> OnPlayerPositionChanged;//플레이어 위치를 올림픽에게 보냄 
    public static event Action<bool> OlympicCheck;//플레이어가 올림픽을 개최했으면 중복안되게 해줄이벤트


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
