using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이원형, 함승윤
public class MapManager : MonoBehaviour
{
    // 타일 종류를 저장하는 배열 (각 타일 타입에 해당하는 프리팹을 할당)
    [Header("타일 프리팹")]
    [SerializeField] GameObject _tilePrefab;

    //[Header("타일 구매 UI")]
    //[SerializeField] GameObject _tileBuyUI;
    //[Header("타일 UI 생성될 곳")]
    //[SerializeField] Transform _tileParent;

    // 생성된 타일 오브젝트를 저장하는 배열
    [Header("실제 땅")]
    [SerializeField] public GameObject[] _tiles = new GameObject[40];

    // 타일의 초기 데이터 (위치, 타입 등)
    [Header("타일 초기 데이터")]
    [SerializeField] TileInfoData[] _tiledates = new TileInfoData[40];

    void Start()
    {
        CreatMap();
    }
    //Vector3 tilePos;
    public void ChangeTilePos(int num, GameObject gameObject)
    {
        gameObject.transform.localScale = new Vector3(1.2f, 0.18f, 1.8f);
        if (10 < num && num < 20)
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (20 < num && num < 30)
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
        }
        else if (30 < num && num < 40)
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else if (num == 10 || num == 20 || num == 30 || num == 0)
        {
            gameObject.transform.localScale = new Vector3(1.8f, 0.18f, 1.8f);
            gameObject.transform.GetChild(4).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 맵을 생성하는 함수
    /// </summary>
    public void CreatMap()
    {
        for (int i = 0; i < _tiledates.Length; i++)
        {
            // 프리팹을 생성하고 위치를 설정
            GameObject _temp = Instantiate(_tilePrefab);

            ChangeTilePos(i, _temp);
            _temp.transform.position = _tiledates[i]._tilePos;
            // TileController를 가져와서 데이터 적용
            TileController tileScript = _temp.GetComponent<TileController>();

            if (tileScript != null)
            {
                tileScript.SetTileData(_tiledates[i]); // 데이터 적용
                TileSetting(tileScript, _temp); // 타일타입에 맞춰 자식 객체 활성화.
            }
            else
            {
                Debug.LogError($"TileController가 프리팹 {_tilePrefab.name} 안에 없습니다! 프리팹 확인 필요.");
            }

            // 생성된 타일을 배열에 저장
            _tiles[i] = _temp;
        }
    }

    void TileSetting(TileController tile, GameObject tiles)
    {
        switch((int)tile._subTileType)
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
