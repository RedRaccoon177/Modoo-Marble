using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileClickUi : MonoBehaviour, IPointerClickHandler
{
    TileController _tileController;

    private void Awake()
    {
        _tileController = GetComponent<TileController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("타일 이름 : " + _tileController._tileName);
        Debug.Log("타일 타입 : " + _tileController._tileType);
        Debug.Log("땅 소유주 : " + _tileController._tileLandOwner);
        Debug.Log("타일 땅 가격 : " + _tileController._tileLandPrice);
    }

}
