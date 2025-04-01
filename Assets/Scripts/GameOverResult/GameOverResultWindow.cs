using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Photon.Pun;

/// <summary>
/// 게임 오버 시 플레이어 결과 UI를 생성하고 표시하는 클래스
/// </summary>
public class GameOverResultWindow : MonoBehaviour
{
    [Header("게임 오버 상황")]
    public GameObject _gameOverPanel;

    [Header("플레이어 결과 UI 프리팹")]
    public GameObject _playerResultUIPrefab;

    [Header("결과 UI 부모 오브젝트 (Canvas 내부)")]
    public Transform _contentParent;

    [Header("UI 간격 (Y축)")]
    public float _spacingY = -180f;

    public static double _gameOverMoney;
    public double _startMoney;

    /// <summary>
    /// 플레이어 수에 따라 결과 UI를 생성하고 순서대로 배치
    /// </summary>
    public void CreateResultUIs(double _startmoney)
    {
        _gameOverPanel.SetActive(true);

        // 모든 플레이어 데이터 가져오기
        List<ServerIngamePlayer> allPlayers = new List<ServerIngamePlayer>(ServerIngamePlayer._players.Values);

        // 총 자산 기준으로 내림차순 정렬
        allPlayers.Sort((a, b) => b._totalMoney.CompareTo(a._totalMoney));

        // UI 생성
        for (int i = 0; i < allPlayers.Count; i++)
        {
            ServerIngamePlayer player = allPlayers[i];

            // UI 프리팹 인스턴스 생성
            GameObject uiObj = Instantiate(_playerResultUIPrefab, _contentParent);

            // 위치 설정 (수직으로 차례대로 나열)
            RectTransform rt = uiObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, _spacingY * i);

            // PlayerResult 컴포넌트 접근 후 데이터 세팅
            PlayerResult resultUI = uiObj.GetComponent<PlayerResult>();
            if (resultUI != null)
            {
                resultUI.Setup(i + 1, $"{player._playerNickName}", player._totalMoney);
            }

            if (allPlayers[i]._playerNum == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                _startMoney = _startmoney;
                _gameOverMoney = FirebaseDataMgr.Instance.userMoney;
                _gameOverMoney = _gameOverMoney - _startMoney;
                _gameOverMoney = _gameOverMoney + allPlayers[i]._totalMoney;
            }
        }
    }
}
