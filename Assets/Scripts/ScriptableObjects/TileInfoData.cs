using UnityEngine;

public enum TileType
{
    Ground,     // 0 일반 토지
    Sea,        // 1 관광지
    Item,       // 2 포츈 Card
    Start,      // 3 시작점
    Island,     // 4 무인도
    Olympics,   // 5 올림픽
    Travel,     // 6 여행지
    revenue,    // 7 국세청
    casino      // 8 카지노
}
public enum SubTileType
{
    Bangkok = 1,
    Beijing = 3,
    Macau = 4,
    Dokdo = 5,
    NewDelhi = 6,
    Dubai = 8,
    Cairo = 9,
    Toronto = 11,
    Gyeongpodae = 12,
    SanJose = 13,
    Bogota = 14,
    Hawaii = 15,
    Santiago = 16,
    BuenosAires = 18,
    SaoPaulo = 19,
    Athens = 21,
    Prague = 23,
    Berlin = 24,
    Santorini = 25,
    Lisbon = 26,
    Madrid = 27,
    Haeundae = 28,
    Rome = 29,
    Osaka = 31,
    London = 32,
    Paris = 34,
    Tahiti = 35,
    NewYork = 37,
    Seoul = 39,

}
public partial class TileInfoData : ScriptableObject
{
    [Header("보조 타일 타입")] public SubTileType _subTileType;
}

[CreateAssetMenu(fileName = "TileInfoData")]
public partial class TileInfoData : ScriptableObject
{
    [Header("타일 키값")] public int _tileKey;
    [Header("타일 이름")] public string _tileName;
    [Header("타일 위치")] public Vector3 _tilePos;
    [Header("타일 타입")] public TileType _tileType;

    [Separator]
    [Header("타일 땅 소유주")] public int _tileLandOwner;
    [Header("타일 1번 건물 소유주")] public int _tilePensionOwner;
    [Header("타일 2번 건물 소유주")] public int _tileCondoOwner;
    [Header("타일 3번 건물 소유주")] public int _tileHotelOwner;
    [Header("랜드마크 소유주")] public int _tileLandMarkOwner;

    [Separator]
    [Header("지역 가격 색상 타입")] public int _tilePriceColor;

    [Separator]
    [Header("타일 땅 가격")] public double _tileLandPrice;
    [Header("타일 1번 건물 가격")] public double _tilePensionPrice;
    [Header("타일 2번 건물 가격")] public double _tileCondoPrice;
    [Header("타일 3번 건물 가격")] public double _tileHotelPrice;
    [Header("랜드마크 가격")] public double _tileLandMarkPrice;

    [Separator]
    [Header("타일 땅 통행료")] public double _tileLandToll;
    [Header("타일 1번 건물 통행료")] public double _tilePensionToll;
    [Header("타일 2번 건물 통행료")] public double _tileCondoToll;
    [Header("타일 3번 건물 통행료")] public double _tileHotelToll;
    [Header("랜드마크 통행료")] public double _tileLandMarkToll;

}
