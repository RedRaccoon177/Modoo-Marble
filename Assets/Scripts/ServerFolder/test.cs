using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 룸에 참가한 플레이어 목록을 UI로 표시하고
// 방장과 일반 플레이어의 버튼을 다르게 처리함
public class test : MonoBehaviourPunCallbacks
{
    public Transform roomListPanel; // 플레이어 UI 프리팹이 생성될 부모 패널
    public GameObject TestPlayerImage; // 플레이어 UI 프리팹
    public int readyCount = 0; // 레디한 사람 수

    // 시작 시 플레이어 리스트 UI 갱신
    private void Start()
    {
        UpdatePlayerList();
    }

    // 새로운 플레이어가 방에 들어올 때 호출되는 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList(); // 입장 시 플레이어 목록 다시 표시
    }

    // 현재 방의 모든 플레이어를 UI로 표시
    public void UpdatePlayerList()
    {
        // 기존에 생성되어 있던 자식 오브젝트(플레이어 UI) 전부 삭제
        for (int i = 0; i < roomListPanel.childCount; i++)
        {
            Destroy(roomListPanel.GetChild(i).gameObject);
        }

        // 현재 방에 있는 모든 플레이어에 대해 UI 생성
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            // 플레이어 UI 생성
            var dd = Instantiate(TestPlayerImage, roomListPanel);

            // 0번 자식 오브젝트 (닉네임 텍스트)에 플레이어 이름 적용
            dd.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[i].NickName;

            var player = PhotonNetwork.PlayerList[i];
            bool isLocalPlayer = player == PhotonNetwork.LocalPlayer;

            // 방장일 경우
            if (player.IsMasterClient)
            {
                // 1번 자식: 상태 텍스트를 GameStart로 설정
                dd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "GameStart";

                // 2번 자식: 버튼 컴포넌트 접근
                var btn = dd.transform.GetChild(2).GetComponent<Button>();

                if (isLocalPlayer)
                {
                    // 본인이 방장일 경우만 게임 시작 버튼 활성화
                    btn.onClick.AddListener(StartBtn);
                    //btn.onClick.AddListener(() => Destroy(dd.transform.GetChild(2).gameObject));
                }
                else
                {
                    // 방장이 아닌 다른 플레이어는 시작 버튼 제거
                    Destroy(btn.gameObject);
                }
            }
            // 일반 플레이어일 경우
            else
            {
                // 1번 자식: 상태 텍스트를 no ready로 설정
                dd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "no ready";

                var btn = dd.transform.GetChild(2).GetComponent<Button>();

                if (isLocalPlayer)
                {
                    // 본인만 버튼 클릭 가능
                    btn.onClick.AddListener(ReadyCountBtn); // 레디 상태 보내기
                    btn.onClick.AddListener(() => Destroy(dd.transform.GetChild(2).gameObject)); // 버튼 제거
                    btn.onClick.AddListener(() => dd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "ready"); // 상태 텍스트 변경
                }
                else
                {
                    // 남의 버튼은 제거
                    Destroy(btn.gameObject);
                }
            }
        }
    }

    // 일반 플레이어가 레디 버튼 누를 때 호출
    public void ReadyCountBtn()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // 레디 카운트 증가 요청을 모든 클라이언트에 보냄
            photonView.RPC("ReadyCount", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    // RPC로 모든 클라이언트가 레디 카운트 증가 처리
    [PunRPC]
    public void ReadyCount(int playerID)
    {
        readyCount++;

        // UI 갱신을 위해 플레이어 목록 순회
        for (int i = 0; i < roomListPanel.childCount; i++)
        {
            Transform playerUI = roomListPanel.GetChild(i);
            TextMeshProUGUI playerNameText = playerUI.GetChild(0).GetComponent<TextMeshProUGUI>();

            Player player = null;

            // 전달받은 playerID로 해당 플레이어 객체 찾기
            foreach (var p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == playerID)
                {
                    player = p;
                    break;
                }
            }

            // 일치하는 닉네임을 찾으면 상태를 ready로 표시
            if (player != null && player.NickName == playerNameText.text)
            {
                playerUI.GetChild(1).GetComponent<TextMeshProUGUI>().text = "ready";
                break;
            }
        }
    }

    // 방장이 게임 시작 버튼을 눌렀을 때 호출됨
    // 모든 사람이 레디 상태일 때만 게임 시작
    [PunRPC]
    public void StartBtn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("게임 시작 버튼 눌렀음");

            // 본인을 제외한 모든 사람이 레디했는지 확인
            if (readyCount >= PhotonNetwork.PlayerList.Length - 1)
            {
                Debug.Log("게임 시작 조건 만족, 씬 전환");
                PhotonNetworkMgr.Instance.changeScene("InGameTestScene");
            }
        }
    }
}
