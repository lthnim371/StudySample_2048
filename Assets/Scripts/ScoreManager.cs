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

    private UILabel scoreLabel;
    private UILabel GameOverLabel;
    private string scoreText = "Score";
    private int currentScore = 0;

    void Awake()
    {
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
    }

	// Use this for initialization
	void Start () {

        this.scoreLabel.text =
            string.Format("{0} : {1}", this.scoreText, this.currentScore);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //새로운 점수 추가하기
    //public void AddScore(int numberLevel)
    public void AddScore(NumberLevel numberLevel)
    {
        //if (numberLevel <= 0) //0 이하로 전달이 오면 그냥 점수 초기화..
        if (numberLevel <= NumberLevel.NONE) //0 이하로 전달이 오면 그냥 점수 초기화..
            this.currentScore = 0;
        else
        {
            //그냥 2의 제곱값으로 점수 추가하기...
            int newScore = (int)Mathf.Pow(2, (float)numberLevel + 1);
            this.currentScore += newScore;
        }

        this.scoreLabel.text =
            string.Format("{0} : {1}", this.scoreText, this.currentScore);
    }

    public void ActiveGameOverText()
    {
        this.GameOverLabel.gameObject.SetActive(true);
    }
}
