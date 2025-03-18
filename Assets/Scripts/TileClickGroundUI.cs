using TMPro;
using UnityEngine;

public class TileClickGroundUI : MonoBehaviour
{
    [Header("땅 이름")] public TextMeshProUGUI _tileName;
    [Header("타일 땅 가격")] public TextMeshProUGUI _tileLandPrice;
    [Header("펜션 건물 가격")] public TextMeshProUGUI _tilePensionPrice;
    [Header("콘도 건물 가격")] public TextMeshProUGUI _tileCondoPrice;
    [Header("호텔 건물 가격")] public TextMeshProUGUI _tileHotelPrice;

    [Separator]
    [Header("통행료")] public TextMeshProUGUI _tileToll;
    private void Start()
    {
        UIManagerP.instance._clickEvenetGround += SetTileData;
    }
    public void SetTileData(TileController data)
    {
        _tileName.text = data._tileName;
        _tileLandPrice.text = data._tileLandPrice.ToString();
        _tilePensionPrice.text = data._tilePensionPrice.ToString();
        _tileCondoPrice.text = data._tileCondoPrice.ToString();
        _tileHotelPrice.text = data._tileHotelPrice.ToString();
        // 총 통행료 넣어야함
    }
}
