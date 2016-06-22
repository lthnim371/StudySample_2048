using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;


public class FBscript : MonoBehaviour {

    public GameObject DialogLoggedIn;
    public GameObject DialogLoggedOut;
    public GameObject DialogUsername;
    public GameObject DialogProfilePic;

    void Awake()
    {        
        FB.Init(SetInit, OnHideUnity);
    }

    void SetInit()
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

    //로그인 - 로그인버튼 같은거 클릭 했을 때 호출
    public void FBlogin()
    {
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
         
        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }

    void AuthCallBack(IResult result)
    {
        if (result.Error != null)
            print(result.Error);
        else
        {
            if (FB.IsLoggedIn)
                print("FB is logged in");
            else
                print("FB is Not logged in");

            DealWitheFBMenus(FB.IsLoggedIn);
        }
    }

    //로그인 버튼 활성화 결정
    void DealWitheFBMenus(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            DialogLoggedIn.SetActive(true);
            DialogLoggedOut.SetActive(false);

            //접속자의 정보 가져오는 것 같음
            FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
            //접속자의 프로필사진 가져오는 것 같음
            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
        }
        else
        {
            DialogLoggedIn.SetActive(false);
            DialogLoggedOut.SetActive(true);
        }
    }

    //로그인 할 때 접속자 이름 출력 - 오토콜백용도인거같음
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
    }
}
