using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event Action OnDataChanged; 

    [SerializeField] private string nickname;//임시값
    private int money = 2000000; //임시값
    private int extraMoney = 0;

    public string Nickname => nickname;
    public int Money => money;
    public int TotalMoney => money + extraMoney; 
    public int Rank { get; private set; }

    public void UpdateMoney(int newMoney)
    {
        money = newMoney;
        OnDataChanged?.Invoke(); 
    }

    public void UpdateExtraAssets(int newExtraAssets)
    {
        extraMoney = newExtraAssets;
        OnDataChanged?.Invoke(); 
    }

    public void UpdateRank(int newRank)
    {
        Rank = newRank;
        OnDataChanged?.Invoke(); 
    }
}
