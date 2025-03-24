using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
using System.Security.Cryptography;



//마스터 턴이있고
//턴이 지나면 마스터가 턴을 +1함
//내턴일때만 사용가능
public class PlayerMoveTest : MonoBehaviourPunCallbacks
{ 
    static public int currentTurn = 1;
    public GameObject playerfabs;

    static public int CurrentTurn
    {
        get
        {
            return currentTurn;
        }
        set
        {
            if (value <= 0)
            {
                currentTurn = 1;
            }
            else
            {
                currentTurn = value;
            }
        }
    }




    public TextMeshProUGUI playerTurnText;
    public TextMeshProUGUI currentTurnText;

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            var ss = PhotonNetwork.Instantiate(playerfabs.name, Vector3.zero, Quaternion.identity);
            //ss.GetComponent<MeshRenderer>().material.color = Color.red;
            photonView.RPC("dsds", RpcTarget.All,ss, Color.red); 
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            var ss = PhotonNetwork.Instantiate(playerfabs.name, Vector3.zero, Quaternion.identity);
            ss.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 3)
        {
            var ss = PhotonNetwork.Instantiate(playerfabs.name, Vector3.zero, Quaternion.identity);
            ss.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 4)
        {
            var ss = PhotonNetwork.Instantiate(playerfabs.name, Vector3.zero, Quaternion.identity);
            ss.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }


        playerTurnText.text = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
      

    }

    [PunRPC]
    void dsds(GameObject ss, Color color)
    {
        Debug.Log("ddd");
        ss.GetComponent<MeshRenderer>().material.color = color;
    }


    private void Update()
    {
        //지금 누구 턴?
        currentTurnText.text = currentTurn.ToString();
    }


   

    public void endTurn()
    {
        //내턴일때만 턴넘김 
        if (PhotonNetwork.LocalPlayer.ActorNumber == CurrentTurn)
        {

            try
            {
                if (photonView == null)
                {
                    Debug.LogError("photonView가 null입니다!");
                    return;
                }
                photonView.RPC("NextTurn", RpcTarget.All);

            }
            catch (System.Exception DD)
            {
                Debug.Log(DD);
            }
        }
    }

    [PunRPC]
    void NextTurn()
    {
        CurrentTurn = (CurrentTurn + 1) % (PhotonNetwork.PlayerList.Length + 1);
    }
   

     



}
