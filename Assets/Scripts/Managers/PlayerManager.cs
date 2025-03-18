using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int userMoney = 0;
    bool isLoan; // 대출여부
    // 게임 안에서 사용되는 돈 , 데이터 베이스 에서 돈을 가져올거임(300만원)
    double _money; 
    int _mapTurn; // 맵을 몇 바퀴 돌앗는지
    List<TileController> _playerGroundLists; // 가지고 있는 건물 리스트

    public void IncreaseMoney(double money)
    {
        _money += money;
    }

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
    
    // 
    public void AddPlayerGroundLists(TileController tileController)
    {
        _playerGroundLists.Add(tileController);
    }
}
