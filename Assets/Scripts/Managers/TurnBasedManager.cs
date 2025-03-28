using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TurnBasedManager : MonoBehaviour
{
    int _diceNumFirst;
    int _diceNumSecond;

    public GameObject _redDicePrefab;
    public GameObject _blueDicePrefab;
    DiceManager _redDice;
    DiceManager _blueDice;

    //public int Dice()
    //{
    //    _diceNumFirst = Random.Range(1, 7);
    //    _diceNumSecond = Random.Range(1, 7);
    //    UIManagerP.instance.InvokeDiceNum(_diceNumFirst, _diceNumSecond);
    //    return _diceNumFirst + _diceNumSecond;
    //}

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            _redDice = PhotonNetwork.Instantiate("DiceRed",new Vector3(7,0,-7), Quaternion.identity).GetComponent<DiceManager>();
            _blueDice = PhotonNetwork.Instantiate("DiceBlue", new Vector3(8, 0, -8), Quaternion.identity).GetComponent<DiceManager>();
        }
    }

    public int Dice()
    {
        _redDice._photonView.RPC("DiceStart", RpcTarget.All);
        _blueDice._photonView.RPC("DiceStart", RpcTarget.All);
        return _redDice._diceNum + _blueDice._diceNum;
    }
}
