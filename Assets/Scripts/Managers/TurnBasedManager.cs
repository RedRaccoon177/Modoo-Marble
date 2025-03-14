using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedManager : MonoBehaviour
{
    int _diceNumFirst;
    int _diceNumSecond;
    public int Dice()
    {
        _diceNumFirst = Random.Range(1, 7);
        _diceNumSecond = Random.Range(1, 7);
        Debug.Log("첫 번째 주사위 숫자 : " + _diceNumFirst);
        Debug.Log("두 번째 주사위 숫자 : " + _diceNumFirst);
        return _diceNumFirst + _diceNumSecond;
    }
}
