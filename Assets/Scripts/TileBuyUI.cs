using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileBuyUI : MonoBehaviour
{
    MapManager _mapInfo;

    [Header("타일 땅 가격")] public TextMeshProUGUI _tileLandPrice;
    [Header("타일 1번 건물 가격")] public TextMeshProUGUI _tilePensionPrice;
    [Header("타일 2번 건물 가격")] public TextMeshProUGUI _tileCondoPrice;
    [Header("타일 3번 건물 가격")] public TextMeshProUGUI _tileHotelPrice;

    //통행료
    //총 구매 비용 = 땅 + 펜션 + 콘도 (특정 조건 비교)
    //보유 현금

    private void Start()
    {
        //_mapInfo._tiles[_playerPosIndex];
    }

}
