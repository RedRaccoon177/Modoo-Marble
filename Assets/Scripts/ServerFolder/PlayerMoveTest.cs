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
public class PlayerMoveTest : Singleton<PlayerMoveTest>
{ 
    static public int currentTurn = 1;
    public GameObject[] playerfabs;

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
            PhotonNetwork.Instantiate(playerfabs[0].name, Vector3.zero, Quaternion.identity);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            PhotonNetwork.Instantiate(playerfabs[1].name, Vector3.zero, Quaternion.identity);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 3)
        {
            PhotonNetwork.Instantiate(playerfabs[2].name, Vector3.zero, Quaternion.identity);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 4)
        {
            PhotonNetwork.Instantiate(playerfabs[3].name, Vector3.zero, Quaternion.identity);
        }

        playerTurnText.text = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
    }

    private void Update()
    {
        //지금 누구 턴?
        currentTurnText.text = currentTurn.ToString();
    }

    public void endTurn()
    {
        UIManagerP.instance.OffBuyUIPanel();
        UIManagerP.instance.OffFactorUI();
        UIManagerP.instance.OffClickUI();

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
            catch (System.Exception error)
            {
                Debug.Log(error);
            }
        }
    }

    [PunRPC]
    void NextTurn()
    {
        CurrentTurn = (CurrentTurn + 1) % (PhotonNetwork.PlayerList.Length + 1);
    }
}
