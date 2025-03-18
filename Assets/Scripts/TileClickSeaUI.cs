using TMPro;
using UnityEngine;

public class TileClickSea : MonoBehaviour
{
    [Header("관광지 이름")] public TextMeshProUGUI _tileName;

    public void SetTileData(TileController data)
    {
        _tileName.text = data._tileName;
    }
}
