using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    public PlayerTestMw player;
    [SerializeField] Text moneyText;
    [SerializeField] Text totalMoneyText;
    [SerializeField] Text rankText;

    private void Awake()
    {
        if (player != null)
            player.OnDataChanged += UpdateUI;
    }

    private void OnDisable()
    {
        if (player != null)
            player.OnDataChanged -= UpdateUI; 
    }

    private void Start()
    {
        UpdateUI(); 

    }

    private void UpdateUI()
    {
        if (player == null) return;

        moneyText.text = $"현금: {player.Money}원";
        totalMoneyText.text = $"총자산: {player.TotalMoney}원";
        rankText.text = $"{player.Rank}위";
    }
}
