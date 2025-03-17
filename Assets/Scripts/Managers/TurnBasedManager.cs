using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedManager : MonoBehaviour
{
    int _diceNumFirst;
    int _diceNumSecond;
    int _currentTurnCount;
    int _maxTurnCount;
    // Queue<Player> playerQueue
    
    public int Dice()
    {
        _diceNumFirst = Random.Range(1, 7);
        _diceNumSecond = Random.Range(1, 7);
        Debug.Log("첫 번째 주사위 숫자 : " + _diceNumFirst);
        Debug.Log("두 번째 주사위 숫자 : " + _diceNumSecond);
        return _diceNumFirst + _diceNumSecond;
    }

    // [PunRpc]
    public void PlayerTurn()
    {
        // playerQueue에서 꺼내고 변수에 저장(현재 차례 플레이어)
        // playerQueue에서 넣기

    }
}
