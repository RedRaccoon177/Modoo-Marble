using Photon.Pun;
using System;
using UnityEngine;

public class BonusCardManager : MonoBehaviourPun
{
    public static BonusCardManager instance; 
    public event Action<int, double> _bonusCardUI;
    public ServerIngamePlayer _player;

    double _increaseMoney = 500;
    double _decreaseMoney = 300;
    double _noting = 0;


    private void Awake()
    {
        if(instance == null) instance = this;
        UIManagerP.instance._bonusCard += TriggerBonusCard;
    }

    /// <summary>
    /// 마스터 클라이언트에서 랜덤으로 보너스 카드 효과를 선택하고 전체 플레이어에게 동기화
    /// </summary>
    public void TriggerBonusCard(ServerIngamePlayer player)
    {
        int randomOption = UnityEngine.Random.Range(0, 3);
        Rpc_ShowBonusCard(randomOption, player);
    }

    /// <summary>
    /// 전체 클라이언트에게 보너스 카드 효과 적용
    /// </summary>
    public void Rpc_ShowBonusCard(int selectedOption, ServerIngamePlayer player)
    {
        // 보너스 효과 실제 처리(예: 돈 주기, 이동 등)도 여기서 분기 가능
        switch (selectedOption)
        {
            case 0:
                //금액 증가
                player.photonView.RPC("IncreaseMoney", RpcTarget.All, _increaseMoney);
                _bonusCardUI.Invoke(selectedOption, _increaseMoney);

                break;
            case 1:
                //금액 감소
                player.photonView.RPC("DecreaseMoney", RpcTarget.All, _decreaseMoney);
                _bonusCardUI.Invoke(selectedOption, _decreaseMoney);

                break;
            case 2:
                //시작 칸으로 가기
                player.photonView.RPC("BonusCardMovePlayer", RpcTarget.All);
                _bonusCardUI.Invoke(selectedOption, _noting);

                break;
            case 3:
                //

                _bonusCardUI.Invoke(selectedOption, _noting);
                break;
        }
    }
}
