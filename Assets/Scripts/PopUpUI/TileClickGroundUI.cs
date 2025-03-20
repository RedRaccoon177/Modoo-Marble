using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileClickGroundUI : MonoBehaviour
{
    [Header("땅 이름")] public TextMeshProUGUI _tileName;
    [Header("타일 땅 가격")] public TextMeshProUGUI _tileLandPrice;
    [Header("펜션 건물 가격")] public TextMeshProUGUI _tilePensionPrice;
    [Header("콘도 건물 가격")] public TextMeshProUGUI _tileCondoPrice;
    [Header("호텔 건물 가격")] public TextMeshProUGUI _tileHotelPrice;

    //추가
    [Header("건물")] public TextMeshProUGUI Building;
    [Header("올림픽")] public TextMeshProUGUI Olympics;
    [Header("축제")] public TextMeshProUGUI Festivities;
    [Header("독점")] public TextMeshProUGUI Monopolize;
    [Header("기타")] public TextMeshProUGUI Etc;

    [Separator]
    [Header("통행료")] public TextMeshProUGUI _tileToll;
    [Header("닫기 버튼")] public Button _closeBtn;

    private void Awake()
    {
        UIManagerP.instance._clickChangeDataGround += SetTileData;
        _closeBtn.onClick.AddListener(() => UIManagerP.instance.OffClickUI());
    }
    public void SetTileData(TileController data)
    {
        _tileName.text = data._tileName;
        _tileLandPrice.text = data._tileLandPrice.ToString();
        _tilePensionPrice.text = data._tilePensionPrice.ToString();
        _tileCondoPrice.text = data._tileCondoPrice.ToString();
        _tileHotelPrice.text = data._tileHotelPrice.ToString();

        //Building.text = data.<건물값넣기>.ToString();
        //Olympics.text = data.<올림픽값넣기>.ToString();
        //Festivities.text = data.<축제값넣기>.ToString();
        //Monopolize.text = data.<독점값넣기>.ToString();
        //Etc.text = data.<기타값넣기>.ToString();

        //_tileToll.text = data.<통행료값넣기>.ToString();

    }
}
