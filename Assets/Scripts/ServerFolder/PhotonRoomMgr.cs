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
public class PhotonRoomMgr : Singleton<PhotonRoomMgr>
{
    public TMP_InputField createRoomInput;
    public TMP_InputField joinRoomInput;

    public Transform roomListPanel;

    public GameObject roomPanels;
    public GameObject serverPanel;
    public GameObject roomPrefab;
    public GameObject LodingPanel;


    //서버연결
    public void isServer()
    {
        //서버 연결
        PhotonNetwork.ConnectUsingSettings();
        LodingPanel.gameObject.SetActive(true);
       
    }


    

    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("방만들기 버튼 클릭");
            //일단 인원 제한없음
            PhotonNetwork.CreateRoom(createRoomInput.text, new RoomOptions()); //방 만들어주는 메서드. 앞엔 방 이름, 뒤엔 옵션

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
        PhotonNetwork.JoinRoom(joinRoomInput.text);
    }
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void QuitRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("룸나감");

    }

    public override void OnConnectedToMaster()
    {
        LodingPanel.gameObject.SetActive(false);
        Debug.Log("서버 연결 완료");
        PhotonNetwork.NickName = FirebaseLoginMgr.user.DisplayName;
        PhotonNetwork.JoinLobby();
        PhotonNetworkMgr.Instance.changeScene("RoomScene");
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
        Debug.Log("클라이언트가 방에 입장시에 알아서 호출");
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
            roomBtn.GetComponentInChildren<Text>().text = roomInfo.Name; //룸 이름을 버튼 텍스트에 담음
            roomBtn.GetComponent<Button>().onClick.AddListener(()=>PhotonNetwork.JoinRoom(roomInfo.Name));
        }
    }






}

