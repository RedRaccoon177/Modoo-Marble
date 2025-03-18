using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using Photon.Realtime;


public class PlayerMoveTest2 : MonoBehaviourPunCallbacks
{
    PlayerMoveTest playerMoveTest;

    private void Update()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
        Debug.Log(PlayerMoveTest.currentTurn);
        //내턴일때만 PhotonNetwork.LocalPlayer.ActorNumber == playerMoveTest.currentTurn
        if (PhotonNetwork.LocalPlayer.ActorNumber == PlayerMoveTest.CurrentTurn  &&Input.GetKeyDown(KeyCode.W) && photonView.IsMine)
        {
            photonView.RPC("playerMove", RpcTarget.All);
        }
    }



    [PunRPC]
    void playerMove()
    {
        transform.Translate(2 , 0, 0);
    }


}
