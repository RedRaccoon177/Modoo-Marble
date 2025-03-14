using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    int diceNum;
    int playerPosIndex;
    public MapController mapInfo;

    private void Start()
    {
        playerPosIndex = 0;
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Dice();
        //    StartCoroutine(MovePlayer(diceNum));
        //}
    }
    public int Dice()
    {
        diceNum = Random.Range(1, 7);
        Debug.Log("주사위 숫자 : " + diceNum);
        return diceNum;
    }

    IEnumerator MovePlayer(int num)
    {
        int count = 0;
        Debug.Log("플레이어 현재 위치 인덱스 : " + playerPosIndex);
        while (count < num)
        {
            if ((playerPosIndex + count) >= 31)
            {
                playerPosIndex -= 32;
            }
            count++;
            transform.position = mapInfo.grounds[playerPosIndex + count].transform.position;
            yield return new WaitForSeconds(1f);
        }
        playerPosIndex += count;
        mapInfo.GroudInfo(playerPosIndex);
    }
}
