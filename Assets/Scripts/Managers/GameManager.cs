using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int _playerPosIndex;

    // 플레이어 move 2번 실행 방지하기 위해 코루틴 담아두는 변수 
    Coroutine _playerMoveCor;

    // 맵 정보 가져오기(배열로 관리)
    MapManager _mapInfo;

    TurnBasedManager _turnBasedManager;

    //플레이어 정보
    PlayerManager _playerManager;

    //값이 변경될 때 발생하는 이벤트
    public event Action<TileController> OnTilePopupChange;

    private void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _mapInfo = FindObjectOfType<MapManager>();
        _turnBasedManager = FindObjectOfType<TurnBasedManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바 입력 감지
        {
            if (_playerMoveCor == null) // 현재 이동 중이 아니면 실행
            {
                _playerMoveCor = StartCoroutine(MovePlayer(_turnBasedManager.Dice()));
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
        int count = 0;  // 실제 이동한 횟수

        while (count < num) // 주사위 값(num)만큼 반복
        {

            // 만약 맵의 끝(39번 타일)을 넘으면 0번으로 돌아감
            if ((_playerPosIndex + count) >= 39)
            {
                _playerPosIndex -= 40;
            }
            // 시작 지점(0번 타일)에 도착하면 보너스 처리
            else if (_playerPosIndex + count == 0)
            {
                StartPointPass();
            }
            count++;
            _playerManager.transform.position = _mapInfo._tiles[_playerPosIndex + count].transform.position;

            yield return new WaitForSeconds(0.1f); // 0.1초 대기 후 다음 이동
        }

        // 최종적으로 위치 업데이트
        _playerPosIndex += count;

        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        //_uiManagerP._buyUI.
        //TODO: 도착한 타일의 팝업을 출력시킨다.
        //1. 1개의 팝업을 만든다.
        //2. 만들어진 팝업에 data를 삽입시킨다.
        //3. 팝업의 위치 좌표를 정 가운데로 가져온다.
        //4. buy 버튼을 클릭하면 좌표를 날려버린다.

        // 변경된 타일 정보를 이벤트를 통해 옵저버들에게 알림
        TileController currentTile = _mapInfo._tiles[_playerPosIndex].GetComponent<TileController>();
        OnTilePopupChange?.Invoke(currentTile);

        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        _playerMoveCor = null; // 코루틴이 끝났으므로 null로 초기화
    }

    public void StartPointPass()
    {
        _playerManager.IncreaseMoney(1000);
    }
}
