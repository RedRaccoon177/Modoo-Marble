using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnFinishedTileClick : MonoBehaviour
{
    public TextMeshProUGUI _name;
    public Button _closeButton;

    private void Awake()
    {
        UIManagerP.instance._DataUnfinished += SetData;
        _closeButton.onClick.AddListener(() => UIManagerP.instance.OffClickUI());
        _closeButton.onClick.AddListener(() => TurnMgr.Instance.endTurn());
    }

    public void SetData(TileController _data)
    {
        _name.text = _data._tileName;
    }
}
