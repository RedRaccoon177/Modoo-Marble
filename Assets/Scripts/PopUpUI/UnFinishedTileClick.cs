using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public TextMeshProUGUI _name;
    public Button _closeButton;

    private void Awake()
    {
        UIManagerP.instance._DataUnfinished += SetData;
        _closeButton.onClick.AddListener(() => UIManagerP.instance.OffClickUI());
    }

    public void SetData(TileController _data)
    {
        _name.text = _data._tileName;
    }
}
