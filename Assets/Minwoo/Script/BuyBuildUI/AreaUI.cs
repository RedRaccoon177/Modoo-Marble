using UnityEngine;
using UnityEngine.UI;

public class AreaUI : MonoBehaviour
{
    [SerializeField] Text propertyNameText;  // 지역 이름
    [SerializeField] Image propertyImage;    // 지역 이미지
    [SerializeField] Text buildPriceText;    // 건설 가격
    [SerializeField] Text tollPriceText;     // 통행료
    [SerializeField] Button checkButton;     // 체크 버튼

    private void Start()
    {
        // "Check" 버튼을 눌렀을 때 패널 닫기
        checkButton.onClick.AddListener(ClosePanel);
    }

    public void SetPropertyData(string name, Sprite image, int buildPrice, int tollPrice)
    {
        propertyNameText.text = name;
        propertyImage.sprite = image;
        buildPriceText.text = buildPrice+"";
        tollPriceText.text = tollPrice+"";

        gameObject.SetActive(true);  // 패널 열기
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false); // 패널 닫기
    }
}
