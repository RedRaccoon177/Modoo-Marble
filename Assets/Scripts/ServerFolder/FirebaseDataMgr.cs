//최동오
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using System;

public class FirebaseDataMgr : MonoBehaviourPunCallbacks
{
    public static FirebaseDataMgr Instance { get; private set; }
    private DatabaseReference dbReference;
    public int userMoney = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(async task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;

            if (FirebaseLoginMgr.user != null)
            {
                //TODO:초기 저장 바꾸기 12000
                //SaveUserData(FirebaseLoginMgr.user.DisplayName, "money", 12000);
                userMoney = await LoadUserDataAsync(FirebaseLoginMgr.user.DisplayName, "money", userMoney);
                if (userMoney == -1)
                {
                    SaveUserData(FirebaseLoginMgr.user.DisplayName, "money", 10000);
                    userMoney = 10000;
                }
                Debug.Log("유저 닉네임 : " + FirebaseLoginMgr.user.DisplayName);
                Debug.Log("유저 돈 : " + userMoney);
            }
            else
            {
                Debug.LogError("파이어베이스 문제");
            }
        });
    }

    //데이터 저장 함수
    //SaveUserData(id,"level",5);
    //id의 레벨은 5 추가됌
    //ContinueWithOnMainThread 메인쓰레드에서 함
    public void SaveUserData<T>(string userId, string dataName, T value) 
    {
        dbReference.Child("users").Child(userId).Child(dataName).SetValueAsync(value).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log(userId+ "의" + dataName +  value+"추가됨");
            }
            else
            {
                Debug.LogError("실패함");
            }
        });
    }

    //데이터 불러오기 함수
    //함수쓸때 앞에 await 붙여야댐
    //playerLevel = await LoadUserDataAsync(id, "level", useLevel);
    //id의 레벨 불러오고 playerLevel 변수에 담음
    //playerLevel =  데이터value; 이런식
    //await할때까지 기달림
    //https://ljhyunstory.tistory.com/284 
    public async Task<T> LoadUserDataAsync<T>(string userId, string dataName, T type)
    {
        // 비동기적으로 데이터 불러오기
        DataSnapshot snapshot = await dbReference.Child("users").Child(userId).Child(dataName).GetValueAsync();
        T Tvalue;

        try
        {
            Tvalue = type;
            if (snapshot.Exists)
            {
                //타입을 바꿔서 집어넣음
                Tvalue = (T)Convert.ChangeType(snapshot.Value, typeof(T));
                Debug.Log(userId + "의 " + dataName + "불러옴");
                Debug.Log("Tvalue : " + Tvalue);
            }
            else
            {
                Debug.Log("저장된 데이터 없음"); 
            }
        }
        catch (System.Exception dd)
        {
            Debug.Log(dd);
            Tvalue = type;
        }

        return Tvalue;
    }
}

