using Photon.Pun;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 관광지(Sea) 타일 구매 UI 처리 클래스
/// </summary>
public class TileSeaBuyUI : MonoBehaviour
{
    [Header("땅 이름")]
    public TextMeshProUGUI _tileName;

    [Header("타일 땅 값")]
    public TextMeshProUGUI _tileLandPrice;

    [Header("장 수 보유 통행료")]
    public TextMeshProUGUI _tileOneToll;
    public TextMeshProUGUI _tileTwoToll;
    public TextMeshProUGUI _tileThreeToll;
    public TextMeshProUGUI _tileFourToll;

    [Header("현재 보유중인 장수")]
    public TextMeshProUGUI _tileSheet;

    [Header("총 구매 비용")]
    public TextMeshProUGUI _tileToll;

    [Header("보유 현금")]
    public TextMeshProUGUI _playerMoney;

    [Header("버튼들")]
    public Button _buyButton;
    public Button _closeButton;

    [Header("플레이어 현재 돈")]
    public double _money;

    [Header("플레이어 고유 번호 지정")]
    int _playerKey;

    [Header("관광지 구매 수")]
    public int _seaBuyCount = 0;

    // 현재 관광지 타일 정보
    TileController _currentTile;

    // 현재 플레이어의 인게임 데이터
    ServerIngamePlayer _playerData = null;

    /// <summary>
    /// 초기화 및 버튼 이벤트 설정
    /// </summary>
    void Awake()
    {
        SetPlayerData();

        _buyButton.onClick.AddListener(() => UIManagerP.instance.OffBuyUIPanel());
        _buyButton.onClick.AddListener(() => BuySeaTile());

        _closeButton.onClick.AddListener(() => UIManagerP.instance.OffBuyUIPanel());
        UIManagerP.instance._buyChangeDataSea += SetTileData;
    }

    /// <summary>
    /// 로컬 플레이어 정보 가져오기
    /// </summary>
    void SetPlayerData()
    {
        _playerKey = PhotonNetwork.LocalPlayer.ActorNumber;

        ServerIngamePlayer[] playerDatas = FindObjectsOfType<ServerIngamePlayer>();
        foreach (var playerData in playerDatas)
        {
            if (playerData.photonView.OwnerActorNr == _playerKey)
            {
                _playerData = playerData;
                break;
            }
        }
    }

    /// <summary>
    /// 관광지 UI에 타일 데이터 표시
    /// </summary>
    /// <param name="data">선택된 타일 데이터</param>
    public void SetTileData(TileController data)
    {
        if (data == null) return;

        _currentTile = data;

        // 텍스트 UI 설정
        _tileName.text = data._tileName;
        _tileLandPrice.text = data._tileLandPrice.ToString();
        _tileOneToll.text = data._tileLandToll.ToString();
        _tileTwoToll.text = data._tilePensionToll.ToString();
        _tileThreeToll.text = data._tileCondoToll.ToString();
        _tileFourToll.text = data._tileHotelToll.ToString();

        // 플레이어 보유 Sea 타일 수 갱신
        _playerData.RefreshOwnedSeaTiles();
        _seaBuyCount = _playerData.GetOwnedSeaTileCount();
        _tileSheet.text = _seaBuyCount.ToString();

        // 플레이어 보유 금액 표시
        if (_playerData != null)
        {
            _money = _playerData.GetMoney();
            _playerMoney.text = _money.ToString();
        }
        else
        {
            Debug.LogWarning("TileSea 터짐: 플레이어 데이터 없음");
        }

        // 구매 버튼 활성화 여부 설정
        double landPrice;
        if (double.TryParse(_tileLandPrice.text, out landPrice))
        {
            _buyButton.interactable = (_money >= landPrice);
            _tileToll.text = GetTollBySeaCount(_seaBuyCount + 1).ToString();
        }
    }

    public void BuySeaTile()
    {
        if (_currentTile == null || _playerData == null) return;

        double landPrice;
        if (!double.TryParse(_tileLandPrice.text, out landPrice)) return;

        // 자금 부족 시 중단
        if (_money < landPrice)
        {
            Debug.Log("돈이 부족하여 관광지를 구매할 수 없습니다.");
            return;
        }

        // 돈 차감 및 반영
        _money -= landPrice;
        _playerData.photonView.RPC("MoneyReturn", RpcTarget.All, _money);

        // 관광지 소유자 등록 (현재 구매한 타일의 땅 주인 설정)
        _currentTile.photonView.RPC("SetOwner", RpcTarget.All, 0, _playerKey);

        // 관광지 타일 리스트에 현재 타일 추가
        _playerData.AddSeaTile(_currentTile);

        // 관광지 보유 리스트 최신화
        _playerData.RefreshOwnedSeaTiles();
        _seaBuyCount = _playerData.GetOwnedSeaTileCount();

        // 보유한 모든 관광지 타일에 대해 단계별 소유자 등록 (최대 4개까지만)
        for (int i = 0; i < _playerData._ownedSeaTiles.Count && i < 4; i++)
        {
            TileController tile = _playerData._ownedSeaTiles[i];

            tile.photonView.RPC("SetOwner", RpcTarget.All, 0, _playerKey); // 땅
            if (_seaBuyCount >= 2) tile.photonView.RPC("SetOwner", RpcTarget.All, 1, _playerKey); // 펜션
            if (_seaBuyCount >= 3) tile.photonView.RPC("SetOwner", RpcTarget.All, 2, _playerKey); // 콘도
            if (_seaBuyCount >= 4) tile.photonView.RPC("SetOwner", RpcTarget.All, 3, _playerKey); // 호텔
        }

        // UI 갱신
        _tileSheet.text = _seaBuyCount.ToString();
        _tileToll.text = GetTollBySeaCount(_seaBuyCount).ToString();

        Debug.Log($"{_playerKey}번 플레이어가 관광지 {_currentTile._tileName}을 구매했습니다. 현재 보유 수: {_seaBuyCount}");
    }


    /// <summary>
    /// 관광지 보유 수에 따라 통행료 반환
    /// </summary>
    /// <param name="count">보유한 관광지 수</param>
    /// <returns>해당 수에 따른 통행료</returns>
    int GetTollBySeaCount(int count)
    {
        switch (count)
        {
            case 1: return (int)_currentTile._tileLandToll;
            case 2: return (int)_currentTile._tilePensionToll;
            case 3: return (int)_currentTile._tileCondoToll;
            case 4: return (int)_currentTile._tileHotelToll;
            default: return (int)_currentTile._tileHotelToll;
        }
    }
}