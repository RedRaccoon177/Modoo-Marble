using UnityEngine;
public enum TileType
{
    Ground,Sea,Item,Start,Island, Olympics, Travel
}

[CreateAssetMenu(fileName = "TileInfoData")]
public class TileInfoData : ScriptableObject
{
    public string tileName;
    public Vector3 tilePos;
    public TileType tileType;
}
