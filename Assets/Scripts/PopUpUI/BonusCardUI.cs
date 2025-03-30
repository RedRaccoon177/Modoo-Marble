using System.Collections;
using TMPro;
using UnityEngine;

public class BonusCardUI : MonoBehaviour
{
    public GameObject _panel;
    public TMP_Text _description;

    public void ShowCard(BonusCardType type)
    {
        _panel.SetActive(true);

        switch (type)
        {
            case BonusCardType.GetMoney:
                _description.text = "ex) 30만원을 획득했습니다!";
                break;
            case BonusCardType.LoseMoney:
                _description.text = "ex) 15만원을 잃었습니다!";
                break;
        }

        StartCoroutine(CloseAfterDelay(3f)); // 3초 뒤 자동 종료
    }

    
    IEnumerator CloseAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _panel.SetActive(false);
    }
}
