using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    bool _isLoan; // 대출여부
    int _playerNum;
    int _playerNickName;

    // 게임 안에서 사용되는 돈 , 데이터 베이스 에서 돈을 가져올거임(300만원)
    public double _money = 10000000; // 임시 값
    PhotonView _view;
    int _mapTurn; // 맵을 몇 바퀴 돌앗는지
    List<TileController> _playerGroundLists; // 가지고 있는 토지 리스트
    private void Start()
    {
        _view = GetComponent<PhotonView>();
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

    public double GetMoney()
    {
        return _money;
    }
    
    public void MapTurn()
    {
        _mapTurn++;
    }
    
    public void PrintPlayerGroundLists()
    {
        foreach (var item in _playerGroundLists)
        {
            Debug.Log(item);
        }
    }
    
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

    // 
    public void AddPlayerGroundLists(TileController tileController)
    {
        _playerGroundLists.Add(tileController);
    }
}
