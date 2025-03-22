using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// 싱글톤 패턴을 구현한 클래스
// 이 클래스는 Photon 기능이 포함된 컴포넌트를 위한 싱글톤
// 예: MonoBehaviourPunCallbacks를 상속한 매니저 클래스에 사용
//Singleton<MonoBehaviourPunCallbacks>   
//Singleton2<MonoBehaviour> 
public class Singleton<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
{
    // 실제 싱글톤 객체를 저장할 정적 변수 (프로젝트 전체에서 공유됨)
    private static T instance;

    // 외부에서 싱글톤 객체를 접근할 수 있는 정적 프로퍼티
    public static T Instance
    {
        get
        {
            // instance가 비어있으면 (처음 접근이면)
            if (instance == null)
            {
                // 씬 안에서 T 타입의 오브젝트를 찾아서 instance에 저장
                instance = (T)FindObjectOfType(typeof(T));

                // 못 찾았으면 새로 생성
                if (instance != null)   // ← 이거 원래 if (instance == null) 이어야 맞음 (아래에서 설명)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T)); // 오브젝트 생성
                    instance = obj.GetComponent<T>(); // 컴포넌트 참조
                }
            }

            // 저장된 instance 반환
            return instance;
        }
    }

    // 유니티에서 오브젝트가 생성될 때 호출됨
    private void Awake()
    {
        if (instance == null)
        {
            // 첫 번째 instance라면 이걸로 저장
            instance = this as T;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않게 유지
        }
        else if (instance != this)
        {
            // 이미 다른 instance가 존재하면 자기 자신을 제거
            Destroy(gameObject);
        }
    }
}


// Photon 없이 일반 MonoBehaviour를 위한 싱글톤
public class Singleton2<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance != null)   // ← 이것도 원래는 instance == null 이어야 맞음
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
