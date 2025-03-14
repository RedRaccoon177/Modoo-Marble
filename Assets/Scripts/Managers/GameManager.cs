using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int _playerPosIndex;
    // 플레이어 move 2번 실행 방지하기 위해 코루틴 담아두는 변수 
    Coroutine _playerMoveCor;
    // 맵 정보 가져오기
    MapManager _mapInfo;
    TurnBasedManager _turnBasedManager;
    PlayerManager _playerManager;

    private void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _mapInfo = FindObjectOfType<MapManager>();
        _turnBasedManager = FindObjectOfType<TurnBasedManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_playerMoveCor == null)
            {
                Debug.Log("실행");
                StartCoroutine(MovePlayer(_turnBasedManager.Dice()));
                Debug.Log("실행 완료");
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
            _playerManager.transform.position = _mapInfo._grounds[_playerPosIndex + count].transform.position;
            yield return new WaitForSeconds(1f);
        }
        _playerPosIndex += count;
        _playerMoveCor = null;
    }
}
