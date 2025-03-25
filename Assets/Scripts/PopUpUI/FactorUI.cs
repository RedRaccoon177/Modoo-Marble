using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactorUI : MonoBehaviour
{
    [Header("인수 버튼")]
    public Button _factorBtn;
    [Header("취소 버튼")]
    public Button _cancelBtn;
    private void Awake()
    {
        _factorBtn.onClick.AddListener(() => UIManagerP.instance.OffFactorUI());
        _cancelBtn.onClick.AddListener(() => UIManagerP.instance.OffFactorUI());
    }
}
