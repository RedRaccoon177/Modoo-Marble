// 최동오
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;

// 룸 안의 플레이어 닉네임을 UI에 표시함
public class PlayerRoomNickname : Singleton<PhotonRoomMgr>
{
    // 첫 번째 플레이어 닉네임을 표시할 텍스트
    public Text playerNickName1;

    // 두 번째 플레이어 닉네임을 표시할 텍스트
    public Text playerNickName2;

    // 세 번째 플레이어 닉네임을 표시할 텍스트
    public Text playerNickName3;

    // 게임 시작 시 자동 실행되는 함수
    private void Start()
    {
        // 플레이어 리스트 UI를 처음 한 번 갱신
        UpdatePlayerListUI();
    }

    // 매 프레임마다 실행됨
    private void Update()
    {
        // 현재 방에 있는 플레이어 수를 디버그 로그로 출력
        Debug.Log("현재 플레이어 수: " + PhotonNetwork.PlayerList.Length);

        // 현재 방에 있는 각 플레이어의 닉네임을 로그로 출력
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log("플레이어 닉네임: " + player.NickName);
        }
    }

    // 플레이어 목록을 UI에 표시하는 함수
    void UpdatePlayerListUI()
    {
        try
        {
            // 플레이어가 1명 이상일 경우 첫 번째 텍스트에 마스터 닉네임 표시
            if (PhotonNetwork.PlayerList.Length >= 1)
                playerNickName1.text = "마스터: " + PhotonNetwork.PlayerList[0].NickName;

            // 플레이어가 2명 이상일 경우 두 번째 텍스트에 표시
            if (PhotonNetwork.PlayerList.Length >= 2)
                playerNickName2.text = "플레이어: " + PhotonNetwork.PlayerList[1].NickName;

            // 플레이어가 3명 이상일 경우 세 번째 텍스트에 표시
            if (PhotonNetwork.PlayerList.Length >= 3)
                playerNickName3.text = "플레이어: " + PhotonNetwork.PlayerList[2].NickName;
        }
        catch (System.Exception dd)
        {
            // 예외 발생 시 디버그 로그로 출력
            Debug.Log(dd);
        }
    }
}
