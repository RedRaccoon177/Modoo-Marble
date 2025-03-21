using System;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class TileController : MonoBehaviour
{
    [Header("보조 타일 타입")] public SubTileType _subTileType;
    [Header("보조 타일 타입")] public UnderTilePrefab subUnderTilePrefab;
    [Header("보조 타일 타입")] public GameObject subTileParent;
    [Header("보조 타일 타입")] public GameObject UnderPrefab;
    [Header("보조 타일 타입")] public GameObject BuildPostions;
    [Header("보조 타일 타입")] public GameObject[] InGameTilePrefabs;

}

public partial class TileController : MonoBehaviour
{
    [Header("타일 키값")] public int _tileKey;
    [Header("타일 이름")] public string _tileName;
    [Header("타일 위치")] public Vector3 _tilePos;
    [Header("타일 타입")] public TileType _tileType;

    [Header("타일 땅 소유주")] public int _tileLandOwner;
    [Header("타일 1번 건물 소유주")] public int _tilePensionOwner;
    [Header("타일 2번 건물 소유주")] public int _tileCondoOwner;
    [Header("타일 3번 건물 소유주")] public int _tileHotelOwner;
    [Header("랜드마크 소유주")] public int _tileLandMarkOwner;

    [Header("지역 가격 색상 타입")] public int _tilePriceColor;

    [Header("타일 땅 가격")] public double _tileLandPrice;
    [Header("타일 1번 건물 가격")] public double _tilePensionPrice;
    [Header("타일 2번 건물 가격")] public double _tileCondoPrice;
    [Header("타일 3번 건물 가격")] public double _tileHotelPrice;
    [Header("랜드마크 가격")] public double _tileLandMarkPrice;

    [Header("타일 땅 통행료")] public double _tileLandToll;
    [Header("타일 1번 건물 통행료")] public double _tilePensionToll;
    [Header("타일 2번 건물 통행료")] public double _tileCondoToll;
    [Header("타일 3번 건물 통행료")] public double _tileHotelToll;
    [Header("랜드마크 통행료")] public double _tileLandMarkToll;

    GameObject _ground;
    GameObject _Sea;

    void Awake()
    {
        TileBuyUI tileBuyUI = FindObjectOfType<TileBuyUI>(); // (가능하면 개선 필요)
        if (tileBuyUI != null)
        {
            tileBuyUI.OnTileValueChange -= ChangeTileData;
            tileBuyUI.OnTileValueChange += ChangeTileData;
        }
    }


    void Start()
    {
        _ground = transform.GetChild(0).gameObject;
        _Sea = transform.GetChild(1).gameObject;
        if (_tileType == TileType.Ground)
        {
            _ground.SetActive(true);
        }
        else if (_tileType == TileType.Sea)
        {
            _Sea.SetActive(true);
        }
    }

    /// <summary>
    /// 타일에 데이터 저장
    /// </summary>
    public void SetTileData(TileInfoData data)
    {
        _tileKey = data._tileKey;
        _tileName = data._tileName;
        _tilePos = data._tilePos;
        transform.position = _tilePos;
        _tileType = data._tileType;
        _subTileType = data._subTileType;

        _tileLandOwner = data._tileLandOwner;
        _tilePensionOwner = data._tilePensionOwner;
        _tileCondoOwner = data._tileCondoOwner;
        _tileHotelOwner = data._tileHotelOwner;
        _tileLandMarkOwner = data._tileLandMarkOwner;

        _tilePriceColor = data._tilePriceColor;

        _tileLandPrice = data._tileLandPrice;
        _tilePensionPrice = data._tilePensionPrice;
        _tileCondoPrice = data._tileCondoPrice;
        _tileHotelPrice = data._tileHotelPrice;
        _tileLandMarkPrice = data._tileLandMarkPrice;

        _tileLandToll = data._tileLandToll;
        _tilePensionToll = data._tilePensionToll;
        _tileCondoToll = data._tileCondoToll;
        _tileHotelToll = data._tileHotelToll;
        _tileLandMarkToll = data._tileLandMarkToll;


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

    public void ChangeTileData(TileController tile)
    {
        Debug.Log("ChangeTileData 호출됨!"); // 디버깅 로그 추가
        _tileLandOwner = tile._tileLandOwner;
        _tilePensionOwner = tile._tilePensionOwner;
        _tileCondoOwner = tile._tileCondoOwner;
        _tileHotelOwner = tile._tileHotelOwner;
    }
}