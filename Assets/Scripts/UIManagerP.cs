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


    [SerializeField] Transform _tileParent;


    [Header("클릭시 토지 UI")] public GameObject _clickUI;
    [Header("클릭시 관광지 UI")] public GameObject TouristClickUI;

    public event Action<TileController> _clickEvenetGround;


    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
    }
    private void Start()
    {
        CreateClickUI();
        ClickTouristUI();
        TileBuyUI _temp0 = Instantiate(_tileGroundBuyUI, _tileParent);
    }
    #region
    //public void CreateClickUI()
    //{
    //
    //    for (int i= 0; i < _ClickUI.Length; i ++)
    //    {
    //        string key = "";
    //        if (i == 0) { key = "Ground"; }
    //        if (i == 1) { key = "Sea"; }
    //        _tempClickUI[key] = Instantiate(_ClickUI[i]);
    //    }
    //}
    //public void OnClickUI(string key,TileController tileData)
    //{
    //    _tempClickUI[key].SetActive(true);
    //}
    #endregion
    public void CreateClickUI()
    {
        Instantiate(_clickUI, _tileParent);
    }

    public void ClickTouristUI()
    {
        Instantiate(TouristClickUI, _tileParent);
    }

    public void OnPopupGround(TileController _tileController)
    {
        _clickEvenetGround?.Invoke(_tileController);
    }
}
