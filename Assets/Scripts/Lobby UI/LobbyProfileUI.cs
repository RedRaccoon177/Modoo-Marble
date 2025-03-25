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

    int playerMoney;

    public async Task LoadPlayerMoney()
    {
        FirebaseDataMgr.Instance.SaveUserData(FirebaseLoginMgr.user.DisplayName, "money", 12000);
        playerMoney = await FirebaseDataMgr.Instance.LoadUserDataAsync(FirebaseLoginMgr.user.DisplayName, "money", playerMoney);
        playerNickNameText.text = FirebaseLoginMgr.user.DisplayName;
        playerMoneyText.text = playerMoney.ToString();
    }

    private async void Start()
    {
        await LoadPlayerMoney();

        string imageUrl = await LoadProfileImageUrl();
        Sprite profileSprite = await LoadImageFromURL(imageUrl);

        if (profileSprite != null)
        {
            ProfileImage.sprite = profileSprite;
        }

    }

    public async Task<string> LoadProfileImageUrl()
    {
        string imageUrl = await FirebaseDataMgr.Instance.LoadUserDataAsync(FirebaseLoginMgr.user.DisplayName, "profileImageUrl", "");
        return imageUrl;
    }

    public async Task<Sprite> LoadImageFromURL(string url)
    {
        using (WWW www = new WWW(url))
        {
            await Task.Run(() =>
            {
                while (!www.isDone) { }
            });

            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D texture = www.texture;
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("이미지 다운로드 실패: " + www.error);
                return null;
            }
        }
    }


}
