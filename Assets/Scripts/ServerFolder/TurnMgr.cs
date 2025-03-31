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
public class TurnMgr : Singleton<TurnMgr>
{
    static public int currentTurn = 1;
    static public int leaveNum = 0;
    static public bool leave1 = false;
    static public bool leave2 = false;
    static public bool leave3 = false;
    static public bool leave4 = false;
    public int playerlist;


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
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string nickname = PhotonNetwork.LocalPlayer.NickName;
        object[] initData = new object[] { actorNumber, nickname };

        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate(playerfabs[0].name, Vector3.zero, Quaternion.identity, 0, initData);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            PhotonNetwork.Instantiate(playerfabs[1].name, Vector3.zero, Quaternion.identity, 0, initData);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 3)
        {
            PhotonNetwork.Instantiate(playerfabs[2].name, Vector3.zero, Quaternion.identity, 0, initData);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 4)
        {
            PhotonNetwork.Instantiate(playerfabs[3].name, Vector3.zero, Quaternion.identity, 0, initData);
        }

        playerTurnText.text = PhotonNetwork.LocalPlayer.ActorNumber.ToString();

        //몇명인지 담음
        playerlist = PhotonNetwork.PlayerList.Length;
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

        CurrentTurn = (CurrentTurn + 1) % (PhotonNetwork.PlayerList.Length + 1+ leaveNum);
        //CurrentTurn += leaveNum;


        //임시
        if (leave1 == true && currentTurn == 1)
        {
            CurrentTurn = 2;
        }
        if (leave2 == true && currentTurn == 2)
        {
            CurrentTurn = 3;
        }
        if (leave3 == true && currentTurn == 3)
        {
            CurrentTurn = 4;
            //if (PhotonNetwork.PlayerList.Length == 2) //이쪽만 바꾸면 됨
            //{
            //    Debug.Log("null뜸");
            //    CurrentTurn = 1;
            //}
        }


        if (leave4 == true && currentTurn == 4)
        {
            CurrentTurn = 1;
        } 

         
    }

    //파산될때 호출**
    //StopTurn(MyActorNumber,true);
    //자기번호 정지시킴
    //StopTurn(MyActorNumber,false);
    //자기번호 정지풀음
    //**endturn함수보다 먼저 수행해야댐
    [PunRPC]
    public void StopTurn(int LocalPlayerActorNumber, bool isStop)
    {
        switch (LocalPlayerActorNumber)
        {
            case 1:
                leave1 = isStop; break;
            case 2:
                leave2 = isStop; break;
            case 3:
                leave3 = isStop; break;
            case 4:
                leave4 = isStop; break;
            default: break;
        }
    }

    //모든 플레이 턴 정지 풀음
    //명시적으로 false넣어 주게 시킴
    [PunRPC]
    public void ResetTurn(bool isStop)
    {
        leave1 = isStop;
        leave2 = isStop;      
        leave4 = isStop;
        leave3 = isStop;
    }




}
