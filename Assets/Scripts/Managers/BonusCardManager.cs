using Photon.Pun;
using UnityEngine;

public class BonusCardManager : MonoBehaviourPun
{
    public static BonusCardManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void TriggerBonusCard(TileController tile)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int randomOption = Random.Range(0, 4);
            photonView.RPC("Rpc_ShowBonusCard", RpcTarget.All, randomOption);
        }

        UIManagerP.instance.InvokeBonusCardUI(tile);
    }

}
