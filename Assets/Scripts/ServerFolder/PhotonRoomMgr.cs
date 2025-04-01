//최동오
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

//싱글톤
public class PhotonRoomMgr : MonoBehaviourPunCallbacks
{
    public TMP_InputField createRoomInput;
    public TMP_InputField joinRoomInput;

    public Transform roomListPanel;

    public GameObject roomPanels;
    public GameObject serverPanel;
    public GameObject roomPrefab;
    public GameObject LodingPanel;

    List<string> names = new List<string>() { "점심내기 한판", "주사위 운빨 겜", "주사위의 신을 찾아라", "내 용돈 줄 사람 구함" };
    static int nameCount;

    //서버연결
    public void isServer()
    {
        //서버 연결
        PhotonNetwork.ConnectUsingSettings();
        LodingPanel.gameObject.SetActive(true);
    }

    string RanddomRoomName()
    {
        string roomname;

        roomname = names[nameCount];
        nameCount++;
        if (names.Count-1 < nameCount)
        {
            nameCount = 0;
        }
        return roomname;
    }

    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("방만들기 버튼 클릭");

            //일단 인원 제한없음
            PhotonNetwork.CreateRoom(RanddomRoomName(), new RoomOptions{ MaxPlayers =4, EmptyRoomTtl =0 ,IsOpen = true}); //방 만들어주는 메서드. 앞엔 방 이름, 뒤엔 옵션
        }
        else
        {
            //혹시몰라서
            //서버 연결
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void JoinRoom()
    {
        Debug.Log("JoinRoom");
        LodingPanel.gameObject.SetActive(true);
        StartCoroutine(FakeLodingWaitGameStart());
        //PhotonNetwork.JoinRoom(joinRoomInput.text);

    }

    public void JoinRandomRoom()
    {
        Debug.Log("JoinRandomRoom");
        LodingPanel.gameObject.SetActive(true);
        StartCoroutine(FakeLodingWaitRandomRoom());
        //PhotonNetwork.JoinRandomRoom();

    }

    public void QuitRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("룸나감");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        StartCoroutine(FakeLodingWait());

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("연결 끊김 감지. 사유: " + cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방 참여 실패. 보통 이러면 새로운 방 생성");
        //PhotonNetwork.CreateRoom(PhotonNetwork.NickName, new RoomOptions()); //방 만들어주는 메서드. 앞엔 방 이름, 뒤엔 옵션
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        PhotonNetworkMgr.Instance.changeScene("InGameRoomScene");

    }

    public override void OnJoinedLobby()
    {

        Debug.Log("로비 입장");

    }

    public override void OnLeftLobby()
    {
        Debug.Log("로비 퇴장");
    }

    //방에서 유저 나가면
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        TurnMgr.leaveNum++;
        if (otherPlayer.ActorNumber == 1)
        {
            TurnMgr.leave1 = true;
        }
        if (otherPlayer.ActorNumber == 2)
        {
            TurnMgr.leave2 = true;
        }
        if (otherPlayer.ActorNumber == 3)
        {
            TurnMgr.leave3 = true;
        }
        if (otherPlayer.ActorNumber == 4)
        {
            TurnMgr.leave4 = true;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("리스트들어옴");
        foreach (Transform child in roomListPanel)
        {

            Destroy(child.gameObject);

        }
        foreach (RoomInfo roomInfo in roomList)
        {
            var roomBtn = Instantiate(roomPrefab, roomListPanel); //룸 리스트 패널 하에 버튼 하나 생성
            roomBtn.GetComponentInChildren<TextMeshProUGUI>().text = roomInfo.Name; //룸 이름을 버튼 텍스트에 담음
            roomBtn.GetComponent<Button>().onClick.AddListener(()=>PhotonNetwork.JoinRoom(roomInfo.Name));
        }
    }


    IEnumerator FakeLodingWait()
    {
        yield return new WaitForSeconds(2f);
        LodingPanel.gameObject.SetActive(false);
        Debug.Log("서버 연결 완료");
        PhotonNetwork.NickName = FirebaseLoginMgr.user.DisplayName;
        PhotonNetwork.JoinLobby();
        //PhotonNetworkMgr.Instance.changeScene("RoomScene");
        PhotonNetworkMgr.Instance.changeScene("RoomScene");
    }

    IEnumerator FakeLodingWaitGameStart()
    {
        yield return new WaitForSeconds(2f);
        LodingPanel.gameObject.SetActive(false);
        PhotonNetwork.JoinRoom(joinRoomInput.text);
    }

    IEnumerator FakeLodingWaitRandomRoom()
    {
        yield return new WaitForSeconds(2f);
        LodingPanel.gameObject.SetActive(false);
        PhotonNetwork.JoinRandomRoom();
    }


}

