using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class DiceManager : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    bool _isRolling;
    public event Action<int, int> _dicePlayerMove;
    public int diceID;
    bool isGroundOn;
    Rigidbody _rb;
    Coroutine _diceCor;
    public PhotonView _photonView;
    public int _diceNum;
    Quaternion one = Quaternion.Euler(-90, 0, 0);
    Quaternion two = Quaternion.Euler(0, 0, 0);
    Quaternion three = Quaternion.Euler(0, 90, -90);
    Quaternion four = Quaternion.Euler(180, 0, -90);
    Quaternion five = Quaternion.Euler(180, 0, 0);
    Quaternion six = Quaternion.Euler(90, 0, 0);

    private void Start()
    {
        _isRolling = false;
        _photonView = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
    }
    [PunRPC]
    public void DiceStart(int _diceNum)
    {
        this._diceNum = 0;
        this._diceNum = _diceNum;
        _isRolling = true;
        _rb.AddForce(0, 5, 0, ForceMode.Impulse);
        _diceCor = StartCoroutine(RollingDice());
    }

    IEnumerator RollingDice()
    {
        while (_isRolling == true)
        {
            transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "DiceGround" && _isRolling == true)
        {
            _isRolling = false;
            StopCoroutine(RollingDice());
            _rb.velocity = Vector3.zero;
            ChangeRotation(_diceNum);
            _dicePlayerMove?.Invoke(diceID, _diceNum);
        }
    }

    public void ChangeRotation(int num)
    {
        if (num == 1) { transform.rotation = one; }
        else if (num == 2) { transform.rotation = two; }
        else if (num == 3) { transform.rotation = three; }
        else if (num == 4) { transform.rotation = four; }
        else if (num == 5) { transform.rotation = five; }
        else if (num == 6) { transform.rotation = six; }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            diceID = (int)data[0];
        }
    }
}
