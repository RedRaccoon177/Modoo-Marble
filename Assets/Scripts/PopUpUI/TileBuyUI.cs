using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.ComponentModel;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using System.Reflection;

public class TileBuyUI : MonoBehaviour
{
    #region UI 변수들
    [Header("구매 체크 버튼 및 체크 이미지")]
    public Button[] _buildingButtons;           // 순서: Land, Pension, Condo, Hotel
    public Image[] _checkImages;                // 체크 여부에 따라 색상 변경

    [Header("UI 구성요소")]
    public Button _buyBtn;                      // 구매 버튼
    public Button _cancelBtn;                   // 취소 버튼

    [Header("땅 이름")]
    public TextMeshProUGUI _tileName;           // 타일의 이름 표시

    [Header("타일 땅,건물들 가격")]
    public TextMeshProUGUI[] _buildingPricesText; // 각 건물의 가격 텍스트

    [Header("통행료")]
    public TextMeshProUGUI _tileToll;           // 통행료 텍스트

    [Header("총 구매 비용")]
    public TextMeshProUGUI _tileTotalCost;      // 총 가격 텍스트

    [Header("보유 현금")]
    public TextMeshProUGUI _playerMoney;        // 플레이어의 현재 자산

    [Header("플레이어 현재 보유 금액 (게임 내에서 변할 수 있음)")]
    public double _currentMoney;
    public double _cancelRememberMoney;         // 취소 시 복구할 금액

    [Header("토지, 펜션, 콘도, 호텔 구매 여부 체크")]
    private bool[] _buildingChecks = new bool[4]; // 건물 구매 여부

    [Header("타일 땅, 건물 소유주")]
    private int[] _tileOwners = new int[4];     // 건물 소유자 정보

    public event Action<TileController> OnTileValueChange; // 변경 이벤트

    TileController _currentTile;                // 현재 타일 정보 참조

    bool _FHandleBankOwnership = false;         // 은행처리 중복 방지

    [Header("플레이어들의 고유 번호 지정")]
    int _playerKey;                             // 나의 고유 ActorNumber
    int[] _enemyKeys;                           // 상대방들 ActorNumber

    PlayerMoveTest _playerMoveTest;

    ServerIngamePlayer _playerData;
    #endregion

    void Awake()
    {
        // UI 이벤트 연결
        UIManagerP.instance._buyChangeDataGround += SetTileData;
        UpdateCheckImages(); // 색상 초기화
        BindButtonEvents();  // 버튼 이벤트 연결
        SetPlayerAndEnemies();
    }

    /// <summary>
    /// 나와 적 구분해서 고유번호 저장
    /// </summary>
    void SetPlayerAndEnemies()
    {
        // 현재 로컬 플레이어의 고유 ActorNumber를 저장
        _playerKey = PhotonNetwork.LocalPlayer.ActorNumber;

        // 모든 플레이어 목록 중에서
        _enemyKeys = PhotonNetwork.PlayerList
            .Where(p => p.ActorNumber != _playerKey)      // 나(로컬 플레이어)가 아닌 플레이어만 필터링하고
            .Select(p => p.ActorNumber)                   // 각 플레이어의 고유 ActorNumber만 추출해서
            .ToArray();                                   // 배열로 만들어 적 목록(_enemyKeys)에 저장
    }

    /// <summary>
    /// 각 버튼의 클릭 이벤트 연결
    /// </summary>
    void BindButtonEvents()
    {
        for (int i = 0; i < _buildingButtons.Length; i++)
        {
            int index = i; // 람다 캡처 방지
            _buildingButtons[i].onClick.AddListener(() => ToggleButtonState(index));
        }

        _buyBtn.onClick.AddListener(BuyButtonClick);
        _cancelBtn.onClick.AddListener(CancelBtnClick);
    }

    #region 구매 체크 이미지 및 버튼 상태 갱신
    /// <summary>
    /// 건물 구매 여부 상태 전환 + UI 색상 및 Buy 버튼 갱신
    /// </summary>
    void ToggleButtonState(int index)
    {
        double price = _currentTile.GetPrice(index);

        if (_buildingChecks[index]) _currentMoney += price;
        else if (_currentMoney >= price) _currentMoney -= price;
        else { Debug.Log("마이너스 금지"); return; }

        _buildingChecks[index] = !_buildingChecks[index];

        UpdateImageColor(_checkImages[index], _buildingChecks[index]);
        UpdateBuyButtonState();

        if (_FHandleBankOwnership) TileBuyCheckBtnCheck();
    }

    /// <summary>
    /// Buy 버튼 활성화 조건: 토지 주인이 내가 아니고, 토지가 체크됨
    /// </summary>
    void UpdateBuyButtonState()
    {
        _buyBtn.interactable = _tileOwners[0] != _playerKey && _buildingChecks[0];

        //내 토지일 경우
        if (_tileOwners[0] == _playerKey)
        {
            for (int i = 1; i < 4; i++)
            {
                //다른 토지들를 하나라도 구매 안 했을 경우
                if (_tileOwners[i] != _playerKey)
                {
                    _buyBtn.interactable = true;
                }
            }
        }
    }

    /// <summary>
    /// 이미지 색상: 초록은 구매 체크, 빨강은 미체크
    /// </summary>
    void UpdateImageColor(Image image, bool isChecked)
    {
        image.color = isChecked ? new Color(0.3f, 1f, 0.3f) : new Color(1f, 0.3f, 0.3f);
    }

    /// <summary>
    /// 체크 이미지들 초기화
    /// </summary>
    void UpdateCheckImages()
    {
        for (int i = 0; i < _checkImages.Length; i++) UpdateImageColor(_checkImages[i], _buildingChecks[i]);
    }
    #endregion

    /// <summary>
    /// 타일 선택 시 정보 갱신 및 UI 세팅
    /// </summary>
    public void SetTileData(TileController data)
    {
        if (data == null) return;

        _FHandleBankOwnership = false;
        _currentTile = data;

        _tileName.text = data._tileName;

        for (int i = 0; i < 4; i++)
        {
            _buildingPricesText[i].text = data.GetPrice(i).ToString();
            _tileOwners[i] = data.GetOwner(i);
        }

        ServerIngamePlayer[] playerDatas = FindObjectsOfType<ServerIngamePlayer>();
        foreach (var playerData in playerDatas)
        {
            if (playerData.photonView.OwnerActorNr == _playerKey)
            {
                _playerData = playerData;
                break;
            }
        }

        ResetButtonStates();
        _currentMoney = _cancelRememberMoney = _playerData.GetMoney();

        int landOwner = _tileOwners[0];

        if (landOwner == 0) HandleBankOwnership();
        else if (landOwner == _playerKey) HandlePlayerOwnership();
        else HandleEnemyOwnership(_playerData);
    }

    /// <summary>
    /// 버튼 상태 초기화 후 Buy 버튼 갱신
    /// </summary>
    void ResetButtonStates()
    {
        for (int i = 0; i < 4; i++)
        {
            _buildingChecks[i] = false;
            _buildingButtons[i].interactable = true;
        }

        UpdateCheckImages();
        UpdateBuyButtonState();
    }

    /// <summary>
    /// 은행 소유일 경우, 자금 충분하면 자동 체크
    /// </summary>
    void HandleBankOwnership()
    {
        for (int i = 0; i < 4; i++)
        {
            if (_currentMoney >= _currentTile.GetPrice(i)) _buildingButtons[i].onClick.Invoke();
            else _buildingButtons[i].interactable = false;
        }

        _FHandleBankOwnership = true;

        // 버튼 처리 이후 1프레임 대기 후 상태 갱신
        StartCoroutine(DelayUpdateBuyButtonState());
    }

    /// <summary>
    /// UI 상태 갱신을 한 프레임 뒤에 실행 (Buy 버튼 문제 해결용)
    /// </summary>
    IEnumerator DelayUpdateBuyButtonState()
    {
        yield return null;
        UpdateBuyButtonState();
    }

    /// <summary>
    /// 두 번째 건물부터 자금 부족 시 버튼 비활성화 처리
    /// </summary>
    void TileBuyCheckBtnCheck()
    {
        for (int i = 1; i < 4; i++)
        {
            double price = _currentTile.GetPrice(i);
            _buildingButtons[i].interactable = !_buildingChecks[i] && _currentMoney >= price || _buildingChecks[i];
        }
    }

    /// <summary>
    /// 구매 버튼 눌렀을 때 처리
    /// </summary>
    void BuyButtonClick()
    {
        if (_currentTile == null) return;

        for (int i = 0; i < 4; i++)
        {
            int owner = _tileOwners[i];
            int newOwner = owner == 0
                ? (_buildingChecks[i] ? _playerKey : 0)
                : owner; // 기존 소유자 유지

            _currentTile.photonView.RPC("SetOwner", RpcTarget.All, i, newOwner);
        }

        ServerIngamePlayer[] playerDatas = FindObjectsOfType<ServerIngamePlayer>();
        foreach (var _playerData in playerDatas)
        {
            if (_playerData.photonView.OwnerActorNr == _playerKey)
            {
                _playerData.photonView.RPC("MoneyReturn", RpcTarget.All, _currentMoney);
                _playerData.photonView.RPC("TotalMoney", RpcTarget.All);
                break;
            }
        }

        _playerMoveTest = FindObjectOfType<PlayerMoveTest>();
        _playerMoveTest.endTurn();

        UIManagerP.instance.OffBuyUIPanel();
    }

    /// <summary>
    /// 취소 시 금액 원상복구 + UI 닫기
    /// </summary>
    void CancelBtnClick()
    {
        _currentMoney = _cancelRememberMoney;
        UIManagerP.instance.OffBuyUIPanel();

        _playerMoveTest = FindObjectOfType<PlayerMoveTest>();
        _playerMoveTest.endTurn();
    }

    /// <summary>
    /// 플레이어가 이미 소유한 땅일 경우 건물만 구매 가능
    /// </summary>
    void HandlePlayerOwnership()
    {
        // 플레이어가 구매한 토지는 전부 녹색으로 전환
        for (int i = 0; i < 4; i++)
        {
            if (_tileOwners[i] == _playerKey)
            {
                _buildingChecks[i] = true;
                UpdateImageColor(_checkImages[i], _buildingChecks[i]);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            _buildingButtons[i].interactable = _tileOwners[i] == 0 && _currentMoney >= _currentTile.GetPrice(i);
        }

        _buyBtn.interactable = true;
    }

    #region 추후 기능
    /// <summary>
    /// 적이 소유한 땅일 경우: 건물 비활성화, 금액만 갱신
    /// </summary>
    void HandleEnemyOwnership(ServerIngamePlayer pm)
    {
        _currentMoney = pm.GetMoney();
    }
    #endregion
}