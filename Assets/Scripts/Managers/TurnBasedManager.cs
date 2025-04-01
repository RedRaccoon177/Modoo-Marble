using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TurnBasedManager : MonoBehaviourPun
{
    int _diceNumFirst;
    int _diceNumSecond;
    private int?[] diceResults = new int?[2]; // 주사위 2개라고 가정
    public GameObject _redDicePrefab;
    public GameObject _blueDicePrefab;
    DiceManager _redDice;
    DiceManager _blueDice;
    int _redDiceViewID;
    int _blueDiceViewID;

    //public int Dice()
    //{
    //    _diceNumFirst = Random.Range(1, 7);
    //    _diceNumSecond = Random.Range(1, 7);
    //    UIManagerP.instance.InvokeDiceNum(_diceNumFirst, _diceNumSecond);
    //    return _diceNumFirst + _diceNumSecond;
    //}

    private void Start()
    {
        // 방장이 생성, 보내기
        if (PhotonNetwork.IsMasterClient == true)
        {
            _redDice = PhotonNetwork.Instantiate("DiceRed",new Vector3(7,0,-7), Quaternion.identity,0, new object[] { 0 }).GetComponent<DiceManager>();
            _blueDice = PhotonNetwork.Instantiate("DiceBlue", new Vector3(8, 0, -8), Quaternion.identity, 0, new object[] { 1 }).GetComponent<DiceManager>();
            _redDiceViewID = _redDice.photonView.ViewID;
            _blueDiceViewID = _blueDice.photonView.ViewID;
            _redDice._dicePlayerMove += PlayerMove;
            _blueDice._dicePlayerMove += PlayerMove;
            photonView.RPC("testt",RpcTarget.Others, _redDiceViewID, _blueDiceViewID);
        }
    }
    [PunRPC]
    public void testt(int _redDiceView, int _blueView)
    {
        _redDice = PhotonView.Find(_redDiceView).GetComponent<DiceManager>();
        _blueDice = PhotonView.Find(_blueView).GetComponent<DiceManager>();
        _redDice._dicePlayerMove += PlayerMove;
        _blueDice._dicePlayerMove += PlayerMove;
    }

    public void Dice()
    {
        var a = Random.Range(1,7);
        var b = Random.Range(1,7);
        _redDice._photonView.RPC("DiceStart", RpcTarget.All, a);
        _blueDice._photonView.RPC("DiceStart", RpcTarget.All, b);
    }

    public void PlayerMove(int diceKey, int diceNum)
    {
        diceResults[diceKey] = diceNum;
    
        if ((diceResults[0] != null && diceResults[1] != null))
        {
            int total = diceResults[0].Value + diceResults[1].Value;
            Debug.Log("최종 주사위 결과!!!!!!!!!!!!!!!! : " + total);
            ServerIngamePlayer._players[TurnMgr.currentTurn].RpcMovePlayer(total);
            diceResults[0] = null;
            diceResults[1] = null;
        }
    }
}
