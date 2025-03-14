using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;

public class PhotonNetworkMgr : MonoBehaviourPunCallbacks
{
    static PhotonNetworkMgr instance = null;

    public static PhotonNetworkMgr Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PhotonNetworkMgr();
            }
            return instance;
        }
    }

    //임시 디버그 확인용
    public Text debugText;
    public ScrollRect scrollRect;

    private void Start()
    {
        // 씬 동기화 설정 (마스터 클라이언트가 씬을 변경하면 자동으로 동기화됨)
        PhotonNetwork.AutomaticallySyncScene = true;
        //임시 창모드
        Screen.SetResolution(1366, 768, false);

        //임시 디버그용 창모드
        Application.logMessageReceived += HandleLog;
    }
    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logMessage = $"[{type}] {logString}\n"; // 로그 타입과 메시지 조합

        if (debugText != null)
        {
            debugText.text += logMessage; // UI Text에 추가
        }
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;

    }

    public override void OnLeftRoom() //방 나가면 알아서 호출
    {
        SceneManager.LoadScene(0); //타이틀로 이동
    }

    public void changeScene()
    {

        PhotonNetwork.LoadLevel("SampleScene");

    }


}
