using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using Sample_2048;

public class GPGSManager : Singleton<GPGSManager> {

    public long Score { get; set; }
    public UILabel loggedLabel;
    public UILabel testLabel;

    void Start()
    {
        this.Initialize();
    }

    public void Initialize()
    {
        print("프로퍼티 테스트...현재 점수는 " + this.Score.ToString());
        this.Score = 12345;

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        testLabel.text = "초기화 하였다";
    }

    public void Login()
    {
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    loggedLabel.text = "Login Success";
                    print("Login Success");
                }
                else
                {
                    loggedLabel.text = "Login Failure";
                    print("Login Failure");
                }
            });
        }
    }

    public void Logout()
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.SignOut();
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

    public void UnlockAchievement()
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportProgress(Sample_2048Class.achievement_test_achievement, 100f, (bool success) =>
            {
                if (success)
                    print("UnlockAchievement Success");
                else
                    print("UnlockAchievement Failure");
            });
        }//if
    }

    public void IncrementalAchievement()
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(Sample_2048Class.achievement_incremental_achivement, 5
                , (bool success) =>
                {
                    if (success)
                        print("IncrementalAchievement Success");
                    else
                        print("IncrementalAchievement Failure");
                });
        }//if
    }

    public void PostingLeaderboard()
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(this.Score, Sample_2048Class.leaderboard_awesome_leaderboard_for_cool_people
                , (bool success) =>
                {
                    if (success)
                        print("PostingLeaderboard Success");
                    else
                        print("PostingLeaderboard Failure");
                });
        }//if
    }

    public void ShowAchievement()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowAchievementsUI();
        }//if
    }

    public void ShowLeaderboard()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }//if
    }

    public void ShowSpecificLeaderboard()
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(
                Sample_2048Class.leaderboard_awesome_leaderboard_for_cool_people);
        }//if
    }


}
