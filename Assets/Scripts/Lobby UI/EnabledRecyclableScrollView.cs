using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnabledRecyclableScrollView : Scroll<int>
{
    [SerializeField] private int _slotCount;

    private void Start()
    {
        List<int> dataList = new List<int>();

        for(int i = 0; i < _slotCount; i++)
        {
            dataList.Add(i);
        }

        Init(dataList);
    }
}
