using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Photon.Pun.Demo.Procedural;



//마스터 턴이있고
//턴이 지나면 마스터가 턴을 +1함
//내턴일때만 사용가능
public class PlayerMoveTest : MonoBehaviourPunCallbacks
{
    int turn;

    private void Start()
    {
        turn = PhotonNetwork.IsMasterClient ? 1 : 1;

    }

    private void Update()
    {
        //내턴
        if (PhotonNetwork.IsMasterClient)
        {
            //q누르면 턴 넘김
            if (Input.GetKeyDown(KeyCode.Q))
            {
                photonView.RPC("playerMove", RpcTarget.All);
                photonView.RPC("endTurn", RpcTarget.All);
            }
        }

    }

   

    [PunRPC]
    void endTurn()
    {
        turn++;
        Debug.Log(turn+ "입니다");
    }

    [PunRPC]
    void playerMove()
    {
        transform.Translate(5 * Time.deltaTime, 0, 0);
    }


    void sdsd()
    {
        var PlayerList = PhotonNetwork.PlayerList;

        foreach (var dd in PlayerList)
        {
            Debug.Log(dd.NickName); //현재 방 닉네임들 출력
        }
    }

}
