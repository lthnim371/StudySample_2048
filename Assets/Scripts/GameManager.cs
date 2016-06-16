using UnityEngine;
using System.Collections;


public enum State
{
    Loaded, WaitingForInput, CheckingMatches, GameOver
}

public enum InputDirection
{
    NONE, LEFT, RIGHT, UP, DOWN, RESET
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
    private InputDirection inputDirection = InputDirection.NONE;

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
                    this.inputDirection = InputDirection.LEFT;
                    GridManager.Instance.ReadyMove(this.inputDirection);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    this.inputDirection = InputDirection.RIGHT;
                    GridManager.Instance.ReadyMove(this.inputDirection);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    this.inputDirection = InputDirection.UP;
                    GridManager.Instance.ReadyMove(this.inputDirection);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    this.inputDirection = InputDirection.DOWN;
                    GridManager.Instance.ReadyMove(this.inputDirection);
                }
                else if (Input.GetKeyDown(KeyCode.R)) //리셋버튼
                {
                    this.inputDirection = InputDirection.RESET;
                    //숫자칸들 초기화하고 처음 2개 만들기로 가기
                    GridManager.Instance.Reset();
                    this.state = State.Loaded;
                    return;
                }
                else //아무것도 안 눌렀으면 프레임 패스
                {
                    this.inputDirection = InputDirection.NONE;
                    return;
                }

                //무언가 눌렀다면 스테이트 변경
                this.state = State.CheckingMatches;
                //이동하자
                GridManager.Instance.MoveTile(this.inputDirection);
                break;
            case State.CheckingMatches:
                if (GridManager.Instance.TileMoving() == true)
                    return;
                GridManager.Instance.AddNewNumberTile();
                if (GridManager.Instance.IsGameOver() == true)
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
