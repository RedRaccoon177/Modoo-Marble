using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// 접속한 플레이어 수에 따라 UI 오브젝트를 자동 활성화하고 플레이어가 접속한 순서대로 UI 오브젝트에 정보를 연결함.
/// </summary>
public class PlayerUIManager : MonoBehaviourPunCallbacks
{
    [Header("플레이어 UI 오브젝트 4개 (순서대로 등록)")]
    public GameObject[] playerUIObjects; // 최대 4개의 플레이어 UI 오브젝트 (인스펙터에 순서대로 등록해야 함)

    void Start()
    {
        // 현재 룸에 접속한 플레이어 수를 가져옴
        int playerCount = PhotonNetwork.PlayerList.Length;

        for (int i = 0; i < playerUIObjects.Length; i++)
        {
            if (i < playerCount)
            {
                // 해당 인덱스의 UI 오브젝트를 활성화
                playerUIObjects[i].SetActive(true);

                // UI 오브젝트에서 PlayerUIController 컴포넌트를 가져옴
                var uiController = playerUIObjects[i].GetComponent<PlayerUIController>();

                if (uiController != null)
                {
                    // 해당 UI가 표시할 플레이어 인덱스를 설정함
                    uiController.assignedIndex = i;
                }
            }
            else
            {
                // 플레이어가 존재하지 않는 슬롯은 UI를 비활성화
                playerUIObjects[i].SetActive(false);
            }
        }
    }
}