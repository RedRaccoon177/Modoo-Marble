using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class TileBuyUI : MonoBehaviour
{
    // 구매 체크 버튼 (땅, 건물)
    public Button _buyTileLandBtn;    // 땅 구매 버튼
    public Button _buyTilePensionBtn; // 펜션 구매 버튼
    public Button _buyTileCondoBtn;   // 콘도 구매 버튼
    public Button _buyTileHotelBtn;   // 호텔 구매 버튼

    // 실제 구매 버튼
    public Button _buyBtn;

    private bool _isLandCheck = false;      // 토지 구매 여부 체크
    private bool _isPensionCheck = false;   // 펜션 구매 여부 체크
    private bool _isCondoCheck = false;     // 콘도 구매 여부 체크
    private bool _isHotelCheck = false;     // 호텔 구매 여부 체크

    //체크 이미지들
    public Image _islandCheckImage;
    public Image _isPensionCheckImage;       
    public Image _isCondoCheckImage;
    public Image _isHotelCheckImage;

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

    // 플레이어 현재 보유 금액 (게임 내에서 변할 수 있음)
    private double _currentMoney;

    //플레이어의 고유 번호
    int _playerKey = 1;

    // 타일 정보 변경 이벤트
    public event Action<TileController> OnTileValueChange;

    private TileController _currentTile; // 현재 선택된 타일 저장

    void Awake()
    {
        // 게임 매니저에서 타일 데이터 변경 이벤트를 구독
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.OnTilePopupDataChange += SetTileData;
    }

    void Start()
    {
        // 초기 색상 설정
        UpdateCheckImages();

        // 버튼 클릭 이벤트 연결
        _buyTileLandBtn.onClick.AddListener(() => ToggleButtonState(ref _isLandCheck, _islandCheckImage, ref _currentMoney, _currentTile._tileLandPrice));
        _buyTilePensionBtn.onClick.AddListener(() => ToggleButtonState(ref _isPensionCheck, _isPensionCheckImage, ref _currentMoney, _currentTile._tilePensionPrice));
        _buyTileCondoBtn.onClick.AddListener(() => ToggleButtonState(ref _isCondoCheck, _isCondoCheckImage, ref _currentMoney, _currentTile._tileCondoPrice));
        _buyTileHotelBtn.onClick.AddListener(() => ToggleButtonState(ref _isHotelCheck, _isHotelCheckImage, ref _currentMoney, _currentTile._tileHotelPrice));
        _buyBtn.onClick.AddListener(() => BuyButtonClick());
    }

    #region 구매 체크 이미지 색상 변경 false 빨강 / true 초록
    // 버튼 상태를 변경하고 체크 이미지 색상 업데이트 + Buy 버튼 상태 업데이트
    private void ToggleButtonState(ref bool isChecked, Image image, ref double playerMoney, double price)
    {
        // isChecked가 참이면 구매 한 것일 테니 클릭하여 구매 안하겠다고 하는 것이다. 돈을 돌려줘라.
        if (isChecked) playerMoney += price;
        else playerMoney -= price;

        isChecked = !isChecked; // 현재 상태 반전 (true <-> false)
        UpdateImageColor(image, isChecked); // 변경된 상태에 따라 이미지 색상 변경
        UpdateBuyButtonState(); // Buy 버튼 상태 업데이트

        TileBuyCheckBtnCheck(_currentTile);
        Debug.Log(_currentMoney);
    }

    // Buy 버튼을 _isLandCheck 값에 따라 활성화/비활성화
    private void UpdateBuyButtonState()
    {
        _buyBtn.interactable = _isLandCheck; // _isLandCheck가 true면 활성화, false면 비활성화
    }

    // 특정 체크 이미지의 색상을 변경
    private void UpdateImageColor(Image image, bool isChecked)
    {
        image.color = isChecked ? new Color(0.3f, 1f, 0.3f) : new Color(1f, 0.3f, 0.3f); // 초록색 or 빨간색
    }

    // 모든 체크 이미지 초기 색상 설정
    private void UpdateCheckImages()
    {
        UpdateImageColor(_islandCheckImage, _isLandCheck);
        UpdateImageColor(_isPensionCheckImage, _isPensionCheck);
        UpdateImageColor(_isCondoCheckImage, _isCondoCheck);
        UpdateImageColor(_isHotelCheckImage, _isHotelCheck);
    }
    #endregion

    public void SetTileData(TileController data)
    {
        // 데이터가 없으면 바로 리턴 (안전장치)
        if (data == null) return;

        _currentTile = data;

        // UI에 타일 정보 업데이트
        _tileName.text = data._tileName;
        _tileLandPrice.text = data._tileLandPrice.ToString();
        _tilePensionPrice.text = data._tilePensionPrice.ToString();
        _tileCondoPrice.text = data._tileCondoPrice.ToString();
        _tileHotelPrice.text = data._tileHotelPrice.ToString();

        _tileLandOwner = data._tileLandOwner;
        _tilePensionOwner = data._tilePensionOwner;
        _tileCondoOwner = data._tileCondoOwner;
        _tileHotelOwner = data._tileHotelOwner;

        // 현재 플레이어 정보를 가져옴
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager를 찾을 수 없습니다!");
            return;
        }

        //타일 버튼 체크 초기화
        ResetButtonStates();

        // 소유 상태에 따라 버튼 활성화 여부 결정
        UpdateTilePurchaseButtons(data, playerManager);
    }

    // 모든 버튼 상태 초기화 (초기값: false)
    void ResetButtonStates()
    {
        // 모든 체크 상태를 false로 초기화
        _isLandCheck = false;
        _isPensionCheck = false;
        _isCondoCheck = false;
        _isHotelCheck = false;

        // 모든 체크 이미지 색상을 빨간색으로 초기화
        UpdateCheckImages();

        // Buy 버튼 상태 초기화
        UpdateBuyButtonState();

        // 버튼 클릭 이벤트 초기화 후 다시 연결
        Button[] buyButtons = { _buyTileLandBtn, _buyTilePensionBtn, _buyTileCondoBtn, _buyTileHotelBtn };
        Image[] checkImages = { _islandCheckImage, _isPensionCheckImage, _isCondoCheckImage, _isHotelCheckImage };

        for (int i = 0; i < buyButtons.Length; i++)
        {
            buyButtons[i].onClick.RemoveAllListeners();
            buyButtons[i].interactable = true; // 버튼 초기화 (클릭 가능하게)

            int index = i; // 람다 캡처 문제 방지

            buyButtons[i].onClick.AddListener(() =>
            {
                switch (index)
                {
                    case 0: ToggleButtonState(ref _isLandCheck, _islandCheckImage, ref _currentMoney, _currentTile._tileLandPrice); break;
                    case 1: ToggleButtonState(ref _isPensionCheck, _isPensionCheckImage, ref _currentMoney, _currentTile._tilePensionPrice); break;
                    case 2: ToggleButtonState(ref _isCondoCheck, _isCondoCheckImage, ref _currentMoney, _currentTile._tileCondoPrice); break;
                    case 3: ToggleButtonState(ref _isHotelCheck, _isHotelCheckImage, ref _currentMoney, _currentTile._tileHotelPrice); break;
                }
            });
        }
    }

    private void UpdateTilePurchaseButtons(TileController data, PlayerManager playerManager)
    {
        if (data == null || playerManager == null) return;

        // 현재 플레이어가 보유한 돈을 가져옴
        _currentMoney = playerManager.GetMoney();

        // 소유 상태에 따라 구매 가능 여부를 체크
        switch (data._tileLandOwner)
        {
            case 0: // 은행 소유 → 플레이어가 구매 가능
                HandleBankOwnership(data);
                break;
            case 1: // 자신 소유 → 건물 추가 구매 가능 여부 판단
                HandlePlayerOwnership(data);
                break;
            case 2: // 적 소유 → 통행료 차감 후 구매 가능 여부 판단
                HandleEnemyOwnership(data, playerManager);
                break;
        }
    }

    /// <summary>
    /// 은행 소유일 경우 함수
    /// </summary>
    /// <param name="data"></param>
    private void HandleBankOwnership(TileController data)
    {
        if (_currentMoney >= data._tileLandPrice)
        {
            Debug.Log($" HandleBankOwnership 실행됨 (현재 보유 금액: {_currentMoney})");

            // 토지 구매 버튼 강제 클릭 (토글 상태 변경을 위해)
            _buyTileLandBtn.onClick.Invoke();

            if (_currentMoney >= data._tilePensionPrice)
            {
                _buyTilePensionBtn.onClick.Invoke();
            }
            else
            {
                _buyTilePensionBtn.interactable = false;
            }

            if (_currentMoney >= data._tileCondoPrice)
            {
                _buyTileCondoBtn.onClick.Invoke();
            }
            else
            {
                _buyTileCondoBtn.interactable = false;
            }

            if (_currentMoney >= data._tileHotelPrice)
            {
                _buyTileHotelBtn.onClick.Invoke();
            }
            else
            {
                _buyTileHotelBtn.interactable = false;
            }
        }
        else
        {
            _buyTileLandBtn.interactable = false;
            _buyTilePensionBtn.interactable = false;
            _buyTileCondoBtn.interactable = false;
            _buyTileHotelBtn.interactable = false;
        }
    }

    private void TileBuyCheckBtnCheck(TileController data)
    {
        if (!_isPensionCheck && _currentMoney >= data._tilePensionPrice)
        {
            _buyTilePensionBtn.interactable = true;
        }
        else
        {
            _buyTilePensionBtn.interactable = false;
        }

        if (!_isCondoCheck && _currentMoney >= data._tileCondoPrice)
        {
            _buyTileCondoBtn.interactable = true;
        }
        else
        {
            _buyTileCondoBtn.interactable = false;
        }

        if (!_isHotelCheck && _currentMoney >= data._tileHotelPrice)
        {
            _buyTileHotelBtn.interactable = true;
        }
        else
        {
            _buyTileHotelBtn.interactable = false;
        }
    }

    //구매 버튼 클릭
    void BuyButtonClick()
    {
        if (_currentTile == null) return; // 현재 선택된 타일이 없으면 리턴

        _currentTile._tileLandOwner = _isLandCheck ? _playerKey : 0;
        _currentTile._tilePensionOwner = _isPensionCheck ? _playerKey : 0;
        _currentTile._tileCondoOwner = _isCondoCheck ? _playerKey : 0;
        _currentTile._tileHotelOwner = _isHotelCheck ? _playerKey : 0;

        OnTileValueChange?.Invoke(_currentTile); // 정확한 타일 데이터 전달
    }


    #region 추후에 진행

    /// <summary>
    /// 플레이어 자신의 땅일 경우 함수
    /// </summary>
    /// <param name="data"></param>
    private void HandlePlayerOwnership(TileController data)
    {
        _buyTileLandBtn.interactable = false;
        _buyTilePensionBtn.interactable = 
            data._tilePensionOwner == 0 && _currentMoney >= data._tilePensionPrice;
        _buyTileCondoBtn.interactable =
            data._tileCondoOwner == 0 && _currentMoney >= data._tileCondoPrice;
        _buyTileHotelBtn.interactable = 
            data._tileHotelOwner == 0 && _currentMoney >= data._tileHotelPrice;
    }

    /// <summary>
    /// 적 소유의 땅일 경우 함수
    /// </summary>
    /// <param name="data"></param>
    /// <param name="playerManager"></param>
    private void HandleEnemyOwnership(TileController data, PlayerManager playerManager)
    {
        // 통행료를 가져와서 차감
        _currentMoney = playerManager.GetMoney();

    }
    #endregion
}