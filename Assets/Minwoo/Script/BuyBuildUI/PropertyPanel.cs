using UnityEngine;
using UnityEngine.UI;

public class PropertyPanel : MonoBehaviour
{
    public Text propertyNameText;  // 지역 이름
    public Image propertyImage;    // 지역 이미지
    public Text tollPriceText;     // 통행료
    public Text totalBuyPriceText; // 총 구매 비용
    public Text myMoneyText;       // 내 돈

    public Button groundButton;
    public Button smallBuildButton;
    public Button mediumBuildButton;
    public Button bigBuildButton;
    public Button buyButton;
    public Button cancelButton;

    private int totalPrice = 0;  // 총 구매 비용
    private int myMoney = 100;   // 초기 돈 100M

    private void Start()
    {
        // 건물 버튼을 눌렀을 때 가격 설정
        groundButton.onClick.AddListener(() => SelectBuilding(10));
        smallBuildButton.onClick.AddListener(() => SelectBuilding(20));
        mediumBuildButton.onClick.AddListener(() => SelectBuilding(30));
        bigBuildButton.onClick.AddListener(() => SelectBuilding(40));

        // 구매 및 취소 버튼
        buyButton.onClick.AddListener(BuyProperty);
        cancelButton.onClick.AddListener(ClosePanel);
    }

    public void SetPropertyData(string propertyName, Sprite propertySprite, int toll)
    {
        propertyNameText.text = propertyName;
        propertyImage.sprite = propertySprite;
        tollPriceText.text = "Toll Price: " + toll + "M";
        myMoneyText.text = "My Money: " + myMoney + "M";
    }

    private void SelectBuilding(int price)
    {
        totalPrice = price;
        totalBuyPriceText.text = "Total Buy Price: " + totalPrice + "M";
    }

    private void BuyProperty()
    {
        if (myMoney >= totalPrice)
        {
            myMoney -= totalPrice;
            myMoneyText.text = "My Money: " + myMoney + "M";
            Debug.Log("구매 완료!");
        }
        else
        {
            Debug.Log("돈이 부족합니다!");
        }
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
