using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;

//이원형, 함승윤
public class MapManager : MonoBehaviourPun
{
    // 타일 종류를 저장하는 배열 (각 타일 타입에 해당하는 프리팹을 할당)
    [Header("타일 프리팹")]
    [SerializeField] GameObject _tilePrefab;

    // 생성된 타일 오브젝트를 저장하는 배열
    [Header("실제 땅")]
    [SerializeField] public GameObject[] _tiles = new GameObject[40];

    // 타일의 초기 데이터 (위치, 타입 등)
    [Header("타일 초기 데이터")]
    [SerializeField] public TileInfoData[] _tiledates = new TileInfoData[40];

    GameObject _cityname;
    GameObject _bonusStage;

    Quaternion _rotation = Quaternion.identity;
    Vector3 _localScale = Vector3.zero;

    void Start()
    {
        _tiles = new GameObject[40]; // 모든 클라이언트가 배열 초기화

        if (PhotonNetwork.IsMasterClient)
        {
            CreateMap(); // 방장만 타일 생성
        }
    }

    /// <summary>
    /// 맵을 생성하는 함수
    /// </summary>
    public void CreateMap()
    {
        for (int i = 0; i < _tiledates.Length; i++)
        {
            if (10 < i && i < 20)
            {
                _rotation = Quaternion.Euler(0f, 90f, 0f);
            }
            else if (20 < i && i < 30)
            {
                _rotation = Quaternion.Euler(0f, -180f, 0f);
            }
            else if (30 < i && i < 40)
            {
                _rotation = Quaternion.Euler(0f, -90f, 0f);
            }
            GameObject _temp = PhotonNetwork.Instantiate("Tile", _tiledates[i]._tilePos, _rotation);


            TileController tileScript = _temp.GetComponent<TileController>();
            if (tileScript != null)
            {
                tileScript.SetTileData(_tiledates[i]);
            }

            TileNameSetting(i, _temp);
            TileSetting(tileScript, _temp);

            _tiles[i] = _temp;
        }
    }

    public void TileNameSetting(int num, GameObject gameObject)
    {
        _cityname = gameObject.transform.GetChild(5).GetChild(0).gameObject;
        _bonusStage = gameObject.transform.GetChild(5).GetChild(1).gameObject;

        switch (gameObject.GetComponent<TileController>()._tileType)
        {
            case TileType.Start:
                _bonusStage.SetActive(true);
                _bonusStage.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<TileController>()._tileName;
                _cityname.SetActive(false);
                break;

            case TileType.Sea:
                _cityname.SetActive(true);
                _cityname.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<TileController>()._tileName;
                _bonusStage.SetActive(false);
                break;

            case TileType.Ground:
                _cityname.SetActive(true);
                _cityname.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<TileController>()._tileName;
                _bonusStage.SetActive(false);
                break;

            case TileType.Item:
                _cityname.SetActive(true);
                _cityname.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<TileController>()._tileName;
                _bonusStage.SetActive(false);
                break;

            case TileType.Island:
                _bonusStage.SetActive(true);
                _bonusStage.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<TileController>()._tileName;
                _cityname.SetActive(false);
                break;

            case TileType.Olympics:
                _bonusStage.SetActive(true);
                _bonusStage.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<TileController>()._tileName;
                _cityname.SetActive(false);
                break;

            case TileType.Travel:
                _bonusStage.SetActive(true);
                _bonusStage.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<TileController>()._tileName;
                _cityname.SetActive(false);
                break;

            case TileType.revenue:
                _cityname.SetActive(true);
                _cityname.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<TileController>()._tileName;
                _bonusStage.SetActive(false);
                break;

            case TileType.casino:
                _cityname.SetActive(true);
                _cityname.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<TileController>()._tileName;
                _bonusStage.SetActive(false);
                break;
        }
    }

    public void TileSetting(TileController tile, GameObject tiles)
    {
        switch ((int)tile._subTileType)
        {
            case 0:
                tile.InGameTilePrefabs[0].SetActive(true);
                break;

            case 1:
                tile.InGameTilePrefabs[1].SetActive(true);
                break;

            case 2:
                tile.InGameTilePrefabs[2].SetActive(true);
                break;

            case 3:
                tile.InGameTilePrefabs[6].gameObject.SetActive(true);
                break;

            case 4:
                tile.InGameTilePrefabs[7].SetActive(true);
                break;

            case 5:
                tile.InGameTilePrefabs[8].SetActive(true);
                break;

            case 6:
                tile.InGameTilePrefabs[9].SetActive(true);
                break;

            case 7:
                break;

            case 8:
                tile.InGameTilePrefabs[5].SetActive(true);
                break;

            case 9:
                tile.InGameTilePrefabs[7].SetActive(true);
                break;

            case 10:
                break;

            case 11:
                tile.InGameTilePrefabs[18].SetActive(true);
                break;

            case 12:
                tile.InGameTilePrefabs[26].SetActive(true);
                break;

            case 13:
                tile.InGameTilePrefabs[27].SetActive(true);
                break;

            case 14:
                tile.InGameTilePrefabs[28].SetActive(true);
                break;

            case 15:
                tile.InGameTilePrefabs[14].SetActive(true);
                break;

            case 16:
                tile.InGameTilePrefabs[15].SetActive(true);
                break;

            case 17:
                tile.InGameTilePrefabs[16].SetActive(true);
                break;

            case 18:
                tile.InGameTilePrefabs[22].SetActive(true);
                break;

            case 19:
                tile.InGameTilePrefabs[23].SetActive(true);
                break;

            case 20:
                tile.InGameTilePrefabs[24].SetActive(true);
                break;

            case 21:
                tile.InGameTilePrefabs[25].SetActive(true);
                break;

            case 22:
                tile.InGameTilePrefabs[20].SetActive(true);
                break;

            case 23:
                tile.InGameTilePrefabs[21].SetActive(true);
                break;

            case 24:
                tile.InGameTilePrefabs[29].SetActive(true);
                break;

            case 25:
                tile.InGameTilePrefabs[30].SetActive(true);
                break;

            case 26:
                tile.InGameTilePrefabs[31].SetActive(true);
                break;

            case 27:
                tile.InGameTilePrefabs[32].SetActive(true);
                break;

            case 28:
                tile.InGameTilePrefabs[21].SetActive(true);
                break;


        }
    }
}
