using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyProfileUI : MonoBehaviour
{
    public TextMeshProUGUI playerNickNameText;
    public TextMeshProUGUI playerMoneyText;

    public Image ProfileImage; // 이거는 따로 플레이어 이미지 넣어줄것 코드 안했음

    int playerMoney;//돈을 담아줄것

    public async Task LoadPlayerMoney()
    {
        FirebaseDataMgr.Instance.SaveUserData(FirebaseLoginMgr.user.DisplayName, "money", 2000000);//일단 돈을 넣어준거임
        playerNickNameText.text = FirebaseLoginMgr.user.DisplayName;
        playerMoney = await FirebaseDataMgr.Instance.LoadUserDataAsync(FirebaseLoginMgr.user.DisplayName, "money", playerMoney);//playerMoney에 서버에 가지고있는 돈을 넣어줌

        playerMoneyText.text = playerMoney.ToString();
    }

    private async void Start()
    {
        await LoadPlayerMoney();
    }

}
