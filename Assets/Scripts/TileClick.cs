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
        Debug.Log("클릭 활성화");
        UIManagerP.instance.InvokeClickUI(_tileController, _tileController._tileType);
        UIManagerP.instance.OnClickUI(_tileController._tileType);
    }

}
