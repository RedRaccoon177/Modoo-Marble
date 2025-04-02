using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class Olympic : MonoBehaviour, IPointerClickHandler
{
    // 타일의 통행료 정보를 관리하는 TileController 컴포넌트 참조
    TileController _tileController;

    // 현재 올림픽 타일이 활성 상태인지 여부를 나타내는 플래그
    private bool isOlympicTileActive = false;

    // 올림픽 타일이 이미 처리되었는지 알리는 이벤트 (중복 클릭 방지용)
    public static event Action<bool> AlreadyCheck;

    // RPC 호출을 위한 PhotonView 컴포넌트 참조
    private PhotonView photonView;

    // 초기화 시 필요한 컴포넌트를 가져옴
    private void Start()
    {
        _tileController = GetComponent<TileController>();
        photonView = GetComponent<PhotonView>();
    }

    // 객체가 활성화될 때 호출되는 메서드
    // 플레이어 위치 변경 및 올림픽 체크 이벤트를 구독
    private void OnEnable()
    {
        ServerIngamePlayer.OnPlayerPositionChanged += CheckOlympicTile;
        ServerIngamePlayer.OlympicCheck += OlymplcCheck;
    }

    // 객체가 비활성화될 때 호출되는 메서드
    // 이벤트 구독 해제하여 메모리 누수 방지
    private void OnDisable()
    {
        ServerIngamePlayer.OnPlayerPositionChanged -= CheckOlympicTile;
        ServerIngamePlayer.OlympicCheck -= OlymplcCheck;
    }

    // 플레이어의 위치가 바뀔 때 호출되는 콜백 메서드
    // 현재 위치가 올림픽 타일(인덱스 20)인지 확인하고 플래그 설정
    // 변경된 위치에 따라 올림픽 타일 도착 여부 판단
    private void CheckOlympicTile(int actorNumber, int playerPosIndex)
    {
        // 내 캐릭터가 아니면 무시
        if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber) return;

        if (playerPosIndex == 20)
        {
            isOlympicTileActive = true;
        }
        else
        {
            isOlympicTileActive = false;
        }
    }

    // 올림픽 타일 상태를 외부로부터 받아와 업데이트하는 메서드
    private void OlymplcCheck(bool check)
    {
        isOlympicTileActive = check;
    }

    // 유저가 타일을 클릭했을 때 호출되는 메서드
    public void OnPointerClick(PointerEventData eventData)
    {
        // 올림픽 타일이 활성 상태일 경우에만 실행
        if (isOlympicTileActive)
        {
            // 모든 클라이언트에게 올림픽 효과 적용을 요청하는 RPC 호출
            photonView.RPC("ApplyOlympicEffect", RpcTarget.All);

            // 클릭 중복 방지를 위해 이벤트를 통해 상태 전달
            AlreadyCheck?.Invoke(false);
        }
    }

    // 모든 클라이언트에서 실행되는 RPC 메서드
    // 각 건물의 통행료를 2배로 증가시킴
    [PunRPC]
    void ApplyOlympicEffect()
    {
        _tileController._tileLandToll *= 2;
        _tileController._tilePensionToll *= 2;
        _tileController._tileCondoToll *= 2;
        _tileController._tileHotelToll *= 2;
        _tileController._tileLandMarkToll *= 2;

        Debug.Log("모든 유저에게 올림픽 효과 적용됨");
    }
}
