using UnityEngine;
using System.Collections;


public enum State
{
    Loaded, WaitingForInput, CheckingMatches, GameOver
}

public enum InputDirection
{
    NONE, Left, Right, Up, Down
}

public enum NumberLevel
{
    NONE, _2, _4, _8, _16, _32, _64, _128, _256, _512, _1024, _2048
}

public class GameManager : MonoBehaviour {

    private static GameManager sInstance;
    public static GameManager Instance
    {
        get
        {
            if (sInstance == null)
            {
                GameObject newGameObj = new GameObject("_Gamemanager");
                sInstance = newGameObj.AddComponent<GameManager>();
            }

            return sInstance;
        }
    }

    private State state = State.Loaded;

    void Awake()
    {
        sInstance = this;
    }

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update() {
        
        switch (this.state)
        {
            case State.Loaded:
                this.state = State.WaitingForInput;
                GridManager.Instance.AddNewNumberTile();
                GridManager.Instance.AddNewNumberTile();
                ScoreManager.Instance.AddScore(0);
                break;
            case State.WaitingForInput:
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    GridManager.Instance.ReadyMove(InputDirection.Left);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    GridManager.Instance.ReadyMove(InputDirection.Right);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    GridManager.Instance.ReadyMove(InputDirection.Up);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    GridManager.Instance.ReadyMove(InputDirection.Down);
                }
                else if (Input.GetKeyDown(KeyCode.R)) //리셋버튼
                {
                    //숫자칸들 초기화하고 처음 2개 만들기로 가기
                    GridManager.Instance.Reset();
                    this.state = State.Loaded;
                    return;
                }
                else //아무것도 안 눌렀으면 프레임 패스
                    return;

                //무언가 눌렀다면 스테이트 변경
                this.state = State.CheckingMatches;                

                break;
            case State.CheckingMatches:
                GridManager.Instance.AddNewNumberCell();
                if(GridManager.Instance.IsGameOver() == true)
                    this.state = State.GameOver;
                else
                    this.state = State.WaitingForInput;
                break;
            case State.GameOver:
                ScoreManager.Instance.ActiveGameOverText();
                break;
        }

	}
}
