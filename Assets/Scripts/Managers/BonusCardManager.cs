using Photon.Pun;
using System;
using UnityEngine;

public class BonusCardManager : MonoBehaviourPun
{
    public static BonusCardManager instance; 
    public event Action<int> _bonusCardUI;

    private void Awake()
    {
        if(instance == null) instance = this;
        UIManagerP.instance._bonusCard += TriggerBonusCard;
    }

    /// <summary>
    /// 마스터 클라이언트에서 랜덤으로 보너스 카드 효과를 선택하고 전체 플레이어에게 동기화
    /// </summary>
    public void TriggerBonusCard(TileController tile)
    {
        int randomOption = UnityEngine.Random.Range(0, 4);
        photonView.RPC("Rpc_ShowBonusCard", RpcTarget.All, randomOption);
        _bonusCardUI.Invoke(randomOption);
    }

    /// <summary>
    /// 전체 클라이언트에게 보너스 카드 효과 적용
    /// </summary>
    [PunRPC]
    public void Rpc_ShowBonusCard(int selectedOption)
    {
        // 보너스 효과 실제 처리(예: 돈 주기, 이동 등)도 여기서 분기 가능
        switch (selectedOption)
        {
            case 0:
                Debug.Log("[보너스] 돈을 받았습니다.");
                break;
            case 1:
                Debug.Log("[보너스] 돈을 잃었습니다.");
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
