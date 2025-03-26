using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class FactorUI : MonoBehaviour
{
    [Header("인수 버튼")]
    public Button _factorBtn;
    [Header("취소 버튼")]
    public Button _cancelBtn;
    public TileController _currentTile;
    public ServerIngamePlayer _player;




    private void Awake()
    {
        _factorBtn.onClick.AddListener(() => UIManagerP.instance.OffFactorUI());
        _factorBtn.onClick.AddListener(Factor);
        _factorBtn.onClick.AddListener(StartBuyProcess);
        _cancelBtn.onClick.AddListener(() => UIManagerP.instance.OffFactorUI());
    }

    private void StartBuyProcess()
    {
        UIManagerP.instance.OnBuyUI(TileType.Ground);
        UIManagerP.instance.InvokeBuyUI(_currentTile, TileType.Ground);
    }


    public void Factor()
    {
        for (int i=0; i< 4; i ++)
        {
            if (_currentTile.GetOwner(i) != 0)
            {
                _currentTile.photonView.RPC("SetOwner",RpcTarget.All, i,_player._playerNum);
            }
        }
    }


}
