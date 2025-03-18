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
    [Header("클릭시 뜨는 토지 UI")] public GameObject _clickGroundUI; 
    [Header("클릭시 뜨는 관광지 UI")] public GameObject _clickSeaUI; 
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
        CreateUI();
        OffBuyUIPanel();
        OffClickUIPanel();
    }
    public void OnBuyUIPanel()
    {
        _tileUIParent.gameObject.SetActive(true);
    }
    public void OffBuyUIPanel()
    {
        _tileUIParent.gameObject.SetActive(false);
    }
    public void OnClcikUIPanel()
    {
        _clickGroundUI.SetActive(true);
    }
    public void OffClickUIPanel()
    {
        _clickGroundUI.SetActive(false);
    }
    public void CreateUI()
    {
        var temp = Instantiate(_tileGroundBuyUI, _tileUIParent);
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
        _clickGroundUI = Instantiate(_clickGroundUI, canvus);
    }

    public void OnPopupGround(TileController _tileController)
    {
        _clickEvenetGround?.Invoke(_tileController);
    }
}
