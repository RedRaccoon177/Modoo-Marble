using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// 플레이어 접속 순서를 기준으로 해당 플레이어의 인게임 정보(닉네임, 자산 등)를 UI에 표시하는 컨트롤러 클래스
/// 각 플레이어의 UI 오브젝트에 붙여서 개별적으로 동작한다
/// </summary>
public class PlayerUIController : MonoBehaviourPunCallbacks
{
    [Header("UI가 맡을 플레이어 접속 순서 (0~3)")]
    public int assignedIndex; // 몇 번째로 접속한 플레이어의 정보를 표시할 것인지 (0 = 첫 번째, 1 = 두 번째 ...)

    [Header("텍스트 필드")]
    public TextMeshProUGUI playerNameText; // 플레이어 닉네임을 표시할 UI 텍스트
    public TextMeshProUGUI moneyText;      // 플레이어의 현재 보유 현금을 표시할 UI 텍스트
    public TextMeshProUGUI totalMoneyText; // 플레이어의 총 자산(현금 + 건물/토지 자산)을 표시할 UI 텍스트

    private int targetActorNumber; // 표시 대상 플레이어의 Photon ActorNumber (고유 번호)

    void Start()
    {
        // PhotonNetwork.PlayerList는 접속 순서대로 정렬된 플레이어 배열이다
        // assignedIndex에 해당하는 플레이어를 찾아낸다
        Player targetPlayer = PhotonNetwork.PlayerList[assignedIndex];

        // 해당 플레이어의 고유 ActorNumber를 저장해놓는다 (나중에 데이터 조회용으로 사용)
        targetActorNumber = targetPlayer.ActorNumber;

        // 닉네임 텍스트를 설정한다. 닉네임이 비어있다면 기본 이름으로 대체한다
        playerNameText.text = !string.IsNullOrEmpty(targetPlayer.NickName)
            ? targetPlayer.NickName
            : $"Player {assignedIndex + 1}";

        // 일정 주기마다 플레이어 자산 정보를 UI에 갱신한다
        InvokeRepeating(nameof(UpdatePlayerUI), 1f, 1f);
    }

    /// <summary>
    /// 플레이어 정보를 기반으로 UI 텍스트를 갱신한다 (1초마다 호출됨)
    /// </summary>
    void UpdatePlayerUI()
    {
        // 전역 플레이어 딕셔너리에서 ActorNumber로 해당 플레이어 데이터를 찾아온다
        if (!ServerIngamePlayer._players.TryGetValue(targetActorNumber, out var playerData))
            return;

        // 찾은 데이터로 현금과 총 자산을 UI에 표시한다
        moneyText.text = $"{playerData._money:N0} ₩";           // 예: 1,000,000 ₩
        totalMoneyText.text = $"{playerData._totalMoney:N0} ₩"; // 예: 3,200,000 ₩
    }
}
