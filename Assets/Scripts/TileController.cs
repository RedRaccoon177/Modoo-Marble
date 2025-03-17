using UnityEngine;

public class TileController : MonoBehaviour
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

    // 타일 정보를 설정하는 메서드
    public void SetTileData(TileInfoData data)
    {
        _tileKey = data._tileKey;
        _tileName = data._tileName;
        transform.position = data._tilePos;
        _tileType = data._tileType;

        // 타일 색상 적용 (MeshRenderer가 있다고 가정)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = GetTileColor(_tileType);
        }
    }

    // 타일 타입별 색상 반환 메서드
    private Color GetTileColor(TileType type)
    {
        switch (type)
        {
            case TileType.Ground: return Color.green;
            case TileType.Sea: return Color.blue;
            case TileType.Item: return Color.yellow;
            case TileType.Start: return Color.red;
            case TileType.Island: return Color.gray;
            case TileType.Olympics: return Color.magenta;
            case TileType.Travel: return Color.cyan;
            case TileType.revenue: return Color.black;
            case TileType.casino: return Color.white;
            default: return Color.white;
        }
    }
}
