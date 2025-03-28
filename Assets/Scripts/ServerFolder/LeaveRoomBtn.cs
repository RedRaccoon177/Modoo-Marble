using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoomBtn : MonoBehaviourPunCallbacks
{
    public void LeaveRoom()
    {
        Debug.Log("방나가는버튼클릭햇음");
        PhotonRoomMgr.Instance.QuitRoom();
    }

    public void Die()
    {
        photonView.RPC("die", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber,true);
    }

    [PunRPC]
    public void die(int a, bool b)
    {
        TurnMgr.Instance.StopTurn(a,b);
    }

    public void Live()
    {
        photonView.RPC("Live", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, false);
    }

    [PunRPC]
    public void live(int a, bool b)
    {
        TurnMgr.Instance.StopTurn(a, b);
    }

    

}
