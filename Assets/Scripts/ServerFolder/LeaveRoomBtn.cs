using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoomBtn : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        //애네를 기억하는 애를 만들어서 그거를 TRUE해야댈듯
        int aa = PhotonNetwork.PlayerList.Length;
        //플레이어가 없는 번호는 leave =true로
        for (int i = 1; i <= 4; i++)
        {
            if (aa >= i)
            {

            }
            else
            {
                TurnMgr.Instance.StopTurn(i, true);
            }

        }
    }

    //방 나가기
    public void LeaveRoom()
    {
        Debug.Log("방나가는버튼클릭햇음");
        Die();
        PhotonNetwork.LeaveRoom(this);
        PhotonNetworkMgr.Instance.changeScene("RoomScene");
    }
    public void Die()
    {
        photonView.RPC("die", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, true);
    }

    [PunRPC]
    public void die(int a, bool b)
    {
        TurnMgr.Instance.StopTurn(a,b);
    }

    //턴 
    public void Live()
    {
        photonView.RPC("live", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, false);
    }

    [PunRPC]
    public void live(int a, bool b)
    {
        TurnMgr.Instance.StopTurn(a, b);
    }
}
