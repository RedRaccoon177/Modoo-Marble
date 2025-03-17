using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonImage : MonoBehaviour
{
    //[SerializeField] AlreadyBuyBuilding //이거는 건물일 이미 가지고 있으면 처리할꺼임 승윤씨한테서 값을 받아오자
    [SerializeField] Text BulidType; //무슨건물 지을껀지 타입? 받아오기 
    [SerializeField] Text BulidPrice; //지을건물 가격 받아오기 
    [SerializeField] Button button;//이거는 자기자신을 담아오자(이 스크립트가 있는 버튼을)
    private Image buttonImage;
    private bool isToggled = false;

    bool Ground;
    bool SmallBuilding;
    bool MediumBuilding;
    bool BigBuilding;
    bool Landmark;


    void Start()
    {
        /*
        Ground = 받아온 클래스에 건물이 땅이 지어져 있는지 
        SmallBuilding =받아온 클래스에 건물이 팬션이 지어져 있는지 
        MediumBuilding =받아온 클래스에 건물이 모텔이 지어져 있는지 
        BigBuilding =받아온 클래스에 건물이 호텔이 지어져 있는지 
        Landmark =받아온 클래스에 건물이 랜드마크가 지어져 있는지 
        */


        buttonImage = button.GetComponent<Image>();
        button.onClick.AddListener(ToggleColor);
        button.onClick.AddListener(Buildtype);
        //BulidType.text = $"{/*받아온값 건물타입*/}";
        //BulidPrice.text = $"{/*받아온값 건물가격*/}*/만";

    }

    void Buildtype()
    {
        //if ()
        //{
        //    BuildTypeCheak = 1;
        //}

    }


    void ToggleColor()
    {
        if (isToggled == true)// 값을 만약 받았으면  || 넣고 받은값 bool 체크하고 사용(이미 산거는 노란색으로 표시할꺼임)
        {
            buttonImage.color = new Color(1f, 1f, 0f);
        }
        else
        {
            buttonImage.color = Color.white;//이거는 토글이 안되있을때 흰색으로 
        }

        isToggled = !isToggled;
    }
}
