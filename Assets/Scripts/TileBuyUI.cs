using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileBuyUI : MonoBehaviour
{
    [Header("땅 이름")] public TextMeshProUGUI _tileName;
    [Header("타일 땅 가격")] public TextMeshProUGUI _tileLandPrice;
    [Header("펜션 건물 가격")] public TextMeshProUGUI _tilePensionPrice;
    [Header("콘도 건물 가격")] public TextMeshProUGUI _tileCondoPrice;
    [Header("호텔 건물 가격")] public TextMeshProUGUI _tileHotelPrice;

    [Separator]
    [Header("통행료")] public TextMeshProUGUI _tileToll;
    [Header("총 구매 비용")] public TextMeshProUGUI _tileTotalCost;
    [Header("보유 현금")] public TextMeshProUGUI _playerMoney;

    void Start()
    {
        // GameManager의 OnValueChanged 이벤트 구독
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.OnValueChanged += SetTileData;
    }


    public void SetTileData(TileController data)
    {
        if (data == null) return;

        _tileName.text = data._tileName;
        _tileLandPrice.text = data._tileLandPrice.ToString();
        _tilePensionPrice.text = data._tilePensionPrice.ToString();
        _tileCondoPrice.text = data._tileCondoPrice.ToString();
        _tileHotelPrice.text = data._tileHotelPrice.ToString();
    }
}
