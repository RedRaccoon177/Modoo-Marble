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
            PhotonNetwork.CreateRoom(createRoomInput.text, new RoomOptions{ MaxPlayers =4, EmptyRoomTtl =0 ,IsOpen = true}); //방 만들어주는 메서드. 앞엔 방 이름, 뒤엔 옵션
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
    
    private Dictionary<string, GameObject> roomDictionary = new Dictionary<string, GameObject>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("리스트들어옴"); // 디버그 로그

        // 현재 딕셔너리에 등록된 방 이름들을 복사해서 리스트로 저장 (삭제할 때 사용)
        List<string> removeRoom = new List<string>(roomDictionary.Keys);

        foreach (RoomInfo roomInfo in roomList)
        {
            // 방이 삭제된 경우
            if (roomInfo.RemovedFromList == true)
            {
                // 해당 방이 딕셔너리에 있으면
                if (roomDictionary.ContainsKey(roomInfo.Name) == true)
                {
                    // 버튼 오브젝트 삭제 후 딕셔너리에서도 제거
                    Destroy(roomDictionary[roomInfo.Name]);
                    roomDictionary.Remove(roomInfo.Name);
                }
            }
            else // 새로 생성된 방이거나 기존 방
            {
                // 아직 버튼을 만들지 않은 새 방인 경우
                if (roomDictionary.ContainsKey(roomInfo.Name) == false)
                {
                    // 방 버튼 프리팹을 생성하여 방 리스트 패널 아래에 붙임
                    var roomBtn = Instantiate(roomPrefab, roomListPanel);
                    roomBtn.GetComponentInChildren<TextMeshProUGUI>().text = roomInfo.Name;
                    roomBtn.GetComponent<Button>().onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));

                    // 딕셔너리에 방 이름과 버튼 오브젝트 등록
                    roomDictionary.Add(roomInfo.Name, roomBtn);
                }
            }
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

