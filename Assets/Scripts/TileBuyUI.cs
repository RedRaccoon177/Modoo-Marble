using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileBuyUI : MonoBehaviour
{
    [Header("타일 땅 가격")] public TextMeshProUGUI _tileLandPrice;
    [Header("펜션 건물 가격")] public TextMeshProUGUI _tilePensionPrice;
    [Header("콘도 건물 가격")] public TextMeshProUGUI _tileCondoPrice;
    [Header("호텔 건물 가격")] public TextMeshProUGUI _tileHotelPrice;

    [Separator]
    [Header("통행료")] public TextMeshProUGUI _tileToll;
    [Header("총 구매 비용")] public TextMeshProUGUI _tileTotalCost;
    [Header("보유 현금")] public TextMeshProUGUI _playerMoney;

    public void SetTileData(TileInfoData data)
    {
        _tileLandPrice.text = data._tileLandPrice.ToString("N0");
        _tilePensionPrice.text = data._tilePensionPrice.ToString("N0");
        _tileCondoPrice.text = data._tileCondoPrice.ToString("N0");
        _tileHotelPrice.text = data._tileHotelPrice.ToString("N0");
    }


}
