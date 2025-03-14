using UnityEngine;

public class TileController : MonoBehaviour
{
    public string tileName;
    public TileType tileType;

    // 타일 정보를 설정하는 메서드
    public void SetTileData(TileInfoData data)
    {
        tileName = data._tileName;
        tileType = data._tileType;
        transform.position = data._tilePos;

        // 타일 색상 적용 (MeshRenderer가 있다고 가정)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = GetTileColor(tileType);
        }

        Debug.Log($"타일 적용 완료: {tileName}, 타입: {tileType}, 위치: {transform.position}");
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
