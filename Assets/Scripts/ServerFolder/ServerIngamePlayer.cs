using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using UnityEngine.UI;
using Photon.Realtime;
using System;

public class ServerIngamePlayer : MonoBehaviourPunCallbacks
{
    int _playerPosIndex =0;

    // 플레이어 move 2번 실행 방지하기 위해 코루틴 담아두는 변수 
    Coroutine _playerMoveCor;

    // 맵 정보 가져오기(배열로 관리)
    MapManager _mapInfo;

    TurnBasedManager _turnBasedManager;

    PlayerManager _playerManager;


    private void Start()
    {
        //여기에 돈 쓸거면 플레이어프리팹 안에 있는게 편함
        //나중에  생각하면 싱글톤도 생각해봐야할듯
        _playerManager = GetComponent<PlayerManager>();
        _mapInfo = FindObjectOfType<MapManager>();
        _turnBasedManager = FindObjectOfType<TurnBasedManager>();

        _playerPosIndex = 0;
       
    }

    private void Update()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
        if (PhotonNetwork.LocalPlayer.ActorNumber == PlayerMoveTest.CurrentTurn && Input.GetKeyDown(KeyCode.Space))
        {
            if (photonView.IsMine)
            {
                Debug.Log("여기들어옴");
                var moveDice = _turnBasedManager.Dice();
                photonView.RPC("RpcMovePlayer", RpcTarget.All, moveDice);
            }
        }
    }

    [PunRPC]
    public void RpcMovePlayer(int num)
    {
        StartCoroutine(MovePlayer(num));
        Debug.Log("RpcMovePlayer 들어옴 + " + num);
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
            transform.position = _mapInfo._tiles[_playerPosIndex + count].transform.position;

            yield return new WaitForSeconds(0.1f); // 0.1초 대기 후 다음 이동
        }

        // 최종적으로 위치 업데이트
        _playerPosIndex += count;

        // 변경된 타일 정보를 이벤트를 통해 옵저버들에게 알림
        TileController currentTile = _mapInfo._tiles[_playerPosIndex].GetComponent<TileController>();
        
        if (photonView.IsMine)
        {
            UIManagerP.instance.OnBuyUI(currentTile._tileType);
            UIManagerP.instance.InvokeBuyUI(currentTile, currentTile._tileType);
        }
        _playerMoveCor = null; // 코루틴이 끝났으므로 null로 초기화
    }

    public void StartPointPass()
    {
        _playerManager.IncreaseMoney(1000);
    }
}
