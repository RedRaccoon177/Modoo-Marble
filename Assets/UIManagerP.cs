using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerP : MonoBehaviour
{
    [Header("토지 UI")]
    public GameObject _buyUI;
    [Header("클릭시 토지 UI")] public GameObject[] _clickUI; 
    [Header("클릭시 토지 UI")] public GameObject[] _tempClickUI;
    public static UIManagerP instance;
    public Transform canvas;
    public event Action<TileController> _clickEvenetGround;


    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(instance);
        }
    }
    private void Start()
    {
        CreateClickUI();
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
        for (int i = 0; i < 1; i++)
        {
            Instantiate(_clickUI[i],canvas);
        }
    }

    public void OnPopupGround(TileController _tileController)
    {
        _clickEvenetGround?.Invoke(_tileController);
    }
}
