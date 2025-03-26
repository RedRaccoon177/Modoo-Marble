using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using UnityEngine.UI;
using Photon.Realtime;
using System;
using System.Linq;

/// <summary>
/// 게임 내 플레이어 상태 및 행동을 관리하는 클래스
/// </summary>
public class ServerIngamePlayer : MonoBehaviourPunCallbacks
{
    bool _isLoan; // 대출여부
    public int _playerNum;
    int _playerNickName;

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

    private void Start()
    {
        //여기에 돈 쓸거면 플레이어프리팹 안에 있는게 편함
        //나중에  생각하면 싱글톤도 생각해봐야할듯
        _money = 10000000;
        _playerNum = PhotonNetwork.LocalPlayer.ActorNumber;
        _view = GetComponent<PhotonView>();
        _mapInfo = FindObjectOfType<MapManager>();
        _turnBasedManager = FindObjectOfType<TurnBasedManager>();
        _playerPosIndex = 0;
    }

    private void Update()
    {
        // 자신의 턴이고 스페이스바를 누르면 이동 시작
        if (PhotonNetwork.LocalPlayer.ActorNumber == PlayerMoveTest.CurrentTurn && Input.GetKeyDown(KeyCode.Space))
        {
            if (photonView.IsMine && _isTurn)
            {
                //_isTurn = false;
                var ddd = _turnBasedManager.Dice();
                photonView.RPC("RpcMovePlayer", RpcTarget.All, 1);
            }
        }

        // 턴이 넘어갔으면 다시 가능하도록 설정
        if (PhotonNetwork.LocalPlayer.ActorNumber != PlayerMoveTest.CurrentTurn)
        {
            _isTurn = true;
        }

        // 디버그용: Q 키로 소유한 땅 목록 출력
        if (Input.GetKeyDown(KeyCode.Q) && _view.IsMine)
        {
            PrintPlayerGroundLists();
        }
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
            if (photonView.IsMine)
            {
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
                    UIManagerP.instance.OnBuyUI(currentTile._tileType);
                    UIManagerP.instance.InvokeBuyUI(currentTile, currentTile._tileType);
                }
            }
        }
        // 주인 = 다른 사람
        else if(currentTile.GetOwner(0) != _playerNum)
        {
            double currentTileTollPrice = currentTile.TotalTollPrice(currentTile);
            if (_view.IsMine == true)
            {
                if (_money > currentTileTollPrice)
                {
                    Debug.Log("빠져 나간 돈 : " + currentTileTollPrice);
                    _view.RPC("DecreaseMoney", RpcTarget.All, currentTileTollPrice);
                    // 땅 주인 돈 ++
                    // 땅 주인의 PhotoneNetwork.LocalPlayer.ActorNumber 번호 알고있는데 어떻게 찾을지
                    // 1. 딕셔너리를 하나 만들어서 플레이어 생성될때 그 딕셔너리에 플레이어 저장
                    // 2. find로 찾기
                    if (currentTile._tileType == TileType.Ground)
                    {
                        UIManagerP.instance.OnFactorUI(currentTile, this);
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
