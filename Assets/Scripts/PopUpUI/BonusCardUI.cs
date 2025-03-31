using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusCardUI : MonoBehaviour
{
    public TextMeshProUGUI _name;
    public TextMeshProUGUI _description;
    public Button _closeButton;

    private void Awake()
    {
        BonusCardManager.instance._bonusCardUI += ShowBonus;
        _closeButton.onClick.AddListener(() => UIManagerP.instance.OffClickUI());
        _closeButton.onClick.AddListener(() => TurnMgr.Instance.endTurn());
    }

    public void ShowBonus(int selectedOption)
    {
        _name.text = "보너스 카드";
        Debug.Log("보너스 카드 선택지: " + selectedOption);

        switch (selectedOption)
        {
            case 0:
                _description.text = "돈을 받았습니다!";
                break;
            case 1:
                _description.text = "돈을 잃었습니다!";
                break;
            case 2:
                _description.text = "한 칸 앞으로 이동!";
                break;
            case 3:
                _description.text = "턴이 스킵됩니다!";
                break;
        }
    }
}