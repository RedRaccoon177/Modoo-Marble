using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactorUI : MonoBehaviour
{
    [Header("인수 버튼")]
    public Button _factorBtn;
    [Header("취소 버튼")]
    public Button _cancelBtn;
    [Header("플레이어 돈 텍스트")]
    public TextMeshProUGUI _currentTileName;
    [Header("인수 지역 텍스트")]
    public TextMeshProUGUI _playerMoney;
    [Header("인수 비용텍스트")]
    public TextMeshProUGUI _buyPrice; // 비용

    public TileController _currentTile; // 현재 타일
    public ServerIngamePlayer _currentPlayer; // 나
    public ServerIngamePlayer _targetPlayer; // 땅 주인
    bool _skipNextClick;

    double _totalBuyPrice; // 토지 건물 추가 건설 금액
    int _firstZeroOwner; // 토지 건물의 은행 소유 찾기(예외처리용)

    public void SetData()
    {
        // 토지의 총 금액 계산 (소유 = 은행이 아닌)
        _totalBuyPrice = _currentTile.TotalBuyPrice(_currentTile);
        _playerMoney.text = _currentPlayer.GetMoney().ToString();
        _buyPrice.text = _totalBuyPrice.ToString();
        _currentTileName.text = _currentTile._tileName;
        _skipNextClick = false;
        _firstZeroOwner = 0;
    }

    private void Awake()
    {
        _factorBtn.onClick.RemoveAllListeners();
        _factorBtn.onClick.AddListener(() => {
            Factor(); // 인수 처리
            UIManagerP.instance.OffFactorUI(); // UI 닫기
            StartBuyProcess(); // 구매 UI 띄우기
        });
        _cancelBtn.onClick.AddListener(() => UIManagerP.instance.OffFactorUI());
        _cancelBtn.onClick.AddListener(() => PlayerMoveTest.Instance.endTurn());
    }

    private void StartBuyProcess()
    {
        if (_skipNextClick == false)
        {
            UIManagerP.instance.OnBuyUI(TileType.Ground);
            UIManagerP.instance.InvokeBuyUI(_currentTile, TileType.Ground);
        }
    }

    public void Factor()
    {
        for (int i = 0; i < 4; i++)
        {
            if (_firstZeroOwner <_currentTile.GetOwner(i))
            {
                _firstZeroOwner = _currentTile.GetOwner(i);
            }
        }
        // 건물 하나라도 인수 안되면 추가 구매 창 안뛰움
        if(_currentPlayer.GetMoney() < _currentTile.GetPrice(_firstZeroOwner))
        {
            // 타일의 0이아닌 첫번 째 건물 찾기
            _skipNextClick = true;
        }
        // 0이아닌 첫 번째 건물`````````````````````````````````````````````````````` 
        // 주인에게 건설비용 돌려주기
        _currentPlayer.photonView.RPC("DecreaseMoney", RpcTarget.All, _totalBuyPrice);
        _currentPlayer.photonView.RPC("AddPlayerOwnerTileList", RpcTarget.All, _currentTile.photonView.ViewID);
        Debug.Log($" {_currentPlayer._playerNum} : 건설 비용 빠져 나간 돈 : " + _totalBuyPrice);

        // 새로운 주인에게 비용 부과
        _targetPlayer.photonView.RPC("IncreaseMoney", RpcTarget.All, _totalBuyPrice);
        _targetPlayer.photonView.RPC("MinusPlayerOwnerTileList", RpcTarget.All, _currentTile.photonView.ViewID);
        Debug.Log($" {_targetPlayer._playerNum} : 건설 비용 빠져 나간 돈 : " + _totalBuyPrice);

        for (int i=0; i< 4; i ++)
        {
            if (_currentTile.GetOwner(i) != 0)
            {
                _currentTile.photonView.RPC("SetOwner",RpcTarget.All, i, _currentPlayer._playerNum);
            }
        }

        _currentPlayer.photonView.RPC("TotalMoney", RpcTarget.All);
        _targetPlayer.photonView.RPC("TotalMoney", RpcTarget.All);
    }   
}
