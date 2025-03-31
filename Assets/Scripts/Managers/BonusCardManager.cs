using Photon.Pun;
using System;
using UnityEngine;

public class BonusCardManager : MonoBehaviourPun
{
    public static BonusCardManager instance; 
    public event Action<int> _bonusCardUI;
    public ServerIngamePlayer _player;

    double _increaseMoney = 11;
    double _decreaseMoney = 22;


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
        int randomOption = UnityEngine.Random.Range(0, 2);
        Rpc_ShowBonusCard(randomOption, player);
        _bonusCardUI.Invoke(randomOption);

        //photonView.RPC("Rpc_ShowBonusCard", RpcTarget.All, randomOption);
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
                player.photonView.RPC("IncreaseMoney", RpcTarget.All, _increaseMoney);

                break;
            case 1:
                player.photonView.RPC("DecreaseMoney", RpcTarget.All, _decreaseMoney);
                
                break;
            case 2:
                Debug.Log("[보너스] 앞으로 한 칸 이동!");
                
                break;
            case 3:
                Debug.Log("[보너스] 턴 스킵!");
                
                break;
        }
    }
}
