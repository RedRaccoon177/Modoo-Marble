using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceNumText : MonoBehaviour
{
    [Header("주사위 숫자")] public TextMeshProUGUI _diceNum;
    void Awake()
    {
        
    }
    public void ChangeDiceText(int FirstDice , int SecondDice)
    {
        _diceNum.text = FirstDice + "+" + SecondDice + "=" + (FirstDice + SecondDice);
    }
}
