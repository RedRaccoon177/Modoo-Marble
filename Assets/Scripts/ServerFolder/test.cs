using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviourPunCallbacks
{
    public Transform roomListPanel;
    public GameObject TestPlayerImage;
    public int readyCount = 0;

    //겟차일드 찾고 
    //그거에 텍스를 찾아서 변경

    private void Start()
    {
        UpdatePlayerList();
    }
   


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public void UpdatePlayerList()
    {
        for (int i = 0; i < roomListPanel.childCount; i++)
        {
            Destroy(roomListPanel.GetChild(i).gameObject);
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            var dd = Instantiate(TestPlayerImage, roomListPanel); //룸 리스트 패널 하에 하나 생성
            dd.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[i].NickName;

            if (PhotonNetwork.PlayerList[i].IsMasterClient)
            {
                dd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "GameStart";

                //방장만 게임시작 버튼 기능 추가
                var btn = dd.transform.GetChild(2).GetComponent<Button>();
                btn.onClick.AddListener(StartBtn);
            }
            else
            {
                dd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "only ready";
                var btn = dd.transform.GetChild(2).GetComponent<Button>();
                btn.onClick.AddListener(ReadyCountBtn);
            }

        }


    }

    //레디버튼 누를경우 전체의 readyCount가 오름
    public void ReadyCountBtn()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ReadyCount", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ReadyCount()
    {
        readyCount++;

    }

    //전체 유저수가 레디를 누를경우 클릭가능하게
    [PunRPC]
    public void StartBtn()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("게임 시작 버튼 눌럿음");
            if (readyCount >= PhotonNetwork.PlayerList.Length - 1)
            {
                Debug.Log("게임시작 버튼 눌러서 인게임 씬으로 넘김 ");
                PhotonNetworkMgr.Instance.changeScene("testIngameScene");
            }
        }

    }


}
