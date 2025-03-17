using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Photon.Pun.Demo.Procedural;
using UnityEngine.UI;



//마스터 턴이있고
//턴이 지나면 마스터가 턴을 +1함
//내턴일때만 사용가능
public class PlayerMoveTest : MonoBehaviourPunCallbacks
{
    int currentTurn = 1;

    public Text playerTurnText;
    public Text currentTurnText;

    private void Start()
    {
        playerTurnText.text = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        if (PhotonNetwork.IsMasterClient)
        {
            currentTurn = PhotonNetwork.IsMasterClient ? 1 : 1;
        }

    }


    private void Update()
    {
        //지금 누구 턴?
        currentTurnText.text = currentTurn.ToString();

        //내턴일때만
        if (PhotonNetwork.LocalPlayer.ActorNumber == currentTurn && Input.GetKeyDown(KeyCode.W))
        {
            photonView.RPC("playerMove", RpcTarget.All);
        }
    }


    [PunRPC]
    void playerMove()
    {
        transform.Translate(5 * Time.deltaTime, 0, 0);
    }

    public void endTurn()
    {
        //내턴일때만 턴넘김 
        if (PhotonNetwork.LocalPlayer.ActorNumber == currentTurn)
        {
            photonView.RPC("NextTurn", RpcTarget.All);
        }
    }

    [PunRPC]
    void NextTurn()
    {
        currentTurn = (currentTurn + 1 ) % PhotonNetwork.PlayerList.Length;
    }





}
