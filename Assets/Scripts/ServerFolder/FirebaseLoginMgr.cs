// 최동오
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Threading.Tasks;

// 로그인과 회원가입, 닉네임 설정 등을 관리하는 클래스
public class FirebaseLoginMgr : MonoBehaviour
{
    // 현재 로그인한 유저 정보
    static public FirebaseUser user;
    // 파이어베이스 인증 객체
    static public FirebaseAuth auth;

    // 로그인용 입력필드 및 경고 텍스트
    [Header("로그인용")]
    [SerializeField] private InputField LoginIdInputField;       // 로그인 이메일 입력필드
    [SerializeField] private InputField LoginPasswordInputField; // 로그인 비밀번호 입력필드
    [SerializeField] Text LoginwarningText;                      // 로그인 경고 메시지

    // 회원가입용 입력필드 및 경고 텍스트
    [Header("회원가입용")]
    [SerializeField] private InputField CreateIdInputField;       // 회원가입 이메일 입력필드
    [SerializeField] private InputField CreatePasswordInputField; // 회원가입 비밀번호 입력필드
    [SerializeField] Text CreatewarningText;                      // 회원가입 경고 메시지

    // 닉네임 설정용 입력필드 및 경고 텍스트
    [Header("닉네임 설정용")]
    [SerializeField] private InputField NickNameInputField; // 닉네임 입력필드
    [SerializeField] Text NickNamewarningText;              // 닉네임 설정 경고 메시지

    // UI 패널 관리용 오브젝트들
    [Header("큰테두리Ui")]
    [SerializeField] private GameObject SceneChanege;       // 씬 전환 UI
    [SerializeField] private GameObject LoginUiPanel;       // 로그인 UI 전체 패널
    [SerializeField] private GameObject CreateUiIdPanel;    // 회원가입 UI 패널
    [SerializeField] private GameObject NickNameUiPanel;    // 닉네임 설정 UI 패널

    private void Awake()
    {
        // 파이어베이스 초기화 (비동기 방식)
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            DependencyStatus dependencyStatus = task.Result; // 결과 상태 가져오기
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // 인증 인스턴스 가져오기
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            }
        });
        // 경고 텍스트 초기화
        CreatewarningText.text = "";
        LoginwarningText.text = "";
    }

    // 회원가입 패널로 전환
    public void CreateIdPanel()
    {
        CreateUiIdPanel.gameObject.SetActive(true);
        LoginUiPanel.gameObject.SetActive(false);
        NickNameUiPanel.gameObject.SetActive(false);
    }

    // 회원가입 패널 닫고 로그인 패널로 전환
    public void CreateIdPanelFalse()
    {
        LoginUiPanel.gameObject.SetActive(true);
        CreateUiIdPanel.gameObject.SetActive(false);
    }

    // 닉네임 설정 패널로 전환
    public void NickNamePanel()
    {
        CreateUiIdPanel.gameObject.SetActive(false);
        LoginUiPanel.gameObject.SetActive(false);
        NickNameUiPanel.gameObject.SetActive(true);
    }

    // 로그인 패널로 전환
    public void LoginPanel()
    {
        CreateUiIdPanel.gameObject.SetActive(false);
        LoginUiPanel.gameObject.SetActive(true);
        NickNameUiPanel.gameObject.SetActive(false);
    }

    // 회원가입 버튼 눌렀을 때 호출
    public void CreateId()
    {
        StartCoroutine(CreateIdCor(CreateIdInputField.text, CreatePasswordInputField.text));
    }

    // 로그인 버튼 눌렀을 때 호출
    public void Login()
    {
        StartCoroutine(LoginCor(LoginIdInputField.text, LoginPasswordInputField.text));
    }

    // 로그아웃
    public void Logout()
    {
        auth.SignOut(); // 인증 객체에서 로그아웃 실행
        Debug.Log("로그 아웃");
    }

    // 닉네임 생성 버튼 눌렀을 때 호출
    public void CreateNickName()
    {
        StartCoroutine(CreateNickNameCor(NickNameInputField.text));
    }

    // 닉네임 설정 코루틴
    IEnumerator CreateNickNameCor(string NickName)
    {
        if (user != null)
        {
            // 닉네임 정보를 UserProfile로 생성
            UserProfile profile = new UserProfile { DisplayName = NickName };
            // 닉네임 서버에 등록 요청
            Task ProfileTask = user.UpdateUserProfileAsync(profile);

            yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

            if (ProfileTask.Exception != null)
            {
                Debug.LogWarning(message: "닉네임 설정 실패" + ProfileTask.Exception);
                FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                NickNamewarningText.text = "닉네임 설정 실패";
            }
            else
            {
                NickNamewarningText.text = "";
                Debug.Log("닉네임 : " + user.DisplayName);
                var dd = string.IsNullOrEmpty(user.DisplayName);
                if (dd != true) // 닉네임이 존재할 경우
                {
                    NickNameUiPanel.gameObject.SetActive(false);
                    SceneChanege.gameObject.SetActive(true);
                }
            }
        }
    }

    // 회원가입 처리 코루틴
    IEnumerator CreateIdCor(string email, string password)
    {
        var createIdTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => createIdTask.IsCompleted);

        if (createIdTask.Exception != null)
        {
            Debug.LogWarning(message: "다음과 같은 이유로 회원가입 실패:" + createIdTask.Exception);
            FirebaseException firebaseEx = createIdTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = " 회원가입 실패";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "이메일 누락";
                    break;
                case AuthError.MissingPassword:
                    message = "패스워드 누락";
                    break;
                case AuthError.WeakPassword:
                    message = "패스워드 약함";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "중복 이메일";
                    break;
                default:
                    message = "관리자에게 문의 바랍니다";
                    break;
            }
            CreatewarningText.text = message;
        }
        else
        {
            Debug.Log("회원가입 완료");
            user = createIdTask.Result.User; // 유저 정보 저장
            CreatewarningText.text = "";
            LoginUiPanel.gameObject.SetActive(true);
            CreateUiIdPanel.gameObject.SetActive(false);
        }
    }

    // 로그인 처리 코루틴
    IEnumerator LoginCor(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning(message: "다음과 같은 이유로 로그인 실패:" + loginTask.Exception);
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = "로그인 실패";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "이메일 누락";
                    break;
                case AuthError.MissingPassword:
                    message = "패스워드 누락";
                    break;

                case AuthError.WrongPassword:
                    message = "패스워드 틀림";
                    break;
                case AuthError.InvalidEmail:
                    message = "이메일 형식이 옳지 않음";
                    break;
                case AuthError.UserNotFound:
                    message = "아이디가 존재하지 않음";
                    break;
                default:
                    message = "관리자에게 문의 바랍니다";
                    break;
            }
            LoginwarningText.text = message;
        }
        else
        {
            Debug.Log("로그인 완료");
            user = loginTask.Result.User;
            LoginwarningText.text = "";
            LoginUiPanel.gameObject.SetActive(false);
            Debug.Log(user.DisplayName);

            // 닉네임이 없다면 닉네임 설정 패널로 이동
            if (string.IsNullOrEmpty(user.DisplayName) == true)
            {
                Debug.Log("닉네임이 없습니다");
                NickNamePanel();
                CreateNickName();
            }
            else
            {
                SceneChanege.gameObject.SetActive(true);
            }
        }
    }
}
