using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Olympic : MonoBehaviour, IPointerClickHandler
{
    TileController _tileController;
    private bool isOlympicTileActive = false;
 

    public static event Action<bool> AlreadyCheck;


    private void Start()
    {
        _tileController = GetComponent<TileController>();
    }

    private void OnEnable()
    {
        ServerIngamePlayer.OnPlayerPositionChanged += CheckOlympicTile;
        ServerIngamePlayer.OlympicCheck += OlymplcCheck;
    }
    private void OnDisable()
    {
        ServerIngamePlayer.OnPlayerPositionChanged -= CheckOlympicTile;
        ServerIngamePlayer.OlympicCheck -= OlymplcCheck;
    }

    private void CheckOlympicTile(int playerPosIndex)
    {
        if (playerPosIndex == 20) // 올림픽 타일 위치
        {
            Debug.Log("플레이어가 올림픽 타일에 도착!");
            isOlympicTileActive = true;

        }
    }

    private void OlymplcCheck(bool check)
    {
        isOlympicTileActive= check;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOlympicTileActive == true)
        {
            _tileController._tileLandToll = _tileController._tileLandToll + _tileController._tileLandToll;
            _tileController._tilePensionToll = _tileController._tilePensionToll + _tileController._tilePensionToll;
            _tileController._tileCondoToll = _tileController._tileCondoToll + _tileController._tileCondoToll;
            _tileController._tileHotelToll = _tileController._tileHotelToll + _tileController._tileHotelToll;
            _tileController._tileLandMarkToll = _tileController._tileLandMarkToll + _tileController._tileLandMarkToll;

            AlreadyCheck?.Invoke(false);
            Debug.Log(isOlympicTileActive);

        }
    }
}
