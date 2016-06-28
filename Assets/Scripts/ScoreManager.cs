using UnityEngine;
using System.Collections;

public class ScoreManager : Singleton<ScoreManager> {

    //private static ScoreManager sInstance;
    //public static ScoreManager Instance
    //{
    //    get
    //    {
    //        if (sInstance == null)
    //        {
    //            GameObject newGameObj = new GameObject("_ScoreManager");
    //            sInstance = newGameObj.AddComponent<ScoreManager>();
    //        }

    //        return sInstance;
    //    }
    //}

    private readonly string strZero = "0";
    private readonly string strGameOver = "GameOver";
    private readonly string strPerfect = "Perfect";

    private UILabel bestScoreLabel; //최고점수 라벨
    private UILabel currentScoreLabel; //진행점수 라벨
    private UILabel gameEndLabel; //출력될 게임앤드 라벨
    //private string scoreText = "Score";
    private int currentScore = 0; //보관될 진행점수
    private int bestScore = 0; //보관될 최고점수

    protected override void Awake()
    {
        //sInstance = this; //싱글톤화
        this.gameObject.name = "_ScoreManager";

        GameObject findGameObj = GameObject.FindGameObjectWithTag("UI");
        if (findGameObj == null) {
            print("UI태그의 게임오브젝트를 못 찾았다."); return; }

        Transform findTM = findGameObj.transform.FindChild("Camera/Anchor/TextPanel");
        if (findTM == null) {
            print("TextPanel를 못 찾았다."); return; }

        this.bestScoreLabel = findTM.FindChild("BestScore/ScoreLabel").GetComponent<UILabel>();
        this.currentScoreLabel = findTM.FindChild("CurrentScore/ScoreLabel").GetComponent<UILabel>();
        this.gameEndLabel = findTM.FindChild("GameEndLabel").GetComponent<UILabel>();
        
#if SHOW_DEBUG_MESSAGES
        if (this.bestScoreLabel == null || this.currentScoreLabel == null || this.gameEndLabel == null)
            print("라벨을 물려놓지 않았다.");
#endif
        this.gameEndLabel.gameObject.SetActive(false); //게임앤드라벨 숨기기
    }

	// Use this for initialization
	void Start () {

        //시작 시 최초 점수로 셋팅
        //this.scoreLabel.text =
        //  string.Format("{0} : {1}", this.scoreText, this.currentScore);
        this.currentScoreLabel.text = this.strZero;
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
            //int newScore = (int)Mathf.Pow(2, (float)numberLevel);
            int newScore = cSimpleMath.NaturalNumberPow(2, (int)numberLevel);

            this.currentScore += newScore;
        }

        //this.scoreLabel.text =
        //  string.Format("{0} : {1}", this.scoreText, this.currentScore);
        this.currentScoreLabel.text = this.currentScore.ToString();
        this.ChangeScore(); //현재 점수가 최종점수보다 높다면 바로 갱신
    }

    //현재 점수 초기화.
    public void InitialScore()
    {
        this.ChangeScore(); //초기화하기전에 혹시모르니 최종점수 갱신해보자

        this.currentScore = 0;
        this.currentScoreLabel.text = this.strZero;
        //(아마 게임오버시 호출될거 같으니 게임오버라벨 비활성해놓자)
        if (this.gameEndLabel.gameObject.activeSelf == true)
            this.gameEndLabel.gameObject.SetActive(false);
    }

    //게임앤드라벨 활성화하기
    public void ActiveGameEndText(bool isGameOver)
    {
        //게임클리어 또는 게임오버 분기에 따라 출력텍스트 변경
        if (isGameOver)
            this.gameEndLabel.text = this.strGameOver;
        else
            this.gameEndLabel.text = this.strPerfect;

        this.gameEndLabel.gameObject.SetActive(true); //라벨 활성화하기

        this.ChangeScore(); //최종점수 변경
    }

    //외부에서 저장요청
    public void SaveScoreInfo(bool wantCurrentScoreSave)
    {
        //혹시 모르니 최고점수갱신해보기
        this.ChangeScore();
        //최고점수 저장하기
        PlayerPrefs.SetInt("BestScore", this.bestScore);

        if (wantCurrentScoreSave) //필요하면 현재 진행 점수 저장
            PlayerPrefs.SetInt("CurrentScore", this.currentScore);
        PlayerPrefs.Save();
    }

    //외부에서 불러오기 요청
    public void LoadScoreInfo()
    {
        if (PlayerPrefs.HasKey("BestScore")) //불러올 최고점수 저장데이터가 있다면..
        {
            this.bestScore = PlayerPrefs.GetInt("BestScore"); //최고점수 가져오기
            this.bestScoreLabel.text = this.bestScore.ToString(); //갱신
        }

        if(PlayerPrefs.HasKey("CurrentScore")) //불러올 진행점수가 있다면
        {
            this.currentScore = PlayerPrefs.GetInt("CurrentScore");
            this.currentScoreLabel.text = this.currentScore.ToString();
        }

#if SHOW_DEBUG_MESSAGES
        else
            print("Not Found PlayerPrefs Key");
#endif
    }

    //최종 점수 변경하기
    public void ChangeScore()
    {
        //현재 점수가 최종점수보다 크다면 변경
        if (this.currentScore > this.bestScore)
        {
            this.bestScore = this.currentScore;
            this.bestScoreLabel.text = this.currentScore.ToString();

            //리더보드로 점수 전달
            GPGSManager.Instance.PostingLeaderboard(this.bestScore);
        }
    }
}
