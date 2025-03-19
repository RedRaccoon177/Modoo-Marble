using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileClick : MonoBehaviour, IPointerClickHandler
{
    TileController _tileController;

    private void Awake()
    {
        _tileController = GetComponent<TileController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManagerP.instance.InvokeClickUI(_tileController, _tileController._tileType);
        UIManagerP.instance.OnClickUI(_tileController._tileType);
    }

}
