using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Die();
        FirebaseDataMgr.Instance.SaveUserData(FirebaseLoginMgr.user.DisplayName, "money", GameOverResultWindow._gameOverMoney);
        Debug.Log("방나가는버튼클릭햇음");
        PhotonNetwork.LeaveRoom();
        StartCoroutine(waitSecond());
        SceneManager.LoadScene("RoomScene");
    }

    IEnumerator waitSecond()
    {
        yield return new WaitForSeconds(0.2f);
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
