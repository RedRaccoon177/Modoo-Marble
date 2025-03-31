using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public Transform roomListPanel;
    public GameObject TestPlayerImage;
    public int readyCount = 0;

    public GameObject LodingPanel;

    int playerMoney;//돈을 담아줄것

    //겟차일드 찾고 
    //그거에 텍스를 찾아서 변경

    public async Task LoadPlayerMoney()
    {
        //FirebaseDataMgr.Instance.SaveUserData(FirebaseLoginMgr.user.DisplayName, "money", 2000000);//일단 돈을 넣어준거임
        playerMoney = await FirebaseDataMgr.Instance.LoadUserDataAsync(FirebaseLoginMgr.user.DisplayName, "money", playerMoney);
    }

    private async void Start()
    {
        await LoadPlayerMoney();
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

            dd.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = playerMoney.ToString();

            var player = PhotonNetwork.PlayerList[i];
            bool isLocalPlayer = player == PhotonNetwork.LocalPlayer;

            //방장이냐
            if (PhotonNetwork.PlayerList[i].IsMasterClient)
            {
                dd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "GameStart";

                var btn = dd.transform.GetChild(2).GetComponent<Button>();

                if (isLocalPlayer)
                {
                    btn.onClick.AddListener(StartBtn);
                    //btn.onClick.AddListener(()=>Destroy(dd.transform.GetChild(2).gameObject));
                }
                else
                {
                    Destroy(btn.gameObject);
                }
            }
            //방장아니냐
            else
            {
                dd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "no ready";

                var btn = dd.transform.GetChild(2).GetComponent<Button>();
                if (isLocalPlayer)
                {
                    btn.onClick.AddListener(ReadyCountBtn);
                    btn.onClick.AddListener(() => Destroy(dd.transform.GetChild(2).gameObject));
                    btn.onClick.AddListener(() => dd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "ready");
                }
                else
                {
                    Destroy(btn.gameObject);
                }
                 

            }



        }


    }

    //레디버튼 누를경우 전체의 readyCount가 오름
    public void ReadyCountBtn()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ReadyCount", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    [PunRPC]
    public void ReadyCount(int playerID)
    {
        readyCount++;
        for (int i = 0; i < roomListPanel.childCount; i++)
        {
            Transform playerUI = roomListPanel.GetChild(i);
            TextMeshProUGUI playerNameText = playerUI.GetChild(0).GetComponent<TextMeshProUGUI>();

            Player player = null;
            foreach (var p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == playerID)
                {
                    player = p;
                    break; 
                }
            }

            // 현재 업데이트해야 하는 플레이어 찾기
            //Player player = PhotonNetwork.PlayerList.FirstOrDefault(p => p.ActorNumber == playerID);
            if (player != null && player.NickName == playerNameText.text)
            {
                playerUI.GetChild(1).GetComponent<TextMeshProUGUI>().text = "ready"; // 모든 클라이언트에서 UI 변경
                break;
            }
        }

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
                //StartCoroutine(IngameGo());
                PhotonNetworkMgr.Instance.changeScene("InGameTestScene");

            }
        }

    }

    IEnumerator IngameGo()
    {
        LodingPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        LodingPanel.gameObject.SetActive(false);
        PhotonNetworkMgr.Instance.changeScene("InGameTestScene");
    }




}
