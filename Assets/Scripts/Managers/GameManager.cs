using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon;

public class GameManager : MonoBehaviourPunCallbacks  
{
    int _playerPosIndex;

    // 플레이어 move 2번 실행 방지하기 위해 코루틴 담아두는 변수 
    Coroutine _playerMoveCor;

    // 맵 정보 가져오기(배열로 관리)
    MapManager _mapInfo;

    TurnBasedManager _turnBasedManager;

    //플레이어 정보
    PlayerManager _playerManager;

    [Header("서버")]
    //서버
    [SerializeField] GameObject playerfabs;



    private void Start()
    {
        //서버
        PhotonNetwork.Instantiate(playerfabs.name, Vector3.zero, Quaternion.identity);
       
    }

    public void asd()
    {
        Debug.Log("RpcMovePlayer 들어옴");
        //photonView.RPC("RpcMovePlayer", RpcTarget.All, _turnBasedManager.Dice());
    }

    [PunRPC]
    public void RpcMovePlayer(int num)
    {
    
        Debug.Log("RpcMovePlayer 213들어옴");
        //StartCoroutine(MovePlayer(_turnBasedManager.Dice()));
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

        // 변경된 타일 정보를 이벤트를 통해 옵저버들에게 알림
        TileController currentTile = _mapInfo._tiles[_playerPosIndex].GetComponent<TileController>();
        UIManagerP.instance.OnBuyUI(currentTile._tileType);
        UIManagerP.instance.InvokeBuyUI(currentTile, currentTile._tileType);
        _playerMoveCor = null; // 코루틴이 끝났으므로 null로 초기화
    }

    public void StartPointPass()
    {
        //_playerManager.IncreaseMoney(1000);
    }
}
