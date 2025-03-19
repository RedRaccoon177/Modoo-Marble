using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;

public class PhotonNetworkMgr : Singleton<PhotonNetworkMgr>
{
    private void Start()
    {
        // 씬 동기화 설정 (마스터 클라이언트가 씬을 변경하면 자동으로 동기화됨)
        PhotonNetwork.AutomaticallySyncScene = true;
        //임시 창모드
        Screen.SetResolution(1366, 768, false);

        PhotonNetwork.LogLevel = PunLogLevel.ErrorsOnly;

    }
    

    public void changeScene(string SceneName)
    {
        PhotonNetwork.LoadLevel(SceneName);
    }


}
