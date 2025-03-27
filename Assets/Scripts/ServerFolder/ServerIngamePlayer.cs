using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using UnityEngine.UI;
using Photon.Realtime;
using System;
using System.Linq;
using Unity.VisualScripting;

/// <summary>
/// 게임 내 플레이어 상태 및 행동을 관리하는 클래스
/// </summary>
public class ServerIngamePlayer : MonoBehaviourPunCallbacks
{
    bool _isLoan; // 대출여부
    public int _playerNum;
    int _playerNickName;

    [Header("쿨타임")]
    //주사위 굴리기전 쿨타임
    bool _isCoolFinish = false;
    Coroutine runningCoroutine;
    float second = 5f;
    public float currentDiceCooldown1 = 5f; //주사위 쿨타임
    [SerializeField] private float currentDiceCooldown2 = 5f; //UI팝업창 쿨타임

    Slider mySlider;
    GameObject mySliderobj;


    // 게임 내 자금 (초기값은 테스트용)
    public double _money = 10000000;

    int _mapTurn; // 맵 회전 수
    PhotonView _view;
    List<TileController> _playerGroundLists = new List<TileController>(); // 일반 땅 소유 리스트
    int _playerPosIndex = 0; // 현재 타일 인덱스

    Coroutine _playerMoveCor; // 플레이어 이동 코루틴

    MapManager _mapInfo;
    TurnBasedManager _turnBasedManager;
    PlayerManager _playerManager;

    bool _isTurn = true; // 현재 턴인지 여부

    public int _SeaBuyCount = 0; // 관광지 보유 수

    public List<TileController> _ownedSeaTiles = new List<TileController>(); // 보유 중인 Sea 타입 타일들

    void Start()
    {
        //여기에 돈 쓸거면 플레이어프리팹 안에 있는게 편함
        //나중에  생각하면 싱글톤도 생각해봐야할듯
        _money = 10000000;
        _playerNum = PhotonNetwork.LocalPlayer.ActorNumber;
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
            //내턴 될때 쿹타임 10초 
            if (runningCoroutine == null && _isCoolFinish ==false)
            {
                //runningCoroutine = StartCoroutine(Dicecooltimedelay(second));
                photonView.RPC("ds", RpcTarget.All);
            }

            if (Input.GetKeyDown(KeyCode.Space) || _isCoolFinish == true)
            {

                if (photonView.IsMine && _isTurn == true)
                {
                    _isTurn = false;
                    currentDiceCooldown1 = second;
                    //여기에 기능  //currentDiceCooldown1 주사위굴러가면 초기화 안보이게? 넣으면댐 
                    //ui를 숨기다던지 //처음에 find로 찾아놓고 바꿔야댐 
                    photonView.RPC("ds1", RpcTarget.All);
                    Debug.Log("여기들어옴");
                    var ddd = _turnBasedManager.Dice();
                    photonView.RPC("RpcMovePlayer", RpcTarget.All, ddd);
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

    /// <summary>
    /// 플레이어가 소유한 Sea 타일을 추가한다
    /// </summary>
    public void AddSeaTile(TileController tile)
    {
        if (tile._tileType == TileType.Sea && !_ownedSeaTiles.Contains(tile))
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
            Debug.Log("1.현재 플레이어 번호 : " + _playerNum);
            Debug.Log("1.땅 소유 플레이어 번호 : " + currentTile.GetOwner(0));
            if (photonView.IsMine)
            {
                //쿨타임
                StartCoroutine(cooltimedelay(second));
                UIManagerP.instance.OnBuyUI(currentTile._tileType);
                UIManagerP.instance.InvokeBuyUI(currentTile, currentTile._tileType);
            }
        }
        // 주인 = 나, 타일 타입 = 그라운드
        else if (currentTile.GetOwner(0) == _playerNum)
        {
            Debug.Log("2.현재 플레이어 번호 : " + _playerNum);
            Debug.Log("2.땅 소유 플레이어 번호 : " + currentTile.GetOwner(0));
            if (currentTile._tileType == TileType.Ground)
            {
                if (photonView.IsMine)
                {  
                    //쿨타임
                    StartCoroutine(cooltimedelay(second));
                    UIManagerP.instance.OnBuyUI(currentTile._tileType);
                    UIManagerP.instance.InvokeBuyUI(currentTile, currentTile._tileType);
                }
            }
        }
        // 주인 = 다른 사람
        else if(currentTile.GetOwner(0) != _playerNum && photonView.IsMine)
        {
            Debug.Log("3.현재 플레이어 번호 : " + _playerNum);
            Debug.Log("3.땅 소유 플레이어 번호 : " + currentTile.GetOwner(0));
            double currentTileTollPrice = currentTile.TotalTollPrice(currentTile);
            if (_money > currentTileTollPrice)
            {
                _view.RPC("DecreaseMoney", RpcTarget.All, currentTileTollPrice);
                if (currentTile._tileType == TileType.Ground)
                {
                    //쿨타임
                    StartCoroutine(cooltimedelay(second));
                    UIManagerP.instance.OnFactorUI(currentTile, this);
                }
            }
            else
            {
                Debug.Log("파산");
            }
        }
        _playerMoveCor = null; // 코루틴이 끝났으므로 null로 초기화
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
}
