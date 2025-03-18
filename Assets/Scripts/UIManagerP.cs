using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerP : MonoBehaviour
{
    [Header("토지 구매 UI")]
    public TileBuyUI _tileGroundBuyUI;

    [Header("관광지 구매 UI")]
    public TileSeaBuyUI _tileSeaBuyUI;

    [Header("타일 UI 생성될 곳")]
    [SerializeField] Transform _tileParent;

    [Header("클릭시 토지 UI")] public GameObject _clickUI; 


    void Start()
    {
        TileBuyUI _temp0 = Instantiate(_tileGroundBuyUI, _tileParent);
        TileSeaBuyUI _temp1 = Instantiate(_tileSeaBuyUI, _tileParent);
    }

    void Update()
    {
        
    }
}
