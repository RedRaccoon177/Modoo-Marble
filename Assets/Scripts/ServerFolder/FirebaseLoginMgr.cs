//최동오
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Threading.Tasks;

//로그인창
//임시 나중에 리스트로 담아서 해야될듯 지금은 기능 구현 우선
public class FirebaseLoginMgr : MonoBehaviour
{
    //유저 
    static public FirebaseUser user;
    static public FirebaseAuth auth;

    //임시
    //로그인용
    [Header("로그인용")]
    [SerializeField] private InputField LoginIdInputField;
    [SerializeField] private InputField LoginPasswordInputField;
    [SerializeField] Text LoginwarningText;
    //회원가입용
    [Header("회원가입용")]
    [SerializeField] private InputField CreateIdInputField;
    [SerializeField] private InputField CreatePasswordInputField;
    [SerializeField] Text CreatewarningText;
    [Header("닉네임 설정용")]
    [SerializeField] private InputField NickNameInputField;
    [SerializeField] Text NickNamewarningText;
    //nickField.text = user.DisplayName; 따로만들자 

    [Header("큰테두리Ui")]
    [SerializeField] private GameObject SceneChanege;
    [SerializeField] private GameObject LoginUiPanel;
    [SerializeField] private GameObject CreateUiIdPanel;
    [SerializeField] private GameObject NickNameUiPanel;





    private void Awake()
    {
        //안전코드 auth연결
        //비동기식 ContinueWith
        //파이어베이스 초기화
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            }
        });
        CreatewarningText.text = "";
        LoginwarningText.text = "";
    }

    //지금은 3개 만들자 헷갈린다
    //회원가입패널로 넘어감
    public void CreateIdPanel()
    {
        //회원가입패널
        CreateUiIdPanel.gameObject.SetActive(true);
        LoginUiPanel.gameObject.SetActive(false);
        NickNameUiPanel.gameObject.SetActive(false);

    }

    //닉네임패널로 넘어감
    public void NickNamePanel()
    {
        //회원가입패널
        CreateUiIdPanel.gameObject.SetActive(false);
        LoginUiPanel.gameObject.SetActive(false);
        NickNameUiPanel.gameObject.SetActive(true);

    }

    //로그인패널로 넘어감
    public void LoginPanel()
    {
        //회원가입패널
        CreateUiIdPanel.gameObject.SetActive(false);
        LoginUiPanel.gameObject.SetActive(true);
        NickNameUiPanel.gameObject.SetActive(false);

    }



    //회원가입
    public void CreateId()
    {
        StartCoroutine(CreateIdCor(CreateIdInputField.text, CreatePasswordInputField.text));
    }

    //로그인
    public void Login()
    {
        StartCoroutine(LoginCor(LoginIdInputField.text, LoginPasswordInputField.text));
        SceneChanege.gameObject.SetActive(true);
    }

    //로그아웃
    public void Logout()
    {
        auth.SignOut();
        Debug.Log("로그 아웃");
    }

    public void CreateNickName()
    {
        StartCoroutine(CreateNickNameCor(NickNameInputField.text));
        SceneChanege.gameObject.SetActive(true);

    }
    IEnumerator CreateNickNameCor(string NickName)
    {
        if (user != null)
        {
            //닉네임
            UserProfile profile = new UserProfile { DisplayName = NickName };
            //파이어베이스에 닉네임 정보 올림
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
            }
        }

    }


    //동기식 회원가입 코루틴
    IEnumerator CreateIdCor(string email, string password)
    {
        var createIdTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        //회원가입 성공할때 까지
        yield return new WaitUntil(predicate: () => createIdTask.IsCompleted);
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
            user = createIdTask.Result.User;
            CreatewarningText.text = "";
            LoginUiPanel.gameObject.SetActive(true);
            CreateUiIdPanel.gameObject.SetActive(false);
        }

    }

    //동기식 로그인 코루틴 
    IEnumerator LoginCor(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        //로그인 성공할때 까지
        yield return new WaitUntil(predicate: ()=> loginTask.IsCompleted);
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


            //닉네임이 없을경우 닉네임 생성
            if (string.IsNullOrEmpty(user.DisplayName) == true)
            {
                Debug.Log("닉네임이 없습니다");
                NickNamePanel();
                CreateNickName();
                //ServerPanel.gameObject.SetActive(true);
            }
            else
            {
                SceneChanege.gameObject.SetActive(true);

            }
        }



    }




}
