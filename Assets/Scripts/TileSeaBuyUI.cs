using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileSeaBuyUI : MonoBehaviour
{
    [Header("땅 이름")] public TextMeshProUGUI _tileName;
    [Header("타일 땅 값")] public TextMeshProUGUI _tileLandPrice;

    [Separator]
    [Header("1장 보유 통행료")] public TextMeshProUGUI _tileOnePrice;
    [Header("2장 보유 통행료")] public TextMeshProUGUI _tileTwoPrice;
    [Header("3장 보유 통행료")] public TextMeshProUGUI _tileThreePrice;
    [Header("4장 보유 통행료")] public TextMeshProUGUI _tileFourPrice;
    [Header("현재 보유중인 장수")] public TextMeshProUGUI _tileSheet;

    [Separator]
    [Header("총 구매 비용")] public TextMeshProUGUI _tileToll;
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

        _tileOnePrice.text = data._tilePensionPrice.ToString();
        _tileTwoPrice.text = data._tileCondoPrice.ToString();
        _tileThreePrice.text = data._tileHotelPrice.ToString();
        _tileFourPrice.text = data._tileHotelPrice.ToString();
        //_tileSheet.text = 
    }
}
