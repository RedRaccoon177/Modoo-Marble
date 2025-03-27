using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon;
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
    public double _money;         // 현재 현금 보유액
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
    //List<TileController> _playerGroundLists = new List<TileController>(); // 일반 땅
    public List<TileController> _ownedSeaTiles = new List<TileController>(); // 관광지
    public List<TileController> _playerOwnerTileList = new List<TileController>(); // 내 소유의 모든 타일 저장

    public List<int> _playerOwnerTileViewList = new List<int>();
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

    [PunRPC]
    public void AddPlayerOwnerTileList(int _tileViewNum)
    {
        if (_playerOwnerTileViewList.Contains(_tileViewNum) == false)
        {
            Debug.Log(_playerNum + " 에게 땅 추가" + (_tileViewNum));
            _playerOwnerTileViewList.Add(_tileViewNum);
        }
    }

    [PunRPC]
    public void MinusPlayerOwnerTileList(int _tileViewNum)
    {
        if (_playerOwnerTileViewList.Contains(_tileViewNum) == true)
        {
            Debug.Log(_playerNum + " 에게 땅 없애" + (_tileViewNum));
            _playerOwnerTileViewList.Remove(_tileViewNum);
        }
    }

    /// <summary>
    ///  비용이 마련될 때까지 매각 해버림
    /// </summary>
    /// <param name="_playerOwnerTileList"></param>
    //[PunRPC]
    //public void LowPriceSorting(List<TileController> _playerOwnerTileList)
    //{
    //    // 버블 정렬 (낮은 가격 순)
    //    for (int i=0; i < _playerOwnerTileList.Count() - 1; i++)
    //    {
    //        for (int j=0; j< _playerOwnerTileList.Count() - i - 1; j++)
    //        {
    //            if (_playerOwnerTileList[i].TotalBuyPrice(_playerOwnerTileList[i]) > _playerOwnerTileList[i+1].TotalBuyPrice(_playerOwnerTileList[i+1]))
    //            {
    //                var _temp = _playerOwnerTileList[i]; 
    //                _playerOwnerTileList[i] = _playerOwnerTileList[i + 1];
    //                _playerOwnerTileList[i + 1] = _temp;
    //            }
    //        }
    //    }
    //}

    [PunRPC]
    public void LowPriceSorting(int[] _playerOwnerTileViewArr)
    {
        _playerOwnerTileList.Clear();

        // 배열로 받은거 다시 플레이어 리스트에 넣어주기
        for (int i =0; i < _playerOwnerTileViewArr.Length; i++)
        {
            PhotonView _tileViewId = PhotonView.Find(_playerOwnerTileViewArr[i]);
            TileController _currentTileController = _tileViewId.GetComponent<TileController>();
            _playerOwnerTileList.Add(_currentTileController);
        }

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
    /// 인자값으로 추가로 내야하는 금액 받음
    /// </summary>
    /// <param name="_SaleAmount"></param>
    [PunRPC]
    public void AutomaticSale(double _SaleAmount)
    {
        _view.RPC("LowPriceSorting", RpcTarget.All, _playerOwnerTileViewList.ToArray());
        double _TotalMyLandPrice = 0;
        int i = 0;
        for (i = 0; i < _playerOwnerTileList.Count; i++)
        {
            if (_SaleAmount <= _TotalMyLandPrice)
            {
                break;
            }
            else
            {
                _TotalMyLandPrice += _playerOwnerTileList[i].TotalBuyPrice(_playerOwnerTileList[i]);
            }
        }
        for (int j = 0; j < _playerOwnerTileList.Count; j++)
        {
            for (int k =0; k<4; k++)
            {
                if (_playerOwnerTileList[j].GetOwner(k) == _playerNum)
                {
                    _playerOwnerTileList[j].SetOwner(k,0);
                }
            }
        }
        if (i >= _playerOwnerTileList.Count)
        {
            Debug.Log("파산");
        }

    }

    #region Start문, Update문
    void Start()
    {
        //여기에 돈 쓸거면 플레이어프리팹 안에 있는게 편함
        //나중에  생각하면 싱글톤도 생각해봐야할듯
        _money = 100;
        _totalMoney = _money;
        _players[_playerNum] = this; 
        _view = GetComponent<PhotonView>();
        _mapInfo = FindObjectOfType<MapManager>();
        _turnBasedManager = FindObjectOfType<TurnBasedManager>();
        _playerPosIndex = 0;

        mySliderobj = GameObject.Find("CoolTimeGameObject");
        mySlider = GameObject.Find("CoolTimeGameObject").transform.GetChild(0).GetComponent<Slider>();
    }

    void Update()
    {
        //내턴 and (쿨타임 or 주사위)
        if (PhotonNetwork.LocalPlayer.ActorNumber == TurnMgr.CurrentTurn)
        {

            //내턴 될때 쿹타임 10초 
            if (runningCoroutine == null && _isCoolFinish == false)
            {
                photonView.RPC("Dicecooltime", RpcTarget.All);
            }

            if (Input.GetKeyDown(KeyCode.Space) || _isCoolFinish == true)
            {
                if (photonView.IsMine && _isTurn == true)
                {
                    currentDiceCooldown1 = second;
                    photonView.RPC("HideSlider", RpcTarget.All);

                    _isTurn = false;
                    int _diceNum = _turnBasedManager.Dice();
                    photonView.RPC("RpcMovePlayer", RpcTarget.All, 1);
                }
            }
        }

        //주사위 중복 방지 
        if (PhotonNetwork.LocalPlayer.ActorNumber != TurnMgr.CurrentTurn)
        {
            _isTurn = true;
            _isCoolFinish = false;
        }
    }
    #endregion

    #region 뭔지 모를 함수들
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
        TurnMgr.Instance.endTurn();
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
        
        Debug.Log(_totalMoney + "dd");

        _totalMoney = 0;
        _totalMoney = _money + tileAssetTotal;

        Debug.Log(_totalMoney + "aa");

        if (_totalMoney < 0)
        { 
            _totalMoney = 0;
            Debug.Log( _playerNum + "게임 오버");
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
                if (_money > currentTileTollPrice)
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
                else // 통행료 지불이 불가한 상태라면
                {
                    // 총 내야 하는 통행료 - 현재 돈
                    _view.RPC("AutomaticSale", RpcTarget.All, currentTileTollPrice - _money);
                    Debug.Log("이 금액 부족함 : " + (currentTileTollPrice - _money));
                }
            }
        }
        _playerMoveCor = null; // 코루틴이 끝났으므로 null로 초기화
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

   

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            _playerNum = (int)data[0];
            Debug.Log(_playerNum + "생성");
        }
    }
}
