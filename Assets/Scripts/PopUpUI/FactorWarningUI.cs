using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactorWarningUI : MonoBehaviour
{
    [Header("확이 버튼")]
    public Button _closeBtn;
    private void Awake()
    {
        _closeBtn.onClick.AddListener(() => UIManagerP.instance.OffFactorWarningUI());
        _closeBtn.onClick.AddListener(() => TurnMgr.Instance.endTurn());
    }
}
