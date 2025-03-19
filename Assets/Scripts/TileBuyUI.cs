using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class TileBuyUI : MonoBehaviour
{
    // 구매 버튼 (땅, 건물)
    public Button _buyTileLandBtn;    // 땅 구매 버튼
    public Button _buyTilePensionBtn; // 펜션 구매 버튼
    public Button _buyTileCondoBtn;   // 콘도 구매 버튼
    public Button _buyTileHotelBtn;   // 호텔 구매 버튼

    // 타일 정보 (UI에 표시되는 텍스트)
    [Header("땅 이름")] public TextMeshProUGUI _tileName;                   // 땅의 이름 표시
    [Header("타일 땅 가격")] public TextMeshProUGUI _tileLandPrice;         // 땅 가격 표시
    [Header("펜션 건물 가격")] public TextMeshProUGUI _tilePensionPrice;    // 펜션 가격 표시
    [Header("콘도 건물 가격")] public TextMeshProUGUI _tileCondoPrice;      // 콘도 가격 표시
    [Header("호텔 건물 가격")] public TextMeshProUGUI _tileHotelPrice;      // 호텔 가격 표시

    // 추가 정보
    [Header("통행료")] public TextMeshProUGUI _tileToll;               // 통행료 표시
    [Header("총 구매 비용")] public TextMeshProUGUI _tileTotalCost;    // 총 구매 비용 표시
    [Header("보유 현금")] public TextMeshProUGUI _playerMoney;         // 현재 플레이어 보유 현금 표시

    // 타일 소유주 정보
    [Header("타일 땅 소유주")] public int _tileLandOwner;           // 땅 소유주 ID
    [Header("타일 1번 건물 소유주")] public int _tilePensionOwner;  // 펜션 소유주 ID
    [Header("타일 2번 건물 소유주")] public int _tileCondoOwner;    // 콘도 소유주 ID
    [Header("타일 3번 건물 소유주")] public int _tileHotelOwner;    // 호텔 소유주 ID
    [Header("랜드마크 소유주")] public int _tileLandMarkOwner;      // 랜드마크 소유주 ID

    public event Action<TileController> OnTileValueChange; // 타일 데이터 변경 이벤트

    void Awake()
    {
        // 게임 매니저에서 타일 데이터 변경 이벤트를 구독
        GameManager gameManager = FindObjectOfType<GameManager>();
        //gameManager.OnTilePopupChange += SetTileData;
    }

    public void SetTileData(TileController data)
    {
        // (방어코드)
        if (data == null) return;

        // 타일 정보 UI 업데이트
        _tileName.text = data._tileName;
        _tileLandPrice.text = data._tileLandPrice.ToString();
        _tilePensionPrice.text = data._tilePensionPrice.ToString();
        _tileCondoPrice.text = data._tileCondoPrice.ToString();
        _tileHotelPrice.text = data._tileHotelPrice.ToString();

        // 현재 플레이어 정보를 가져옴
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager를 찾을 수 없습니다!");
            return;
        }

        // 버튼 상태 업데이트 (소유주에 따른 구매 가능 여부 판단)
        UpdateTilePurchaseButtons(data, playerManager);
    }

    private void UpdateTilePurchaseButtons(TileController data, PlayerManager playerManager)
    {
        // (방어코드)
        if (data == null || playerManager == null) return;

        // 현재 플레이어의 소지금 가져오기
        double currentMoney = playerManager.GetMoney();

        // 각 건물의 소유 상태에 따라 버튼 상태 업데이트
        HandleOwnership(_buyTileLandBtn, data._tileLandOwner, data._tileLandPrice, ref currentMoney, playerManager);
        HandleOwnership(_buyTilePensionBtn, data._tilePensionOwner, data._tilePensionPrice, ref currentMoney, playerManager);
        HandleOwnership(_buyTileCondoBtn, data._tileCondoOwner, data._tileCondoPrice, ref currentMoney, playerManager);
        HandleOwnership(_buyTileHotelBtn, data._tileHotelOwner, data._tileHotelPrice, ref currentMoney, playerManager);
    }

    private void HandleOwnership(Button button, int owner, double price, ref double playerMoney, PlayerManager playerManager)
    {
        switch (owner)
        {
            case 0: // 은행 소유 (플레이어가 구매 가능)
                button.interactable = playerMoney >= price;
                break;

            case 1: // 자기 자신 소유 (이미 구매한 경우 비활성화, 추가 구매 가능하면 활성화)
                if (!button.interactable)
                    button.interactable = false;
                else
                    button.interactable = playerMoney >= price;
                break;

            case 2: // 다른 플레이어 소유 (통행료 차감 후 남은 돈으로 구매 가능 여부 체크)
                double toll = GetTileToll(); // 통행료 가져오기
                playerManager.DecreaseMoney(toll); // 통행료 차감
                playerMoney = playerManager.GetMoney(); // 차감 후 남은 돈 업데이트
                button.interactable = playerMoney >= price; // 남은 돈이 충분하면 버튼 활성화
                break;
        }
    }

    // 현재 모든 건물을 구매했는지 확인하는 함수
    private bool HasPurchasedAllBuildings()
    {
        return !_buyTilePensionBtn.interactable &&
               !_buyTileCondoBtn.interactable &&
               !_buyTileHotelBtn.interactable;
    }

    // 모든 버튼을 비활성화하는 함수
    private void DisableAllButtons()
    {
        _buyTileLandBtn.interactable = false;
        _buyTilePensionBtn.interactable = false;
        _buyTileCondoBtn.interactable = false;
        _buyTileHotelBtn.interactable = false;
    }

    // UI에서 통행료 값을 가져오는 함수
    private double GetTileToll()
    {
        return double.Parse(_tileToll.text);
    }

    public void BuyTileLandCheck() => HandleTilePurchase(_tileLandOwner);
    public void BuyTilePensionCheck() => HandleTilePurchase(_tilePensionOwner);
    public void BuyTileCondoCheck() => HandleTilePurchase(_tileCondoOwner);
    public void BuyTileHotelCheck() => HandleTilePurchase(_tileHotelOwner);
    private void HandleTilePurchase(int owner)
    {
        switch (owner)
        {
            case 0: // 은행 소유 → 구매 가능 여부 체크
                Debug.Log("은행 소유: 구매 가능 여부 확인");
                break;

            case 1: // 자기 자신 소유 → 추가 건물 구매 가능 여부 체크
                Debug.Log("자신 소유: 추가 건물 구매 가능 여부 확인");
                break;

            case 2: // 다른 플레이어 소유 → 통행료 지불 후 추가 구매 가능 여부 체크
                Debug.Log("다른 플레이어 소유: 통행료 지불 후 구매 가능 여부 확인");
                break;
        }
    }
}
