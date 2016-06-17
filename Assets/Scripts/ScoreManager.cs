using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    private static ScoreManager sInstance;
    public static ScoreManager Instance
    {
        get
        {
            if (sInstance == null)
            {
                GameObject newGameObj = new GameObject("_ScoreManager");
                sInstance = newGameObj.AddComponent<ScoreManager>();
            }

            return sInstance;
        }
    }

    public UILabel bestScoreLabel;
    public UILabel currentScoreLabel;
    public UILabel gameOverLabel;
    //private string scoreText = "Score";
    private int currentScore = 0;
    private int bestScore = 0;

    void Awake()
    {
        sInstance = this;

        /* 추후에 씬뷰에 미리 생성 없이 사용시 해제 바람
        GameObject findGameObj = GameObject.FindGameObjectWithTag("UI");
        if(findGameObj == null)
        {
            print("UI태그의 게임오브젝트를 못 찾았다.");
            return;
        }
        Transform findTM = findGameObj.transform.FindChild("Camera/Anchor/TextPanel");
        this.scoreLabel = findTM.FindChild("ScoreLabel").GetComponent<UILabel>();
        this.GameOverLabel = findTM.FindChild("GameOverLabel").GetComponent<UILabel>();
        if (this.scoreLabel == null || this.GameOverLabel == null)
        {
            print("UILabel컴포넌트를 못 찾았다.");
            return;
        }

        this.GameOverLabel.gameObject.SetActive(false);
        */

        if (this.bestScoreLabel == null || this.currentScoreLabel == null)
            print("점수 라벨을 물려놓지 않았다.");

        this.gameOverLabel.gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {

        //시작 시 최초 점수로 셋팅
        //this.scoreLabel.text =
        //  string.Format("{0} : {1}", this.scoreText, this.currentScore);
        this.currentScoreLabel.text = "0";
        this.bestScoreLabel.text = this.bestScore.ToString();
    }
	
    //현재 점수에 점수 추가하기
    //public void AddScore(int numberLevel)
    public void AddScore(NumberLevel numberLevel)
    {
        //if (numberLevel <= 0) //0 이하로 전달이 오면 그냥 점수 초기화..
        if (numberLevel <= NumberLevel.NONE) //0 이하로 전달이 오면 일단은 그냥 점수 초기화..
            this.currentScore = 0;
        else
        {
            //그냥 2의 제곱값으로 점수 추가하기...
            int newScore = (int)Mathf.Pow(2, (float)numberLevel + 1);
            this.currentScore += newScore;
        }

        //this.scoreLabel.text =
        //  string.Format("{0} : {1}", this.scoreText, this.currentScore);
        this.currentScoreLabel.text = this.currentScore.ToString();
    }

    //현재 점수 초기화.
    public void InitialScore()
    {
        this.ChangeScore(); //초기화하기전에 혹시모르니 최종점수 갱신해보자

        this.currentScore = 0;
        this.currentScoreLabel.text = "0";
        //(아마 게임오버시 호출될거 같으니 게임오버라벨 비활성해놓자)
        if (this.gameOverLabel.gameObject.activeSelf == true)
            this.gameOverLabel.gameObject.SetActive(false);
    }

    //게임오버라벨 활성화하기
    public void ActiveGameOverText()
    {
        this.gameOverLabel.gameObject.SetActive(true);
        this.ChangeScore(); //최종점수 변경
    }

    //최종 점수 변경하기
    private void ChangeScore()
    {
        //현재 점수가 최종점수보다 크다면 변경
        if (this.currentScore > this.bestScore)
        {
            this.bestScore = this.currentScore;
            this.bestScoreLabel.text = this.currentScore.ToString();
        }
    }
}
