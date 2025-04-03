using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
using System.Security.Cryptography;
using System;

//마스터 턴이있고
//턴이 지나면 마스터가 턴을 +1함
//내턴일때만 사용가능
public class TurnMgr : Singleton<TurnMgr>
{
    // ===================== 상태 변수 =====================
    public static int currentTurn = 1;                   // 현재 턴 주인 (ActorNumber 기준)
    public static int leaveNum = 0;                      // 방에서 나간 플레이어 수
    public static bool leave1 = false;
    public static bool leave2 = false;
    public static bool leave3 = false;
    public static bool leave4 = false;

    public int playerlist;                               // 현재 방에 있는 플레이어 수
    public static bool isGameStarted = false;            // 게임 시작 여부

    public int currentRound = 1;                         // 현재 라운드 번호
    int maxRound = 20;                                    // 최대 라운드 수
    [SerializeField] private int turnCountInRound = 0;   // 현재 라운드에서 몇 명이 턴을 종료했는지

    public TextMeshProUGUI _currentRound;

    int loadedPlayers = 0;                               // 로딩 완료된 플레이어 수
    private HashSet<int> loadedPlayerActorNumbers = new HashSet<int>(); // 로딩 완료된 플레이어 목록

    public GameObject[] playerfabs;                      // 플레이어 프리팹 배열 (1~4번)

    public TextMeshProUGUI playerTurnText;               // 내 ActorNumber 표시용 UI
    public TextMeshProUGUI currentTurnText;              // 현재 턴 ActorNumber 표시용 UI

    public static bool _isGameOver = false;               // 모든 코루틴 정지

    // 현재 턴을 관리하는 프로퍼티
    public static int CurrentTurn
    {
        get { return currentTurn; }
        set { currentTurn = (value <= 0) ? 1 : value; }
    }

    private void Start()
    {
        _isGameOver = false;
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string nickname = PhotonNetwork.LocalPlayer.NickName;
        object[] initData = new object[] { actorNumber, nickname };

        // ActorNumber에 따라 각기 다른 프리팹 인스턴스화
        if (actorNumber >= 1 && actorNumber <= 4)
        {
            PhotonNetwork.Instantiate(playerfabs[actorNumber - 1].name, Vector3.zero, Quaternion.identity, 0, initData);
        }

        // 유효하지 않은 ActorNumber일 경우 중단
        if (actorNumber < 1 || actorNumber > 4) return;

        playerTurnText.text = actorNumber.ToString();             // 내 ActorNumber 표시
        playerlist = PhotonNetwork.PlayerList.Length;            // 현재 방에 있는 플레이어 수 저장

        _currentRound.text = $"현재 라운드: {currentRound} / {maxRound}";
    }

    private void Update()
    {
        currentTurnText.text = currentTurn.ToString();           // 현재 턴을 UI에 표시


    }

    // 턴 종료 시 호출되는 함수 (내 턴일 경우에만 작동)
    public void endTurn()
    {
        UIManagerP.instance.OffBuyUIPanel();
        UIManagerP.instance.OffFactorUI();
        UIManagerP.instance.OffClickUI();
        UIManagerP.instance.OffFactorWarningUI();
        UIManagerP.instance.OffTravelUI();

        if (PhotonNetwork.LocalPlayer.ActorNumber == CurrentTurn)
        {
            try
            {
                if (photonView == null) return;

                var player = ServerIngamePlayer._players[currentTurn];

                // 무인도 상태일 경우 턴 스킵
                if (player._isInIsland)
                {
                    if (player._willEscapeIsland)
                    {
                        player._isInIsland = false;
                        player._willEscapeIsland = false;
                    }
                    else
                    {
                        player._islandSkipCount--;
                        if (player._islandSkipCount <= 0)
                            player._isInIsland = false;
                        else
                        {
                            photonView.RPC("IncreaseTurnCount", RpcTarget.All);
                            photonView.RPC("NextTurn", RpcTarget.All);
                            photonView.RPC("DiceUiRPC", RpcTarget.All);
                            return;
                        }
                    }
                }

                player._isSecondCoolTimeG = true;

                photonView.RPC("IncreaseTurnCount", RpcTarget.All);
                photonView.RPC("NextTurn", RpcTarget.All);
                photonView.RPC("DiceUiRPC", RpcTarget.All);
            }
            catch (Exception error)
            {
                Debug.Log(error);
            }
        }
    }


    //턴 수 증가 함수
    [PunRPC]
    void IncreaseTurnCount()
    {
        turnCountInRound++;

        if (turnCountInRound >= PhotonNetwork.PlayerList.Length)
        {
            currentRound++;
            turnCountInRound = 0;

            _currentRound.text = $"현재 라운드: {currentRound} / {maxRound}";

            if (currentRound > maxRound)
            {
                int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                ServerIngamePlayer myPlayer = null;

                if (ServerIngamePlayer._players.ContainsKey(myActorNumber))
                {
                    myPlayer = ServerIngamePlayer._players[myActorNumber];
                }

                // 내 플레이어에서 _startMoney 꺼내기
                double resultStartMoney = (myPlayer != null) ? myPlayer._startMoney : 0;

                // 게임 종료 처리
                GameOverResultWindow gameoverwindow = FindObjectOfType<GameOverResultWindow>();
                gameoverwindow.CreateResultUIs(resultStartMoney);

                UIManagerP.instance.OffBuyUIPanel();
                UIManagerP.instance.OffClickUI();
                UIManagerP.instance.OffDiceUI();
                UIManagerP.instance.OffFactorUI();
                UIManagerP.instance.OffFactorWarningUI();
                UIManagerP.instance.OffTravelUI();

                Time.timeScale = 0f;
                _isGameOver = true;
                return;
            }
        }
    }

    // 내 턴일 때 Dice UI 켜고, 아니면 끔
    [PunRPC]
    public void DiceUiRPC()
    {
        int myActorNum = PhotonNetwork.LocalPlayer.ActorNumber;

        if (currentTurn == myActorNum)
        {
            // Dictionary에 키가 존재할 경우에만 처리
            if (ServerIngamePlayer._players.TryGetValue(myActorNum, out var me))
            {
                if (!me._isInIsland)
                    UIManagerP.instance.OnDiceUI();
                else
                    UIManagerP.instance.OffDiceUI(); // 무인도 상태면 끄기
            }
            else
            {
                Debug.LogWarning($"[DiceUiRPC] 아직 내 플레이어가 등록되지 않음");
            }
        }
        else
        {
            UIManagerP.instance.OffDiceUI();
        }
    }


    // 다음 턴으로 넘기기 위한 순회 로직 (파산한 플레이어 제외)
    [PunRPC]
    void NextTurn()
    {
        int maxPlayer = PhotonNetwork.PlayerList.Length + leaveNum;
        int loopSafe = 0;

        do
        {
            CurrentTurn = (CurrentTurn % maxPlayer) + 1;
            loopSafe++;
            if (loopSafe > 10)
            {
                break; // 무한 루프 방지
            }
        }
        while
        (
            (leave1 && CurrentTurn == 1) ||
            (leave2 && CurrentTurn == 2) ||
            (leave3 && CurrentTurn == 3) ||
            (leave4 && CurrentTurn == 4)
        );
    }



    // 특정 플레이어 턴 정지 (예: 파산, 무인도 등)
    [PunRPC]
    public void StopTurn(int LocalPlayerActorNumber, bool isStop)
    {
        switch (LocalPlayerActorNumber)
        {
            case 1: leave1 = isStop; break;
            case 2: leave2 = isStop; break;
            case 3: leave3 = isStop; break;
            case 4: leave4 = isStop; break;
            default: break;
        }
    }

    // 모든 플레이어 턴 정지 초기화
    [PunRPC]
    public void ResetTurn(bool isStop)
    {
        leave1 = isStop;
        leave2 = isStop;
        leave3 = isStop;
        leave4 = isStop;
    }

    // 각 클라이언트가 로딩 완료되었음을 마스터에게 알림
    [PunRPC]
    public void NotifyMasterPlayerLoaded(int actorNumber)
    {
        if (loadedPlayerActorNumbers.Contains(actorNumber)) return;

        loadedPlayerActorNumbers.Add(actorNumber);
        loadedPlayers++;

        // 모든 플레이어가 준비되었으면 게임 시작 RPC 호출
        if (loadedPlayers >= PhotonNetwork.PlayerList.Length)
        {
            PhotonView.Get(this).RPC("StartTurnSystem", RpcTarget.All);
        }
    }

    // 모든 플레이어가 준비 완료되었을 때 실행되는 턴 시스템 초기화
    [PunRPC]
    void StartTurnSystem()
    {
        currentTurn = 1;
        currentTurnText.text = currentTurn.ToString();
        isGameStarted = true;
    }
}