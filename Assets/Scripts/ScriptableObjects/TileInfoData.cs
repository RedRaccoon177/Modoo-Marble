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

[CreateAssetMenu(fileName = "TileInfoData")]
public class TileInfoData : ScriptableObject
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
    [Header("타일 땅 가격")] public int _tileLandPrice;
    [Header("타일 1번 건물 가격")] public int _tilePensionPrice;
    [Header("타일 2번 건물 가격")] public int _tileCondoPrice;
    [Header("타일 3번 건물 가격")] public int _tileHotelPrice;
    [Header("랜드마크 가격")] public int _tileLandMarkPrice;

    [Separator]
    [Header("타일 땅 통행료")] public int _tileLandToll;
    [Header("타일 1번 건물 통행료")] public int _tilePensionToll;
    [Header("타일 2번 건물 통행료")] public int _tileCondoToll;
    [Header("타일 3번 건물 통행료")] public int _tileHotelToll;
    [Header("랜드마크 통행료")] public int _tileLandMarkToll;

}
