using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DiceManager : MonoBehaviourPun
{
    bool isGroundOn;
    Rigidbody _rb;
    Coroutine _diceCor;
    public PhotonView _photonView;
    public int _diceNum { get; private set; }
    Quaternion one = Quaternion.Euler(-90, 0, 0);
    Quaternion two = Quaternion.Euler(0, 0, 0);
    Quaternion three = Quaternion.Euler(0, 90, -90);
    Quaternion four = Quaternion.Euler(180, 0, -90);
    Quaternion five = Quaternion.Euler(180, 0, 0);
    Quaternion six = Quaternion.Euler(90, 0, 0);

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
    }
    [PunRPC]
    public void DiceStart()
    {
        _diceNum = Random.Range(1, 7);
        _rb.AddForce(0, 10, 0, ForceMode.Impulse);
        _diceCor = StartCoroutine(RollingDice());
    }

    IEnumerator RollingDice()
    {
        isGroundOn = true;
        while (isGroundOn == true)
        {
            transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "DiceGround")
        {
            isGroundOn = false;
            StopCoroutine(RollingDice());
            ChangeRotation(_diceNum);
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

    public int RandomDiceNum()
    {
        return _diceNum = Random.Range(1, 7);
    }

}
