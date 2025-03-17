using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이원형, 함승윤
public class MapManager : MonoBehaviour
{
    // 타일 종류를 저장하는 배열 (각 타일 타입에 해당하는 프리팹을 할당)
    [Header("타일 프리팹")]
    [SerializeField] GameObject _tilePrefab;

    [Header("타일 구매 UI")]
    [SerializeField] GameObject _tileBuyUI;

    [Header("타일 UI 생성될 곳")]
    [SerializeField] Transform _tileParent;

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

    /// <summary>
    /// 맵을 생성하는 함수
    /// </summary>
    public void CreatMap()
    {
        for (int i = 0; i < _tiledates.Length; i++)
        {
            // 프리팹을 생성하고 위치를 설정
            GameObject _temp = Instantiate(_tilePrefab);
            GameObject _temp2 = Instantiate(_tileBuyUI, _tileParent);

            _temp.transform.position = _tiledates[i]._tilePos;

            // TileController를 가져와서 데이터 적용
            TileController tileScript = _temp.GetComponent<TileController>();
            if (tileScript != null)
            {
                tileScript.SetTileData(_tiledates[i]); // 데이터 적용
            }
            else
            {
                Debug.LogError($"TileController가 프리팹 {_tilePrefab.name} 안에 없습니다! 프리팹 확인 필요.");
            }

            // 생성된 타일을 배열에 저장
            _tiles[i] = _temp;
        }
    }

}
