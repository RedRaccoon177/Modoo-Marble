using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerResult : MonoBehaviour
{
    [Header("플레이어 게임오버 정보들")]
    public TextMeshProUGUI _ranking;
    public TextMeshProUGUI _nickName;
    public TextMeshProUGUI _totalMoney;

    /// <summary>
    /// 결과 UI를 세팅하는 함수
    /// </summary>
    /// <param name="rank">순위</param>
    /// <param name="nickname">닉네임 또는 식별 이름</param>
    /// <param name="totalMoney">총 자산</param>
    public void Setup(int rank, string nickname, double totalMoney)
    {
        _ranking.text = $"{rank}등";
        _nickName.text = nickname;
        _totalMoney.text = $"{totalMoney:N0} G";
    }
}
