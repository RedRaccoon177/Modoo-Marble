using Photon.Pun;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class TileController : MonoBehaviourPun
{
    [Header("보조 타일 타입")] public SubTileType _subTileType;
    [Header("보조 타일 타입")] public UnderTilePrefab subUnderTilePrefab;
    [Header("보조 타일 타입")] public GameObject subTileParent;
    [Header("보조 타일 타입")] public GameObject UnderPrefab;
    [Header("보조 타일 타입")] public GameObject BuildPostions;
    [Header("보조 타일 타입")] public GameObject[] InGameTilePrefabs;

    [Header("건물 활성화를 위한 변수")]
    public GameObject _tileLandObj;
    public GameObject _tilePensionObj;
    public GameObject _tileCondoObj;
    public GameObject _tileHotelObj;

    [Header("메터리얼 배열")] public Material[] materials;
    Material _currentMat;
    MeshRenderer _tileLandMat;
    MeshRenderer _tilePensionMat;
    MeshRenderer _tileCondoMat;
    MeshRenderer _tileHotelMat;
}

public partial class TileController : MonoBehaviourPun
{
    [Header("타일 키값")] public int _tileKey;
    [Header("타일 이름")] public string _tileName;
    [Header("타일 위치")] public Vector3 _tilePos;
    [Header("타일 크기")] public Vector3 _tileLocalScale;
    [Header("타일 타입")] public TileType _tileType;

    [Header("타일 땅 소유주 및 관광지 1개 소유주")] public int _tileLandOwner;
    [Header("타일 1번 건물 및 관광지 2개 소유주")] public int _tilePensionOwner;
    [Header("타일 2번 건물 및 관광지 3개 소유주")] public int _tileCondoOwner;
    [Header("타일 3번 건물 및 관광지 4개 소유주")] public int _tileHotelOwner;
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
    [Header("총 통행료")] public double _totalTollPrice;

    GameObject _ground;
    GameObject _Sea;

    GameObject _cityname;
    GameObject _bonusStage;

    
    void Start()
    {
        StartCoroutine(SetupTileIfNotMaster());
        _tileLandMat = _tileLandObj.GetComponent<MeshRenderer>();
        _tilePensionMat = _tilePensionObj.GetComponent<MeshRenderer>();
        _tileCondoMat = _tileCondoObj.GetComponent<MeshRenderer>();
        _tileHotelMat = _tileHotelObj.GetComponent<MeshRenderer>();
    }

    public void ChangeMat(int _index)
    {
        if (_index == 0) { _tileLandMat.material = _currentMat; }
        else if (_index == 1) { _tilePensionMat.material = _currentMat; }
        else if(_index == 2) { _tileCondoMat.material = _currentMat; }
        else if(_index == 3) { _tileHotelMat.material = _currentMat; }
    }

    public void GetPlayerMat(int _playerNum)
    {
        switch (_playerNum)
        {
            case 1:
                _currentMat = materials[0];
                break;
            case 2:
                _currentMat = materials[1];
                break;
            case 3:
                _currentMat = materials[2];
                break;
            case 4:
                _currentMat = materials[3];
                break;
        }
    }

    public void ActiveObj()
    {
        if(_tileType == TileType.Ground)
        {
            // 땅만 사고 건물 안샀을 때
            if (_tileLandOwner != 0 && _tilePensionOwner == 0 && _tileCondoOwner == 0 && _tileHotelOwner == 0) _tileLandObj.SetActive(true);
            else _tileLandObj.SetActive(false);

            if (_tilePensionOwner != 0) _tilePensionObj.SetActive(true);
            else _tilePensionObj.SetActive(false);
            
            if (_tileCondoOwner != 0) _tileCondoObj.SetActive(true);
            else _tileCondoObj.SetActive(false);
            
            if (_tileHotelOwner != 0) _tileHotelObj.SetActive(true);
            else _tileHotelObj.SetActive(false);
        }
        else if (_tileType == TileType.Sea)
        {
            if(_tileLandOwner != 0) _tileLandObj.SetActive(true);
            else _tileLandObj.SetActive(false);
        }
    }

    IEnumerator SetupTileIfNotMaster()
    {
        yield return new WaitForSeconds(0.5f);

        if (!PhotonNetwork.IsMasterClient)
        {
            MapManager map = FindObjectOfType<MapManager>();

            if (map != null)
            {
                foreach (var data in map._tiledates)
                {
                    if (Vector3.Distance(transform.position, data._tilePos) < 0.1f)
                    {
                        SetTileData(data); // 참가자도 타일 정보 직접 설정
                        Debug.Log($"[참가자] SetTileData 직접 호출됨 → tileKey: {data._tileKey}");

                        // 타일 배열 등록
                        if (data._tileKey >= 0 && data._tileKey < map._tiles.Length)
                        {
                            map._tiles[data._tileKey] = this.gameObject;
                            map.TileNameSetting(data._tileKey, this.gameObject);
                            map.TileSetting(this, this.gameObject);
                            Debug.Log($"[참가자] _tiles[{data._tileKey}] 등록 완료");
                        }

                        break;
                    }
                }
            }
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
        transform.localScale= data._tileLocalScale;
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
    public double GetTollPrice(int index)
    {
        switch (index)
        {
            case 0: return _tileLandToll;
            case 1: return _tilePensionToll;
            case 2: return _tileCondoToll;
            case 3: return _tileHotelToll;
            case 4: return _tileLandMarkToll;
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
    [PunRPC]
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
        GetPlayerMat(owner);
        ChangeMat(index);
        ActiveObj();
    }

    public double TotalTollPrice(TileController _tileController)
    {
        _totalTollPrice = 0;
        if (_tileLandOwner != 0){ _totalTollPrice += _tileLandToll; }
        if (_tilePensionOwner != 0) { _totalTollPrice += _tilePensionToll; }
        if (_tileCondoOwner != 0) { _totalTollPrice += _tileCondoToll; }
        if (_tileHotelOwner != 0) { _totalTollPrice += _tileHotelToll; }
        return _totalTollPrice;
    }
    public double TotalBuyPrice(TileController _tileController)
    {
        _totalTollPrice = 0;
        if (_tileLandOwner != 0) { _totalTollPrice += _tileLandPrice; }
        if (_tilePensionOwner != 0) { _totalTollPrice += _tilePensionPrice; }
        if (_tileCondoOwner != 0) { _totalTollPrice += _tileCondoPrice; }
        if (_tileHotelOwner != 0) { _totalTollPrice += _tileHotelPrice; }
        return _totalTollPrice;
    }

}