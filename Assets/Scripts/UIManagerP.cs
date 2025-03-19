using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerP : MonoBehaviour
{
    public static UIManagerP instance;
    [Header("토지 구매 UI")]
    public TileBuyUI _tileGroundBuyUI;
    [Header("광광지 구매 UI")]
    public TileSeaBuyUI _tileSeaBuyUI;
    [Header("타일 UI 생성 될 곳")]
    [SerializeField] Transform _tileUIParent;
    [SerializeField] Transform canvus;
    [Header("클릭시 뜨는 UI")] public GameObject[] _clickTileUI; 
    public event Action<TileController> _clickEvenetGround;
    public event Action<TileController> _clickEvenetSea;


    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
    }
    private void Start()
    {
        CreateUI();
        CreateClickUI();
        OffBuyUIPanel();
    }
    public void OnBuyUIPanel()
    {
        _tileUIParent.gameObject.SetActive(true);
    }
    public void OffBuyUIPanel()
    {
        _tileUIParent.gameObject.SetActive(false);
    }
    public void CreateUI()
    {
        var temp = Instantiate(_tileGroundBuyUI, _tileUIParent);
    }
    public void CreateClickUI()
    {
        for (int i = 0; i < _clickTileUI.Length; i++)
        {
            _clickTileUI[i] = Instantiate(_clickTileUI[i],canvus);
            _clickTileUI[i].SetActive(false);
        }
    }

    public void OnClickUI(TileType _type)
    {
        OffClickUI();
        for (int i = 0; i < _clickTileUI.Length; i++)
        {
            if (_type == TileType.Ground)
            {
                _clickTileUI[0].SetActive(true);
            }
            else if(_type == TileType.Sea)
            {
                _clickTileUI[1].SetActive(true);
            }
        }
    }
    public void OffClickUI()
    {
        foreach (var temp in _clickTileUI)
        {
            temp.SetActive(false);
        }
    }


    public void InvokeClickUI(TileController _tileController, TileType tileType)
    {
        if (tileType == TileType.Ground)
        {
            _clickEvenetGround?.Invoke(_tileController);
        }
        else if (tileType == TileType.Sea)
        {
            _clickEvenetSea?.Invoke(_tileController);
        }
    }
}
