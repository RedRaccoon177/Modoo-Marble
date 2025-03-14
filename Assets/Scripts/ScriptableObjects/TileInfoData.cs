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
    [Header("타일 이름")] public string tileName;
    [Header("타일 위치")] public Vector3 tilePos;
    [Header("타일 타입")] public TileType tileType;
}
