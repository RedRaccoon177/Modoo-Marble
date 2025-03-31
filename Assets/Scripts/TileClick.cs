using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    TileController _tileController;
    public GameObject _overPrefab;
    Vector3 _originalPos;
    float _pointertileUp;
    int _ClickEventType;
    bool _isHovered;
    private void Awake()
    {
        _isHovered = false;
        _tileController = GetComponent<TileController>();
        _ClickEventType = 0;
        _pointertileUp = 0.001f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 내 턴이고
        if (TurnMgr.currentTurn == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            var a = ServerIngamePlayer._players[PhotonNetwork.LocalPlayer.ActorNumber];
            if (a._isTravel == true) // 여행 상태라면
            {
                // 세계여행에서 또 세계여행으로 못가게
                if (_tileController._tileType != TileType.Travel)
                {
                    a._isTravelClickTile = true;
                    a._travelClickTileNum = _tileController._tileKey;
                    _ClickEventType = 1;
                }
            }
        }
        if (_ClickEventType == 0)
        {
            UIManagerP.instance.InvokeClickUI(_tileController, _tileController._tileType);
            UIManagerP.instance.OnClickUI(_tileController._tileType);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _originalPos =_overPrefab.transform.position;
        _overPrefab.transform.position = new Vector3(transform.position.x, transform.position.y + _pointertileUp, transform.position.z);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _overPrefab.transform.position = _originalPos;
    }
}
