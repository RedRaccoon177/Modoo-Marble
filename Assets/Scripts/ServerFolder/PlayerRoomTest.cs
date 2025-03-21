//최동오
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
public class PlayerRoomTest : Singleton<PhotonRoomMgr>
{
    public Text playerNickName1;
    public Text playerNickName2;
    public Text playerNickName3;

    private void Start()
    {
        UpdatePlayerListUI();

    }

    private void Update()
    {

        Debug.Log($"현재 플레이어 수: {PhotonNetwork.PlayerList.Length}");

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"플레이어 닉네임: {player.NickName}");
        }
    }

    void UpdatePlayerListUI()
    {
        try
        {
            if (PhotonNetwork.PlayerList.Length >= 1)
                playerNickName1.text = $"마스터: {PhotonNetwork.PlayerList[0].NickName}";

            if (PhotonNetwork.PlayerList.Length >= 2)
                playerNickName2.text = $"플레이어: {PhotonNetwork.PlayerList[1].NickName}";

            if (PhotonNetwork.PlayerList.Length >= 3)
                playerNickName3.text = $"플레이어: {PhotonNetwork.PlayerList[2].NickName}";
        }
        catch (System.Exception dd)
        {
            Debug.Log(dd);
        }
    }



}
