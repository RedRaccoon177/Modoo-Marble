using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class TileController : MonoBehaviourPun
{
    [Header("보조 타일 타입")] public SubTileType _subTileType;
    [Header("보조 타일 타입")] public UnderTilePrefab subUnderTilePrefab;
    [Header("보조 타일 타입")] public GameObject subTileParent;
    [Header("보조 타일 타입")] public GameObject UnderPrefab;
    [Header("보조 타일 타입")] public GameObject BuildPostions;
}

public partial class TileController : MonoBehaviourPun
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

        photonView.RPC("RPC_ChangeTileData",
            RpcTarget.AllBuffered,
            _tileKey,
            tile._tileLandOwner,
            tile._tilePensionOwner,
            tile._tileCondoOwner,
            tile._tileHotelOwner
        );
    }

    [PunRPC]
    void RPC_ChangeTileData(int key, int land, int pension, int condo, int hotel)
    {
        if (_tileKey != key) return; // 자기 타일만 반응

        _tileLandOwner = land;
        _tilePensionOwner = pension;
        _tileCondoOwner = condo;
        _tileHotelOwner = hotel;
    }

    /// <summary>
    /// 타일 가격 가져오기
    /// </summary>
    public double GetPrice(int index)
    {
        switch (index)
        {
            case 0: return _tileLandPrice;
            case 1: return _tilePensionPrice;
            case 2: return _tileCondoPrice;
            case 3: return _tileHotelPrice;
            case 4: return _tileLandMarkPrice;
            default:
                Debug.LogWarning("잘못된 가격 인덱스 요청: " + index);
                return 0;
        }
    }

    /// <summary>
    /// 건물 인덱스별 소유주 반환
    /// </summary>
    public int GetOwner(int index)
    {
        switch (index)
        {
            case 0: return _tileLandOwner;
            case 1: return _tilePensionOwner;
            case 2: return _tileCondoOwner;
            case 3: return _tileHotelOwner;
            case 4: return _tileLandMarkOwner;
            default:
                Debug.LogWarning("잘못된 소유주 인덱스 요청: " + index);
                return -1;
        }
    }

    /// <summary>
    /// 건물 인덱스별 소유주 설정
    /// </summary>
    public void SetOwner(int index, int owner)
    {
        switch (index)
        {
            case 0: _tileLandOwner = owner; break;
            case 1: _tilePensionOwner = owner; break;
            case 2: _tileCondoOwner = owner; break;
            case 3: _tileHotelOwner = owner; break;
            case 4: _tileLandMarkOwner = owner; break;
            default:
                Debug.LogWarning("잘못된 소유주 인덱스 설정: " + index);
                break;
        }
    }
}