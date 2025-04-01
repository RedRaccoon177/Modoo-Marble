using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// 플레이어 접속 순서를 기준으로 해당 플레이어의 인게임 정보(닉네임, 자산 등)를 UI에 표시하는 컨트롤러 클래스
/// 각 플레이어의 UI 오브젝트에 붙여서 개별적으로 동작한다
/// </summary>
public class PlayerUIController : MonoBehaviourPunCallbacks, IPlayerDataObserver
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

    private void Start()
    {
        Player targetPlayer = PhotonNetwork.PlayerList[_assignedIndex];
        _targetActorNumber = targetPlayer.ActorNumber;

        _playerNameText.text = !string.IsNullOrEmpty(targetPlayer.NickName)
            ? targetPlayer.NickName : $"Player {_assignedIndex + 1}";

        // 옵저버 등록
        ServerIngamePlayer.RegisterObserver(this);

        // 초기 UI 한 번만 세팅
        Invoke(nameof(DelayedInitialUpdate), 0.3f);
    }

    void DelayedInitialUpdate()
    {
        UpdatePlayerUI();
    }

    // 옵저버 콜백 구현
    public void OnPlayerDataChanged(int actorNumber)
    {
        if (actorNumber == _targetActorNumber)
        {
            UpdatePlayerUI(); // 이 플레이어 UI만 갱신
        }
    }

    /// <summary>
    /// 플레이어 정보를 기반으로 UI 텍스트를 갱신한다.
    /// </summary>
    void UpdatePlayerUI()
    {
        if (!ServerIngamePlayer._players.TryGetValue(_targetActorNumber, out var playerData))
            return;

        _rankingText.text = $"{_ranking}위";
        _moneyText.text = $"{playerData._money:N0} G";
        _totalMoneyText.text = $"{playerData._totalMoney:N0} G";

        SetPlayerRanking();
    }

    /// <summary>
    /// 로컬 플레이어 정보 가져오기
    /// </summary>
    void SetPlayerRanking()
    {
        var allPlayers = ServerIngamePlayer._players.Values.ToList();

        // 자산 기준으로 내림차순 정렬
        var sortedPlayers = allPlayers.OrderByDescending(p => p._totalMoney).ToList();

        int currentRank = 1;
        int sameRankCount = 1;

        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            // 첫 번째 플레이어는 무조건 1등
            if (i == 0)
            {
                sortedPlayers[i]._ranking = currentRank;
            }
            else
            {
                if (sortedPlayers[i]._totalMoney == sortedPlayers[i - 1]._totalMoney)
                {
                    // 이전 플레이어와 자산이 같다면 같은 등수
                    sortedPlayers[i]._ranking = currentRank;
                    sameRankCount++;
                }
                else
                {
                    // 자산이 다르면 순위 + 중복된 순위 수 만큼 건너뜀
                    currentRank += sameRankCount;
                    sortedPlayers[i]._ranking = currentRank;
                    sameRankCount = 1;
                }
            }

            // 이 오브젝트가 맡은 플레이어라면 랭킹 저장
            if (sortedPlayers[i]._playerNum == _targetActorNumber)
            {
                _ranking = sortedPlayers[i]._ranking;
            }
        }
    }
}