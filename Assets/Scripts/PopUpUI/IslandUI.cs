using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class IslandUI : MonoBehaviour
{
    public TextMeshProUGUI _name;
    public TextMeshProUGUI _details;
    public Button _closeButton;
    public Button _escapeButton;

    private TileController _currentTileData;
    private ServerIngamePlayer _myPlayer;
    private double escapeCost = 100; // 탈출 비용

    private void Awake()
    {
        if (UIManagerP.instance != null)
        {
            UIManagerP.instance._islandData -= SetData; // 중복 방지
            UIManagerP.instance._islandData += SetData;
        }

        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(() =>
        {
            UIManagerP.instance.OffClickUI();
            TurnMgr.Instance.endTurn();
        });

        _escapeButton.onClick.RemoveAllListeners();
        _escapeButton.onClick.AddListener(TryEscapeIsland);
    }

    public void SetData(TileController _data)
    {
        Debug.Log("[무인도] SetData 호출됨"); // 확인용 로그

        _currentTileData = _data;
        _name.text = _data._tileName;

        int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
        if (ServerIngamePlayer._players.TryGetValue(myActor, out _myPlayer))
        {
            _escapeButton.interactable = _myPlayer._totalMoney >= escapeCost;
            _details.text = $"{_myPlayer._totalMoney} 중 {escapeCost} 지불 시 다음 턴에 탈출 가능합니다.";
        }
        else
        {
            _escapeButton.interactable = false;
            _details.text = "플레이어 정보를 찾을 수 없습니다.";
        }
    }

    private void TryEscapeIsland()
    {
        if (_myPlayer == null || _myPlayer._totalMoney < escapeCost)
        {
            Debug.LogWarning("[무인도] 탈출 불가 조건"); // 확인용
            return;
        }

        _myPlayer._totalMoney -= escapeCost;
        _myPlayer._willEscapeIsland = true;

        UIManagerP.instance.OffClickUI();
        TurnMgr.Instance.endTurn();
    }
}
