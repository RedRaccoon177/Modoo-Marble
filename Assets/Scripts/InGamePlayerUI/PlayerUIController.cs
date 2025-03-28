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
    public int _assignedIndex; // 몇 번째로 접속한 플레이어의 정보를 표시할 것인지 (0 = 첫 번째, 1 = 두 번째 ...)
    public int _ranking;

    [Header("텍스트 필드")]
    public TextMeshProUGUI _rankingText;
    public TextMeshProUGUI _playerNameText; // 닉네임
    public TextMeshProUGUI _moneyText;      // 현재 보유 현금
    public TextMeshProUGUI _totalMoneyText; // 총 자산(현금 + 건물/토지 자산)

    private int _targetActorNumber; // 각 플레이어의 Photon ActorNumber (고유 번호)

    void Start()
    {
        Player targetPlayer = PhotonNetwork.PlayerList[_assignedIndex];

        _targetActorNumber = targetPlayer.ActorNumber;

        // 닉네임 출력, 닉네임이 비어있다면 기본 이름으로 대체
        _playerNameText.text = !string.IsNullOrEmpty(targetPlayer.NickName)
            ? targetPlayer.NickName : $"Player {_assignedIndex + 1}";

        // 일정 주기마다 플레이어 자산 정보를 UI에 갱신함. 추후 변경해도 됨.
        InvokeRepeating(nameof(UpdatePlayerUI), 1f, 1f);
    }

    /// <summary>
    /// 플레이어 정보를 기반으로 UI 텍스트를 갱신한다.
    /// </summary>
    void UpdatePlayerUI()
    {
        // 전역 플레이어 딕셔너리에서 ActorNumber로 해당 플레이어 데이터를 찾아옴
        if (!ServerIngamePlayer._players.TryGetValue(_targetActorNumber, out var playerData))
            return;

        _moneyText.text = $"{playerData._money:N0} G";           // 예: 1,000,000 G
        _totalMoneyText.text = $"{playerData._totalMoney:N0} G"; // 예: 3,200,000 G
    }
}
