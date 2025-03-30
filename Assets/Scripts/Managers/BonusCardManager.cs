using Photon.Pun;
using System;
using UnityEngine;

public enum BonusCardType
{
    GetMoney,   // µ· È¹µæ
    LoseMoney   // µ· ¼Õ½Ç
}

public class BonusCardManager : MonoBehaviour
{
    public static BonusCardManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void ApplyBonusCard(ServerIngamePlayer player)
    {
        BonusCardType cardType = (BonusCardType)UnityEngine.Random.Range(0, 2);

        switch (cardType)
        {
            case BonusCardType.GetMoney:
                player.photonView.RPC("IncreaseMoney", RpcTarget.All, 300000); // µ· È¹µæ
                break;

            case BonusCardType.LoseMoney:
                player.photonView.RPC("DecreaseMoney", RpcTarget.All, 150000); // µ· °¨¼Ò
                break;
        }

        player.photonView.RPC("TotalMoney", RpcTarget.All);

        // UI ÆË¾÷µµ °°ÀÌ È£Ãâ
        BonusCardUI popup = FindObjectOfType<BonusCardUI>();
        if (popup != null)
        {
            popup.ShowCard(cardType);
        }

        // ÅÏ Á¾·á Ã³¸®
        TurnMgr.Instance.endTurn();
    }
}

