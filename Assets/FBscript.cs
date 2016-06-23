using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;

//주석처리는 역할이 페이스북매니저로 대체되었기 때문.
public class FBscript : MonoBehaviour
{

    public GameObject DialogLoggedIn;
    public GameObject DialogLoggedOut;
    public GameObject DialogUsername;
    public GameObject DialogProfilePic;

    void Awake()
    {
        //일단 임시 해상도 설정
        Screen.SetResolution(Screen.width, (int)((Screen.width * 0.5f) * 3f), true);


        //FB.Init(SetInit, OnHideUnity);
        //Debug.Log(FacebookManager.Instance.IsLoggedIn);

        FacebookManager.Instance.InitFB();
        DealWithFBMenus(FB.IsLoggedIn);
    }

    /*void SetInit()
    {
        if (FB.IsLoggedIn)
            print("FB is logged in");
        else
            print("FB is Not logged in");

        DealWitheFBMenus(FB.IsLoggedIn);
    }

    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    로그인 - 로그인버튼 같은거 클릭 했을 때 호출*/

    //로그인버튼 클릭시 호출됨
    public void FBlogin()
    {
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");

        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }

    public void Share()
    {
        FacebookManager.Instance.Share();
    }

    public void Invite()
    {
        FacebookManager.Instance.Invite();
    }

    public void ShareWithUsers()
    {
        FacebookManager.Instance.ShareWithUsers();
    }

    void AuthCallBack(IResult result)
    {
        if (result.Error != null)
            print(result.Error);
        else
        {
            if (FB.IsLoggedIn)
            {
                FacebookManager.Instance.IsLoggedIn = true;
                FacebookManager.Instance.GetProfile();
                print("FB is logged in");
            }
            else
                print("FB is Not logged in");

            DealWithFBMenus(FB.IsLoggedIn);
        }
    }

    //로그인 버튼 활성화 결정
    void DealWithFBMenus(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            DialogLoggedIn.SetActive(true);
            DialogLoggedOut.SetActive(false);

            ////접속자의 정보 가져오는 것 같음
            //FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
            ////접속자의 프로필사진 가져오는 것 같음
            //FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);

            if (FacebookManager.Instance.ProfileName != null)
            {
                Text userName = DialogUsername.GetComponent<Text>();
                userName.text = "Hi, " + FacebookManager.Instance.ProfileName;
            }
            else
                StartCoroutine("WaitForProfileName");

            if (FacebookManager.Instance.ProfilePic != null)
            {
                Image ProfilePic = DialogProfilePic.GetComponent<Image>();
                ProfilePic.sprite = FacebookManager.Instance.ProfilePic;
            }
            else
                StartCoroutine("WaitForProfilePic");

        }
        else
        {
            DialogLoggedIn.SetActive(false);
            DialogLoggedOut.SetActive(true);
        }
    }

    /*//로그인 할 때 접속자 이름 출력 - 오토콜백용도인거같음
    void DisplayUsername(IResult result)
    {
        Text UserName = DialogUsername.GetComponent<Text>();

        if (result.Error == null)
            UserName.text =
                "Hi there, " + result.ResultDictionary["first_name"]; //"id" 도 있음.
        else
            Debug.Log(result.Error);
    }

    void DisplayProfilePic(IGraphResult result)
    {
        if (result.Texture != null)
        {
            Image ProfilePic =
                DialogProfilePic.GetComponent<Image>();
            ProfilePic.sprite =
                Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
        }
    }*/

    IEnumerator WaitForProfileName()
    {
        while (FacebookManager.Instance.ProfileName == null)
            yield return null;

        DealWithFBMenus(FB.IsLoggedIn);
    }

    IEnumerator WaitForProfilePic()
    {
        while (FacebookManager.Instance.ProfilePic == null)
            yield return null;

        DealWithFBMenus(FB.IsLoggedIn);
    }
}
