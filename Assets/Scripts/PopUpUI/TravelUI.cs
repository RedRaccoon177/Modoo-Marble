using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TravelUI : MonoBehaviour
{
    public Button _cancleBtn;
    private void Start()
    {
        _cancleBtn.onClick.AddListener(() => UIManagerP.instance.OffTravelUI());
        _cancleBtn.onClick.AddListener(() => TurnMgr.Instance.endTurn());
    }

}
