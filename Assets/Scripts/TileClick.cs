using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    TileController _tileController;
    int _ClickEventType;

    private void Awake()
    {
        _tileController = GetComponent<TileController>();
        _ClickEventType = 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("클릭 작동");
        if (TurnMgr.currentTurn == PhotonNetwork.LocalPlayer.ActorNumber)
        {
        Debug.Log("현재 내턴 때 클릭");
        Debug.Log("누구 턴인가 : " + PhotonNetwork.LocalPlayer.ActorNumber);
            var a = ServerIngamePlayer._players[PhotonNetwork.LocalPlayer.ActorNumber];
        Debug.Log("여행 참인지 : " + a._isTravel);
            if (a._isTravel == true)
            {
        Debug.Log("현재 내턴, 여행 때 클릭");
                a._isTravelClickTile = true;
                a._travelClickTileNum = _tileController._tileKey;
                _ClickEventType = 1;
            }
        }
        if (_ClickEventType == 0)
        {
            UIManagerP.instance.InvokeClickUI(_tileController, _tileController._tileType);
            UIManagerP.instance.OnClickUI(_tileController._tileType);
        }
        else 
        {
            Debug.Log("여행");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Outline>().enabled = true; // 강제로 켜보기
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Outline>().enabled = false; // 강제로 켜보기
    }
}
