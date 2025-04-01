using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameLoadingScript : MonoBehaviourPun
{
    public GameObject LodingPanel;

    private void Start()
    {
        StartCoroutine(IngameLoading());
    }

    IEnumerator IngameLoading()
    {
        LodingPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        LodingPanel.SetActive(false);

        yield return new WaitUntil(() => TurnMgr.Instance != null);

        PhotonView turnMgrView = TurnMgr.Instance.GetComponent<PhotonView>();

        if (turnMgrView == null)
        {
            Debug.LogError("[로딩스크립트] TurnMgr에 PhotonView 없음! RPC 실패");
            yield break;
        }

        if (!turnMgrView.IsMine && !PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] 나는 마스터가 아님 → RPC 보낼 수 없음 → 마스터에게 알림 전송");
        }

        turnMgrView.RPC("NotifyMasterPlayerLoaded", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }
}
