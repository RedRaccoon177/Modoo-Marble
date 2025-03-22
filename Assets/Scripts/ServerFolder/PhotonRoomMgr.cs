//최동오
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;

// 포톤 룸 매니저 클래스
// Singleton<PhotonRoomMgr>을 상속받아 싱글톤 패턴으로 구현됨
// 어디서든 PhotonRoomMgr.Instance로 접근 가능
public class PhotonRoomMgr : Singleton<PhotonRoomMgr>
{
    public InputField createRoomInput; // 방 생성 시 입력받는 방 이름
    public InputField joinRoomInput;   // 방 참가 시 입력받는 방 이름
    public GameObject roomPrefab;      // 룸 버튼 프리팹
    public Transform roomListPanel;    // 룸 버튼이 들어갈 부모 패널
    public GameObject roomPanels;      // 룸 관련 UI 패널
    public GameObject serverPanel;     // 서버 접속 UI 패널

    // 포톤 서버에 연결하는 함수
    public void isServer()
    {
        // 포톤 기본 설정을 이용해 서버에 연결 요청
        PhotonNetwork.ConnectUsingSettings();

        // 서버 연결 시 UI 패널을 보여줄 수 있음 (현재는 비활성화됨)
        // serverPanel.gameObject.SetActive(true);
    }

    // 방 생성 함수
    public void CreateRoom()
    {
        // 포톤 서버에 연결되어 있는지 확인
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("방만들기 버튼 클릭");

            // 입력된 방 이름으로 새 방 생성
            // RoomOptions는 방의 설정값들을 담을 수 있는 객체 (현재 기본값 사용)
            PhotonNetwork.CreateRoom(createRoomInput.text, new RoomOptions());
        }
        else
        {
            // 서버에 연결되어 있지 않다면 연결부터 다시 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // 입력된 방 이름으로 참가 요청
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomInput.text);
    }

    // 랜덤한 방에 참가 요청
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // 현재 방에서 나가는 함수
    public void QuitRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("룸나감");
    }

    // 마스터 서버에 연결되었을 때 자동 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 연결 완료");

        // Firebase에 저장된 유저 닉네임을 포톤 닉네임으로 설정
        PhotonNetwork.NickName = FirebaseLoginMgr.user.DisplayName;

        // 포톤 로비에 참가 요청
        PhotonNetwork.JoinLobby();

        // 룸 목록이 있는 씬으로 전환
        PhotonNetworkMgr.Instance.changeScene("RoomScene");
    }

    // 서버와 연결이 끊겼을 때 자동 호출되는 콜백 함수
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("연결 끊김 감지. 사유: " + cause);
    }

    // 랜덤 방 참가 실패 시 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방 참여 실패. 보통 이러면 새로운 방 생성");

        // 방 참가 실패 시 방을 생성하도록 처리할 수 있음 (현재 주석 처리됨)
        // PhotonNetwork.CreateRoom(PhotonNetwork.NickName, new RoomOptions());
    }

    // 방 참가가 완료되었을 때 자동 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log("클라이언트가 방에 입장시에 호출됨");

        // 입장한 방의 이름 출력
        Debug.Log(PhotonNetwork.CurrentRoom.Name);

        // 인게임 룸 씬으로 전환
        PhotonNetworkMgr.Instance.changeScene("InGameRoomScene");
    }

    // 로비에 입장했을 때 자동 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장");
    }

    // 로비에서 나갔을 때 자동 호출되는 콜백 함수
    public override void OnLeftLobby()
    {
        Debug.Log("로비 퇴장");
    }

    // 로비에서 방 목록이 업데이트될 때마다 호출되는 콜백 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("리스트들어옴");

        // 기존에 있던 방 버튼 모두 제거
        foreach (Transform child in roomListPanel)
        {
            Destroy(child.gameObject);
        }

        // 새로운 방 리스트 받아와서 버튼 생성
        foreach (RoomInfo roomInfo in roomList)
        {
            // roomPrefab을 복제하여 panel 아래에 생성
            var roomBtn = Instantiate(roomPrefab, roomListPanel);

            // 버튼의 텍스트에 방 이름을 표시
            roomBtn.GetComponentInChildren<Text>().text = roomInfo.Name;

            // 버튼 클릭 시 해당 방에 참가하도록 이벤트 연결
            roomBtn.GetComponent<Button>().onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));
        }
    }
}

