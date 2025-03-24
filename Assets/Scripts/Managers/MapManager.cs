using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

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

    void Start()
    {
        _tiles = new GameObject[40]; // 모든 클라이언트가 배열 초기화

        if (PhotonNetwork.IsMasterClient)
        {
            CreateMap(); // 방장만 타일 생성
        }
    }

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
    public void CreateMap()
    {
        for (int i = 0; i < _tiledates.Length; i++)
        {
            GameObject _temp = PhotonNetwork.Instantiate("Tile", _tiledates[i]._tilePos, Quaternion.identity);

            ChangeTilePos(i, _temp);

            TileController tileScript = _temp.GetComponent<TileController>();
            if (tileScript != null)
            {
                tileScript.SetTileData(_tiledates[i]);
            }

            _tiles[i] = _temp;
        }
    }
}
