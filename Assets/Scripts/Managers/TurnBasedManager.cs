using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedManager : MonoBehaviour
{
    int _diceNumFirst;
    int _diceNumSecond;
    int _currentTurnCount;
    int _maxTurnCount;
    
    public int Dice()
    {
        _diceNumFirst = Random.Range(1, 7);
        _diceNumSecond = Random.Range(1, 7);
        UIManagerP.instance.InvokeDiceNum(_diceNumFirst, _diceNumSecond);
        return _diceNumFirst + _diceNumSecond;
    }
}
