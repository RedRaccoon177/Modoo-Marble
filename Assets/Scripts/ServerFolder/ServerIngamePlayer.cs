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

/// <summary>
/// 게임 내 플레이어 상태 및 행동을 관리하는 클래스
/// </summary>
public class ServerIngamePlayer : MonoBehaviourPunCallbacks,IPunInstantiateMagicCallback
{
    bool _isLoan; // 대출여부
    public int _playerNum;
    int _playerNickName;

    //주사위 굴리기전 쿨타임
    bool _isCoolFinish = false;
    Coroutine runningCoroutine;

    // 게임 내 자금 (초기값은 테스트용)
    public double _money = 10000000;
    public static Dictionary<int, ServerIngamePlayer> _players = new Dictionary<int, ServerIngamePlayer>();

    int _mapTurn; // 맵 회전 수
    public PhotonView _view;
    List<TileController> _playerGroundLists = new List<TileController>(); // 일반 땅 소유 리스트
    int _playerPosIndex = 0; // 현재 타일 인덱스

    Coroutine _playerMoveCor; // 플레이어 이동 코루틴

    MapManager _mapInfo;
    TurnBasedManager _turnBasedManager;
    PlayerManager _playerManager;

    bool _isTurn = true; // 현재 턴인지 여부

    public int _SeaBuyCount = 0; // 관광지 보유 수

    public List<TileController> _ownedSeaTiles = new List<TileController>(); // 보유 중인 Sea 타입 타일들

    [Header("플레이어 총 자산")] public double _totalMoney;

    void Start()
    {
        //여기에 돈 쓸거면 플레이어프리팹 안에 있는게 편함
        //나중에  생각하면 싱글톤도 생각해봐야할듯
        _money = 100;
        _players[_playerNum] = this; 
        _view = GetComponent<PhotonView>();
        _mapInfo = FindObjectOfType<MapManager>();
        _turnBasedManager = FindObjectOfType<TurnBasedManager>();
        _playerPosIndex = 0;
    }

    void Update()
    {
        //내턴 and 스페이스바 or 쿨타임끝남 
        if (PhotonNetwork.LocalPlayer.ActorNumber == PlayerMoveTest.CurrentTurn)
        {
            //내턴 될때 쿹타임 10초 
            if (runningCoroutine == null)
            {
                runningCoroutine = StartCoroutine(Dicecooltimedelay(5f));
            }

            if (Input.GetKeyDown(KeyCode.Space) || _isCoolFinish == true)
            {

                if (photonView.IsMine && _isTurn == true)
                {
                    _isTurn = false;
                    Debug.Log("여기들어옴");
                    var ddd = _turnBasedManager.Dice();
                    photonView.RPC("RpcMovePlayer", RpcTarget.All, 1);
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

    public void TotalMoney()
    {
        //각 타일들 40번에 걸쳐서 내꺼면 값 추가
        for (int i = 0; i < _mapInfo._tiles.Length; i++)
        { 
            TileController currentTile = _mapInfo._tiles[i].GetComponent<TileController>();
        }

        //_totalMoney = _money + 
    }

    //팝업창 쿨타임(구매, 취소등)
    IEnumerator cooltimedelay(float Scond)
    {
        yield return new WaitForSeconds(10000000000);
        PlayerMoveTest.Instance.endTurn();
    }

    //주사위 쿨타임
    IEnumerator Dicecooltimedelay(float second)
    {
        Debug.Log("10초전");
        Debug.Log("_isCoolFinish" + _isCoolFinish);
        yield return new WaitForSeconds(10000000000);
        _isCoolFinish = true;
        Debug.Log("10초후");
        Debug.Log("_isCoolFinish" + _isCoolFinish);
        runningCoroutine = null; //코루틴 중복 방지
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
                StartCoroutine(cooltimedelay(5f));
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
                    StartCoroutine(cooltimedelay(5f));
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
                    // 토지주인의 돈 증가 함수 실행
                    FindPlayer(currentTile.GetOwner(0))._view.RPC("IncreaseMoney", RpcTarget.All, currentTileTollPrice);
                    Debug.Log($"{currentTile.GetOwner(0)}의 통행료 증가 된 돈 : " + currentTileTollPrice);
                    if (currentTile._tileType == TileType.Ground)
                    {
                        StartCoroutine(cooltimedelay(5f));
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
}
