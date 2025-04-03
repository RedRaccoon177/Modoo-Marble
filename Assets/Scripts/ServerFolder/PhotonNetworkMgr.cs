using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;

// PhotonNetworkMgr 클래스는 Singleton(싱글톤) 패턴으로 만들어졌음.
// MonoBehaviourPunCallbacks을 상속 받음. 
// Singleton<PhotonNetworkMgr>을 상속받아 전역에서 하나의 인스턴스를 공유함.
public class PhotonNetworkMgr : Singleton<PhotonNetworkMgr>
{
    void Start()
    {
        // 씬 동기화 설정
        // 마스터 클라이언트가 LoadLevel로 씬을 바꾸면 연결된 모든 클라이언트도 같은 씬으로 자동 이동됨
        PhotonNetwork.AutomaticallySyncScene = true;

        // 임시로 창모드로 실행되게 설정
        // 해상도: 1366 x 768 / 전체화면(false) → 창모드
        Screen.SetResolution(1366, 768, false);

        // 포톤 로그 레벨 설정 (로그가 너무 많이 출력되지 않도록 설정)
        // ErrorsOnly: 에러만 출력 (Debug.Log 같은 건 생략됨)
        PhotonNetwork.LogLevel = PunLogLevel.ErrorsOnly; 
        //TODO : 추후 삭제해야댐 무조건
        //Debug.unityLogger.logEnabled = false;

    }

    // 씬 전환 함수
    // 외부에서 이 함수 호출 시, 포톤 네트워크를 통해 씬 전환을 실행함
    // 주의: PhotonNetwork.LoadLevel을 쓰면 AutomaticallySyncScene 설정이 되어 있어야 모든 클라이언트가 동일한 씬으로 이동함 (멀티플레이 동기화 유지)
    public void changeScene(string SceneName)
    {
        PhotonNetwork.LoadLevel(SceneName);
    }
}