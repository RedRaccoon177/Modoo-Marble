using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int _playerPosIndex;
    Coroutine _playerMoveCor;
    MapController _mapInfo;
    TurnBasedManager _turnBasedManager;

    private void Start()
    {
        _mapInfo = FindObjectOfType<MapController>();
        _turnBasedManager = FindObjectOfType<TurnBasedManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_playerMoveCor == null)
            {
                StartCoroutine(MovePlayer(_turnBasedManager.Dice()));
            }
        }
    }
    /// <summary>
    /// 플레이어 이동 : num 숫자가 될때까지 한칸씩 이동함
    /// </summary>
    /// <param name="num"> num 숫자가 될때까지 한칸씩 이동함</param>
    /// <returns></returns>
    IEnumerator MovePlayer(int num)
    {
        int count = 0;
        Debug.Log("플레이어 출발 위치 인덱스 : " + _playerPosIndex);
        while (count < num)
        {
            if ((_playerPosIndex + count) >= 31)
            {
                _playerPosIndex -= 32;
            }
            count++;
            transform.position = _mapInfo.grounds[_playerPosIndex + count].transform.position;
            yield return new WaitForSeconds(1f);
        }
        _playerPosIndex += count;
        _mapInfo.GroudInfo(_playerPosIndex);
        _playerMoveCor = null;
    }
}
