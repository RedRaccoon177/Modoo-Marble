using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController: MonoBehaviour
{
    public GameObject tileGround;
    public GameObject tileSea;
    public GameObject tileItem;
    public GameObject tileTravel;
    public GameObject tileOlympics;
    public GameObject tileStart;
    public GameObject tileIsland;

    // 구매 가능한 토지만 저장할지 , 모든 타일들을 저장할지
    public GameObject[] grounds = new GameObject[32];
    public TileInfoData[] dates;
    public GameObject temp;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            CreatMap();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            //GroudInfo();
        }
    }
    public void CreatMap()
    {
        for (int i = 0; i < 32; i++)
        {
            if (dates[i].tileType == TileType.Ground) 
            {
                temp = Instantiate(tileGround);
            }
            else if (dates[i].tileType == TileType.Sea)
            {
                temp = Instantiate(tileSea);
            }
            else if (dates[i].tileType == TileType.Item)
            {
                temp =Instantiate(tileItem);
            }
            else if (dates[i].tileType == TileType.Island)
            {
                temp = Instantiate(tileIsland);
            }
            else if (dates[i].tileType == TileType.Start)
            {
                temp = Instantiate(tileStart);
            }
            else if (dates[i].tileType == TileType.Olympics)
            {
                temp = Instantiate(tileOlympics);
            }
            else if (dates[i].tileType == TileType.Travel)
            {
                temp = Instantiate(tileTravel);
            }
            temp.transform.position = dates[i].tilePos;
            grounds[i] = temp;
        }
    }
    public GameObject GroudInfo(int num)
    {
        return grounds[num];
    }
}
