using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;
public partial class TileBuyUI : MonoBehaviour
{
    [Header("통행료")] public TextMeshProUGUI _tollPriceText;
    double _tollPrice = 0;
    [Header("총 구매 비용")] public TextMeshProUGUI _totalBuyPriceText;
    [Header("보유 현금")] public TextMeshProUGUI _playerTotalMoneyText;
    PlayerManager playerManager;

    public void PrintTotalBuyPrice(double current, double currentRemember)
    {
        _totalBuyPriceText.text = (currentRemember - current).ToString();
    }
    public void PrintTollPrice(double _tollPrice)
    {
        _tollPriceText.text = _tollPrice.ToString();
    }
    public void PrintPlayerMoney(PlayerManager _player)
    {
        _playerTotalMoneyText.text = _player._money.ToString();
    }
}

public partial class TileBuyUI : MonoBehaviour
{
    #region UI 변수들
    [Header("구매 체크 버튼 (땅, 건물)")]
    public Button _buyTileLandBtn;    // 땅 구매 버튼
    public Button _buyTilePensionBtn; // 펜션 구매 버튼
    public Button _buyTileCondoBtn;   // 콘도 구매 버튼
    public Button _buyTileHotelBtn;   // 호텔 구매 버튼

    // 실제 구매 버튼
    public Button _buyBtn;

    //취소 버튼
    public Button _cancelBtn;

    [Header("토지, 펜션, 콘도, 호텔 구매 여부 체크")]
    private bool _isLandCheck = false;      // 토지 구매 여부 체크
    private bool _isPensionCheck = false;   // 펜션 구매 여부 체크
    private bool _isCondoCheck = false;     // 콘도 구매 여부 체크
    private bool _isHotelCheck = false;     // 호텔 구매 여부 체크

    [Header("체크 이미지들 색 변화를 위해")]
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

    [Header("플레이어 현재 보유 금액 (게임 내에서 변할 수 있음)")]
    public double _currentMoney;
    public double _cancelRememberMoney;

    //플레이어의 고유 번호
    int _playerKey = 1;
    int _enemyKey = 2;

    // 타일 정보 변경 이벤트
    public event Action<TileController> OnTileValueChange;

    // 현재 선택된 타일 저장
    private TileController _currentTile;

    // HandleBankOwnership 함수 참 거짓으로 중복 실행 방지
    bool _FHandleBankOwnership = false;
    #endregion

    void Awake()
    {
        UIManagerP.instance._buyChangeDataGround += SetTileData;
        // 초기 색상 설정
        UpdateCheckImages();
        // 버튼 클릭 이벤트 연결
        BindButtonEvents();
    }

    private void OnEnable()
    {
        _tollPrice = 0;
    }

    /// <summary>
    /// 버튼 클릭 이벤트 연결
    /// </summary>
    void BindButtonEvents()
    {
        _buyTileLandBtn.onClick.AddListener(() => ToggleButtonState(ref _isLandCheck, _islandCheckImage, ref _currentMoney, _currentTile._tileLandPrice, _currentTile._tileLandToll));

        _buyTilePensionBtn.onClick.AddListener(() => ToggleButtonState(ref _isPensionCheck, _isPensionCheckImage, ref _currentMoney, _currentTile._tilePensionPrice, _currentTile._tilePensionToll));

        _buyTileCondoBtn.onClick.AddListener(() => ToggleButtonState(ref _isCondoCheck, _isCondoCheckImage, ref _currentMoney, _currentTile._tileCondoPrice, _currentTile._tileCondoToll));

        _buyTileHotelBtn.onClick.AddListener(() => ToggleButtonState(ref _isHotelCheck, _isHotelCheckImage, ref _currentMoney, _currentTile._tileHotelPrice, _currentTile._tileHotelToll));

        _buyBtn.onClick.AddListener(() => BuyButtonClick());

        _cancelBtn.onClick.AddListener(() => CancelBtnClick());
    }

    #region 구매 체크 이미지 색상 변경 false 빨강 / true 초록
    /// <summary>
    /// 버튼 상태를 변경하고 체크 이미지 색상 업데이트 + Buy 버튼 상태 업데이트
    /// </summary>
    /// <param name="isChecked"></param>
    /// <param name="image"></param>
    /// <param name="playerMoney"></param>
    /// <param name="price"></param>
    void ToggleButtonState(ref bool isChecked, Image image, ref double playerMoney, double price , double tollPrice)
    {
        // isChecked가 참이면 구매 한 것일 테니 클릭하여 구매 안하겠다고 하는 것이다. 돈을 돌려줘라.
        if (isChecked)
        {
            playerMoney += price;
            _tollPrice -= tollPrice;
            isChecked = !isChecked; // 현재 상태 반전 (true <-> false)
            UpdateImageColor(image, isChecked); // 변경된 상태에 따라 이미지 색상 변경
            UpdateBuyButtonState(); // Buy 버튼 상태 업데이트
            if (_FHandleBankOwnership == true)
            {
                TileBuyCheckBtnCheck(_currentTile);
            }
        }
        else
        {
            if (playerMoney - price >= 0)
            {
                playerMoney -= price;
                _tollPrice += tollPrice;
                isChecked = !isChecked; // 현재 상태 반전 (true <-> false)
                UpdateImageColor(image, isChecked); // 변경된 상태에 따라 이미지 색상 변경
                UpdateBuyButtonState(); // Buy 버튼 상태 업데이트
                if (_FHandleBankOwnership == true)
                {
                    TileBuyCheckBtnCheck(_currentTile);
                }
            }
            else
            {
                Debug.Log("마이너스 금지");
            }
        }
        PrintTotalBuyPrice(_currentMoney, _cancelRememberMoney);
        PrintTollPrice(_tollPrice);
    }

    /// <summary>
    /// Buy 버튼을 _isLandCheck 값에 따라 활성화/비활성화
    /// </summary>
    void UpdateBuyButtonState()
    {
        if (_tileLandOwner != _playerKey) _buyBtn.interactable = _isLandCheck; // _isLandCheck가 true면 활성화, false면 비활성화
    }

    /// <summary>
    /// 특정 체크 이미지의 색상을 변경
    /// </summary>
    /// <param name="image"></param>
    /// <param name="isChecked"></param>
    void UpdateImageColor(Image image, bool isChecked)
    {
        image.color = isChecked ? new Color(0.3f, 1f, 0.3f) : new Color(1f, 0.3f, 0.3f); // 초록색 or 빨간색
    }

    /// <summary>
    /// 모든 체크 이미지 초기 색상 설정
    /// </summary>
    void UpdateCheckImages()
    {
        UpdateImageColor(_islandCheckImage, _isLandCheck);
        UpdateImageColor(_isPensionCheckImage, _isPensionCheck);
        UpdateImageColor(_isCondoCheckImage, _isCondoCheck);
        UpdateImageColor(_isHotelCheckImage, _isHotelCheck);
    }
    #endregion

    /// <summary>
    /// 각 타일 데이터 가져와서 정보 삽입
    /// </summary>
    /// <param name="data"></param>
    public void SetTileData(TileController data)
    {
        _FHandleBankOwnership = false;

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
        playerManager = FindObjectOfType<PlayerManager>();
        PrintPlayerMoney(playerManager);
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

    /// <summary>
    /// 모든 버튼 상태 초기화 (초기값: false)
    /// </summary>
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
                    case 0: ToggleButtonState(ref _isLandCheck, _islandCheckImage, ref _currentMoney, _currentTile._tileLandPrice, _currentTile._tileLandToll); break;
                    case 1: ToggleButtonState(ref _isPensionCheck, _isPensionCheckImage, ref _currentMoney, _currentTile._tilePensionPrice, _currentTile._tilePensionToll); break;
                    case 2: ToggleButtonState(ref _isCondoCheck, _isCondoCheckImage, ref _currentMoney, _currentTile._tileCondoPrice, _currentTile._tileCondoToll); break;
                    case 3: ToggleButtonState(ref _isHotelCheck, _isHotelCheckImage, ref _currentMoney, _currentTile._tileHotelPrice, _currentTile._tileHotelToll); break;
                }
            });
        }
    }

    /// <summary>
    /// 타일의 소유 상태 구분
    /// </summary>
    /// <param name="data"></param>
    /// <param name="playerManager"></param>
    void UpdateTilePurchaseButtons(TileController data, PlayerManager playerManager)
    {
        if (data == null || playerManager == null) return;

        // 현재 플레이어가 보유한 돈을 가져옴
        _currentMoney = playerManager.GetMoney();
        _cancelRememberMoney = _currentMoney;

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
    void HandleBankOwnership(TileController data)
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

        _FHandleBankOwnership = true;
    }

    /// <summary>
    /// 첫번째 비교 이후 두번째 부터는 버튼 클릭 시 체크용
    /// </summary>
    /// <param name="data"></param>
    void TileBuyCheckBtnCheck(TileController data)
    {
        if (!_isPensionCheck && _currentMoney >= data._tilePensionPrice)
        {
            _buyTilePensionBtn.interactable = true;
        }
        else if (_isPensionCheck)
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
        else if (_isCondoCheck)
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
        else if (_isHotelCheck)
        {
            _buyTileHotelBtn.interactable = true;
        }
        else
        {
            _buyTileHotelBtn.interactable = false;
        }
    }

    /// <summary>
    /// 구매 버튼 클릭
    /// </summary>
    void BuyButtonClick()
    {
        if (_currentTile == null) return; // 현재 선택된 타일이 없으면 리턴

        //자신이 이미 구매한 토지일 경우 자신꺼
        if(_tileLandOwner == _playerKey)
        {
            _currentTile._tileLandOwner = _playerKey;
        }
        //이미 적이 구매한 토지일 경우
        else if (_tileLandOwner != 0)
        {
            _currentTile._tileLandOwner = _isLandCheck ? _playerKey : _enemyKey;
        }
        // 은행꺼였을 경우
        else
        {
            _currentTile._tileLandOwner = _isLandCheck ? _playerKey : 0;
        }

        if(_tilePensionOwner == _playerKey)
        {
            _currentTile._tilePensionOwner = _playerKey;
        }
        else if(_tilePensionOwner != 0)
        {
            _currentTile._tilePensionOwner = _isPensionCheck ? _playerKey : _enemyKey;
        }
        else
        {
            _currentTile._tilePensionOwner = _isPensionCheck ? _playerKey : 0;
        }

        if (_tileCondoOwner == _playerKey)
        { 
            _currentTile._tileCondoOwner = _playerKey;
        }
        else if (_tileCondoOwner != 0)
        {
            _currentTile._tileCondoOwner = _isCondoCheck ? _playerKey : _enemyKey;
        }
        else
        {
            _currentTile._tileCondoOwner = _isCondoCheck ? _playerKey : 0;
        }

        if(_tileHotelOwner == _playerKey)
        {
            _currentTile._tileHotelOwner = _playerKey;
        }
        else if (_tileHotelOwner != 0)
        {
            _currentTile._tileHotelOwner = _isHotelCheck ? _playerKey : _enemyKey;
        }
        else
        {
            _currentTile._tileHotelOwner = _isHotelCheck ? _playerKey : 0;
        }

        OnTileValueChange?.Invoke(_currentTile); // 정확한 타일 데이터 전달
        playerManager.DecreaseMoney(_cancelRememberMoney - _currentMoney);
        UIManagerP.instance.OffBuyUIPanel();
    }

    /// <summary>
    /// 취소 버튼 클릭 했을 때
    /// </summary>
    void CancelBtnClick()
    {
        _currentMoney = _cancelRememberMoney;
        UIManagerP.instance.OffBuyUIPanel();
        //TODO:Panel 비활성화 시키기
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