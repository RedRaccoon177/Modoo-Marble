using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    double _money;
    List<GameObject> _playerGroundLists;


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
