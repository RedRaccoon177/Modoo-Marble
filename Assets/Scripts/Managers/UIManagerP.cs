using System;
using UnityEngine;

public class UIManagerP : MonoBehaviour
{
    public static UIManagerP instance;
    [Header("토지 구매 UI")]
    public GameObject[] _tileBuyUI;

    [Header("클릭시 뜨는 UI")] 
    public GameObject[] _clickTileUI; 

    [Header("타일 UI 생성 될 곳")]
    [SerializeField] Transform _tileUIParent;
    [SerializeField] Transform canvus;
    public event Action<TileController> _clickChangeDataGround;
    public event Action<TileController> _clickChangeDataSea;
    public event Action<TileController> _DataUnfinished;
    public event Action<int,int> _diceNumEvent;
    public event Action<TileController> _buyChangeDataGround;
    public event Action<TileController> _buyChangeDataSea;


    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
    }
    private void Start()
    {
        CreateClickUI();    // 타일 클릭시 나타나는 UI 생성
        OffClickUI();       // 타일 클릭시 나타나는 UI 비활성화
        CreateBuyUI();      // 구매할 때 나타나는 UI 
        OffBuyUIPanel();    // 구매할 때 나타나는 UI 비활성화
    }
    public void OnBuyUIPanel(TileType _tileType)
    {
        OffBuyUIPanel();
        OnBuyUI(_tileType);
    }
    public void OffBuyUIPanel()
    {
        // 판넬 비활성화 전에 자식 객체들 먼저 비활성화
        foreach (var temp in _tileBuyUI)
        {
            temp.SetActive(false);
        }
        _tileUIParent.gameObject.SetActive(false);
    }
    public void CreateBuyUI()
    {
        for (int i=0; i< _tileBuyUI.Length; i++)
        {
            _tileBuyUI[i] = Instantiate(_tileBuyUI[i], _tileUIParent);
        }
    }
    public void OnBuyUI(TileType _tileType)
    {
        _tileUIParent.gameObject.SetActive(true);
        for (int i = 0; i < _clickTileUI.Length; i++)
        {
            if (_tileType == TileType.Ground)
            {
                _tileBuyUI[0].SetActive(true);
            }
            else if (_tileType == TileType.Sea)
            {
                _tileBuyUI[1].SetActive(true);
            }
            else
            {
                _tileBuyUI[2].SetActive(true);
            }
        }
    }
    public void InvokeBuyUI(TileController _currentTile,TileType _tileType)
    {
        if (_tileType == TileType.Ground)
        {
            _buyChangeDataGround.Invoke(_currentTile);
        }
        else if (_tileType == TileType.Sea)
        {
            _buyChangeDataSea.Invoke(_currentTile);
        }
        else
        {
            _DataUnfinished.Invoke(_currentTile);
        }
    }
    /// <summary>
    ///  타일 클릭시 나타나는 UI 생성
    /// </summary>
    public void CreateClickUI()
    {
        for (int i = 0; i < _clickTileUI.Length; i++)
        {
            _clickTileUI[i] = Instantiate(_clickTileUI[i],canvus);
        }
    }
    /// <summary>
    ///  타입 전해주면 그 타입의 ui 띄움 타일 클릭시 생성되는 UI
    /// </summary>
    /// <param name="_type"></param>
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
            else
            {
                _clickTileUI[2].SetActive(true);
            }
        }
    }
    /// <summary>
    ///  클릭 UI들 비활성화
    /// </summary>
    public void OffClickUI()
    {
        foreach (var temp in _clickTileUI)
        {
            temp.SetActive(false);
        }
    }
    /// <summary>
    ///  타입에 맞게 이벤트 실행, 이벤트 = 데이터 주입 이벤트
    /// </summary>
    /// <param name="_tileController"></param>
    /// <param name="tileType"></param>
    public void InvokeClickUI(TileController _currentTile, TileType tileType)
    {
        if (tileType == TileType.Ground)
        {
            _clickChangeDataGround?.Invoke(_currentTile);
        }
        else if (tileType == TileType.Sea)
        {
            _clickChangeDataSea?.Invoke(_currentTile);
        }
        else
        {
            _DataUnfinished.Invoke(_currentTile);
        }
    }
  
    /// <summary>
    /// 주사위 텍스트 변화 이벤트
    /// </summary>
    /// <param name="firstDice"></param>
    /// <param name="secondDice"></param>
    public void InvokeDiceNum(int firstDice, int secondDice)
    {
        _diceNumEvent?.Invoke(firstDice, secondDice);
    }

}
