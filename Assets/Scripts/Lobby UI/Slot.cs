using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : RecyclableScrollSlot<int>
{
    [SerializeField] private TextMeshProUGUI _text; // 슬롯에 표시할 텍스트 ui

    public override void Init()
    {
    }

    public override void UpdateSlot(int data)
    {
        _text.text = data.ToString();
    }
}
