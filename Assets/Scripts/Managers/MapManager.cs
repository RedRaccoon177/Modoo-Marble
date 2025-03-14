using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//이원형, 함승윤
public class MapManager : MonoBehaviour
{
    // 타일 종류를 저장하는 배열 (각 타일 타입에 해당하는 프리팹을 할당)
    [Header("타일 타입")]
    [SerializeField] GameObject[] _tilesType = new GameObject[7];

    // 생성된 타일 오브젝트를 저장하는 배열
    [Header("실제 땅")]
    [SerializeField] public GameObject[] _grounds = new GameObject[32];

    // 타일의 초기 데이터 (위치, 타입 등)
    [Header("타일 초기 데이터")]
    [SerializeField] TileInfoData[] _dates = new TileInfoData[32];

    // 타일 타입과 프리팹을 연결하는 자료 구조
    private Dictionary<TileType, GameObject> _tilePrefabs;

    void Start()
    {
        // Dictionary를 생성하여 각 타일 타입에 해당하는 프리팹을 매핑
        _tilePrefabs = new Dictionary<TileType, GameObject>
        {
            { TileType.Ground, _tilesType[0] },   // 일반 땅 타일
            { TileType.Sea, _tilesType[1] },      // 바다 타일
            { TileType.Item, _tilesType[2] },     // 아이템 타일
            { TileType.Island, _tilesType[3] },   // 섬 타일
            { TileType.Start, _tilesType[4] },    // 시작 지점 타일
            { TileType.Olympics, _tilesType[5] }, // 올림픽 타일
            { TileType.Travel, _tilesType[6] }    // 여행 타일
        };

        CreatMap();
    }

    // 맵을 생성하는 함수
    public void CreatMap()
    {
        // 타일 데이터를 기반으로 맵을 구성
        for (int i = 0; i < _dates.Length; i++)
        {
            // 현재 타일의 타입을 가져와서 Dictionary에서 해당 프리팹을 찾음
            if (_tilePrefabs.TryGetValue(_dates[i].tileType, out GameObject prefab))
            {
                // 프리팹을 생성하고 위치를 설정
                GameObject _temp = Instantiate(prefab);
                _temp.transform.position = _dates[i].tilePos;

                // 생성된 타일을 배열에 저장
                _grounds[i] = _temp;
            }
        }
    }
}
