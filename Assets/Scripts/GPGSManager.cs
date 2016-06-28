using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using Sample_2048;

public class GPGSManager : Singleton<GPGSManager> {
        
    //public long Score { get; set; }
    public UILabel loggedLabel;
    public UILabel leaderboardUpdateLabel;
    public UILabel profileLabel;
    public UILabel achieveLabel;

    //일단 임시로 바로 준비하게끔하자
    protected override void Awake()
    {
        this.gameObject.name = "_GPGSManager";
        instance = this;

        this.Initialize();
    }

    //구글플레이게임서비스 사용 준비
    public void Initialize()
    {
        //옵션 설정?
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .Build();

        PlayGamesPlatform.InitializeInstance(config); //초기화?
        PlayGamesPlatform.DebugLogEnabled = true; //디버그 로그 기록 작성?
        PlayGamesPlatform.Activate(); //사용준비 활성화?
    }

    public bool IsLogged()
    {
        return Social.localUser.authenticated;
    }

    //구글플레이게임에 로그인
    public void Login()
    {
        if (!Social.localUser.authenticated) //로그인여부 확인(로그아웃상태이면 false 반환이므로 ! not연산 붙임)
        {
            //로그인 시도함(매개변수는 콜백형 함수임)
            Social.localUser.Authenticate((bool success) =>
            {
                if (success) //로그인 성공
                {
                    loggedLabel.text = "Login Success";
                    print("Login Success");
                }
                else //로그인 실패
                {
                    this.Initialize(); //혹시 모르니 초기화
                    loggedLabel.text = "Login Failure";
                    print("Login Failure");
                }
            });
        }//if
    }

    //구글플레이게임 로그아웃
    public void Logout()
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.SignOut();
            loggedLabel.text = "Logout Succes";
        }
    }

    public void PlayerStatistics()
    {
        if (Social.localUser.authenticated)
        {
            ((PlayGamesLocalUser)Social.localUser).GetStats((rc, stats) =>
            {
                if (rc <= 0 && stats.HasDaysSinceLastPlayed())
                {
                    Debug.Log("It has been " + stats.DaysSinceLastPlayed + " days");
                }
            });
        }//if
    }

    //업적 해제
    public void UnlockAchievement(string achievementName)
    {
        if (Social.localUser.authenticated)
        {
            //업적 달성 정보 전달하기(업적명(ID), 진행상태(0f 또는 100f), 콜백형함수)
            Social.ReportProgress(achievementName, 100f, (bool success) =>
            {
                if (success)
                {
                    print("UnlockAchievement Success");
                    achieveLabel.text = "UnlockAchievement Success";
                }
                else
                {
                    print("UnlockAchievement Failure");
                    achieveLabel.text = "UnlockAchievement Failure";
                }
            });
        }//if
    }

    //증가형 업적 갱신
    public void IncrementalAchievement(string achievementName)
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(achievementName, 1, (bool success) =>
            {
                if (success)
                {
                    print("IncrementalAchievement Success");
                    achieveLabel.text = "IncrementalAchievement Success";
                }
                else
                {
                    print("IncrementalAchievement Failure");
                    achieveLabel.text = "IncrementalAchievement Failure";
                }
            });
        }//if
    }

    //리더보드 점수 갱신하기?
    public void PostingLeaderboard(long score)
    {
        if (Social.localUser.authenticated)
        {
            //리더보드 점수 갱신(점수, 리더보드명(ID), 콜백형함수)
            Social.ReportScore(score, Sample_2048Class.leaderboard_awesome_leaderboard
                , (bool success) =>
                {
                    if (success)
                    {
                        print("PostingLeaderboard Success");
                        leaderboardUpdateLabel.text = "Leaderboard Update Success";
                    }
                    else
                    {
                        print("PostingLeaderboard Failure");
                        leaderboardUpdateLabel.text = "Leaderboard Update Failure";
                    }
                });
        }//if
    }

    //업적목록 보여주기
    public void ShowAchievement()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowAchievementsUI();
        }//if
        else
            this.Login();
    }

    //전체 리더보드목록 보여주기
    public void ShowLeaderboard()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }//if
        else
            this.Login();
    }

    //특정 리더보드 상세하게 보여주기
    public void ShowSpecificLeaderboard()
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(
                Sample_2048Class.leaderboard_awesome_leaderboard);
        }//if
        else
            this.Login();
    }

    public Texture2D GetProfileImage()
    {
        if (Social.localUser.authenticated)
        {
            profileLabel.text = "Get Profile Image";
            return Social.localUser.image;
        }
        else
        {
            profileLabel.text = "Fail Profile Image";
            return null;
        }
    }

    public string GetProfileName()
    {
        if (Social.localUser.authenticated)
        {
            profileLabel.text = "Get Profile Name";
            return Social.localUser.userName;
        }
        else
        {
            profileLabel.text = "Fail Profile Name";
            return null;
        }
    }

}
