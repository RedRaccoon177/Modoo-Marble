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

    public void ShowBonus(int selectedOption, double money)
    {
        _name.text = "보너스 카드";
        Debug.Log("보너스 카드 선택지: " + selectedOption);

        switch (selectedOption)
        {
            case 0:
                _description.text = "축하드립니다. 로또에 당첨되어 " + money + " G를 획득하셨습니다.";
                break;
            case 1:
                _description.text = "납세의 의무를 성실하게 수행하여 " + money + "G를 납세하셨습니다.";
                break;
            case 2:
                _description.text = "축하드립니다. 처음으로 돌아가세요!";
                break;
            case 3:
                _description.text = "턴이 스킵됩니다!";
                break;
        }
    }
}