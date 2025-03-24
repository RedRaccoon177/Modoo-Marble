using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class REALTEST : MonoBehaviour
{
    public TextMeshProUGUI playerNickNameText;
    public TextMeshProUGUI playerMoneyText;
    public TextMeshProUGUI playerTurn;

    int playerMoney = 0;

    


    public async Task LoadPlayerMoney()
    {
        FirebaseDataMgr.Instance.SaveUserData(FirebaseLoginMgr.user.DisplayName, "money",10001);
        playerMoney = await FirebaseDataMgr.Instance.LoadUserDataAsync(FirebaseLoginMgr.user.DisplayName, "money", playerMoney);
        playerNickNameText.text = FirebaseLoginMgr.user.DisplayName;
        playerMoneyText.text = playerMoney.ToString();
    }

    private async void Update()
    {
        await LoadPlayerMoney();
        //playerTurn.text = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
    }



}
