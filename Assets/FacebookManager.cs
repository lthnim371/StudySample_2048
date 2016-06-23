using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class FacebookManager : MonoBehaviour
{

    private static FacebookManager _instance;
    public static FacebookManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject fbm = new GameObject("FBManager");
                fbm.AddComponent<FacebookManager>();
            }

            return _instance;
        }
    }

    public bool IsLoggedIn { get; set; }
    public string ProfileName { get; set; }
    public Sprite ProfilePic { get; set; }
    public string AppLinkURL { get; set; }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        _instance = this;

        IsLoggedIn = true;
    }

    public void InitFB()
    {
        if (!FB.IsInitialized) //페이스북 사용할 준비 여부 같아보임
            FB.Init(SetInit, OnHideUnity); //그래서 사용하기 위해 준비하는것같음
        else //준비가 되어 있다면
            IsLoggedIn = FB.IsLoggedIn; //로그인상태 받아오는것같음
    }

    void SetInit()
    {
        if (FB.IsLoggedIn)
        {
            GetProfile();
            print("FB is logged in");
        }
        else
            print("FB is Not logged in");

        //DealWitheFBMenus(FB.IsLoggedIn);
        IsLoggedIn = FB.IsLoggedIn;
    }

    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void GetProfile()
    {
        //접속자의 정보 가져오는 것 같음
        FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
        //접속자의 프로필사진 가져오는 것 같음
        FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);

        FB.GetAppLink(DealWithAppLink);
    }

    //로그인 할 때 접속자 이름 출력 - 다른 함수의 오토콜백용도인거같음
    void DisplayUsername(IResult result)
    {
        if (result.Error == null)
            ProfileName =
                "" + result.ResultDictionary["first_name"]; //"id" 도 있음.
        else
            Debug.Log(result.Error);
    }

    void DisplayProfilePic(IGraphResult result)
    {
        if (result.Texture != null)
        {
            ProfilePic =
                Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
        }
    }

    void DealWithAppLink(IAppLinkResult result)
    {
        if (!String.IsNullOrEmpty(result.Url))
        {
            AppLinkURL = "" + result.Url + "";
            Debug.Log(AppLinkURL);
        }
        else
            AppLinkURL = "http://google.com";
    }

    public void Share()
    {
        FB.FeedShare(
            string.Empty,
            new Uri(AppLinkURL),
            "Hello this is the title",
            "this is the caption",
            "check out this game",
            new Uri("http://postfiles13.naver.net/20160623_28/taehoon0720_1466663089836DJniD_JPEG/Pensive_Parakeet.jpg?type=w3"),
            string.Empty,
            ShareCallBack);
    }
    
    void ShareCallBack(IResult result)
    {
        if (result.Cancelled)
            Debug.Log("Share Cancelled");
        else if (!String.IsNullOrEmpty(result.Error))
            Debug.Log("Error on share!");
        else if (!String.IsNullOrEmpty(result.RawResult))
            Debug.Log("Success on share");
    }

    public void Invite()
    {
        FB.Mobile.AppInvite(
            new Uri(AppLinkURL),
            new Uri("http://postfiles13.naver.net/20160623_28/taehoon0720_1466663089836DJniD_JPEG/Pensive_Parakeet.jpg?type=w3"),
            InviteCallBack);
    }

    void InviteCallBack(IResult result)
    {
        if (result.Cancelled)
            Debug.Log("Invite Cancelled");
        else if (!String.IsNullOrEmpty(result.Error))
            Debug.Log("Error on Invite");
        else if (!String.IsNullOrEmpty(result.RawResult))
            Debug.Log("Success on Invite");
    }

    public void ShareWithUsers()
    {
        FB.AppRequest(
            "Come and join me, i bet you can't beat my score!",
            null,
            new List<object>(),
            null,
            null,
            null,
            null,
            ShareWithUsersCallBack);
    }

    void ShareWithUsersCallBack(IAppRequestResult result)
    {
        if (result.Cancelled)
            Debug.Log("Challenge Cancelled");
        else if (!String.IsNullOrEmpty(result.Error))
            Debug.Log("Error on Challenge");
        else if (!String.IsNullOrEmpty(result.RawResult))
            Debug.Log("Success on Challenge");
    }
}