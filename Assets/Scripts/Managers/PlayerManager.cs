using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    bool isLoan; // 대출여부
    double _money; // 게임 안에서 사용되는 돈
    int _mapTurn; // 맵을 몇 바퀴 돌앗는지
    List<GameObject> _playerGroundLists; // 가지고 있는 건물 리스트
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
    public void PrintPlayerGroundLists()
    {
        foreach (var item in _playerGroundLists)
        {
            Debug.Log(item);
        }
    }
}
