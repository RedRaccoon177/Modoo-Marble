// BonusCardUI.cs - UI 출력 전용
using JetBrains.Annotations;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusCardUI : MonoBehaviourPun
{
    public TextMeshProUGUI _name;
    public TextMeshProUGUI _description;
    public Button _closeButton;

    private void Awake()
    {
        UIManagerP.instance._bonusCardUI += SetData;
        _closeButton.onClick.AddListener(() => UIManagerP.instance.OffClickUI());
        _closeButton.onClick.AddListener(() => TurnMgr.Instance.endTurn());
    }

    public void SetData(TileController _data)
    {
        int randomOption = Random.Range(0, 4);
        _name.text = _data._tileName;
        photonView.RPC("ShowBonus", RpcTarget.All, randomOption);
    }

    [PunRPC]
    public void ShowBonus(int selectedOption)
    {
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