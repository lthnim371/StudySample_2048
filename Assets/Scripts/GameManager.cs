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

public static class SimpleMath
{
    public static int NaturalNumberPow(int naturalNumber, int multiplier)
    {
        int result = 1;
        for (int i = 0; i < multiplier; i++)
            result *= naturalNumber;
        return result;
    }

    public static int AbsoluteValue(int integer)
    {
        if (integer < 0)
            return -integer;
        else
            return integer;
    }
    public static float AbsoluteValue(float realNumber)
    {
        if (realNumber < 0)
            return -realNumber;
        else
            return realNumber;
    }
}

[RequireComponent( typeof(InputTouch ))]
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
    //private InputDirection inputDirection = InputDirection.NONE;
    private InputTouch inputTouch;
    private bool paused = false;

    void Awake()
    {
        sInstance = this;
        this.inputTouch = this.GetComponent<InputTouch>();

        Screen.SetResolution(Screen.width, (int)((Screen.width * 0.5f) * 3), true);
    }

    // Update is called once per frame
    void Update() {

        //일단 뒤로가기키로 게임종료한다. 추후에 바로 종료 안되게 설정해도 된다.
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        else if (this.paused == true) //일단 홈키로 어플밖으로 나갔을때는 입력 안 받도록 하자. 추후 타임오버 등을 고려할 때 변경해야될듯..
            return;

        switch (this.state)
        {
            case State.Loaded:
                this.state = State.WaitingForInput;
                GridManager.Instance.AddNewNumberTile();
                GridManager.Instance.AddNewNumberTile();
                ScoreManager.Instance.AddScore(0);
                break;
            case State.WaitingForInput:
                /*if (Input.GetKeyDown(KeyCode.LeftArrow))
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
                */
                //터치입력의 결과값을 전달하여 게임진행 여부 결정
                this.state =
                    GridManager.Instance.ReadyMove(this.inputTouch.GetFlickDirection());
                break;
            case State.CheckingMatches:
                if (GridManager.Instance.TileMoving() == true) //타일들이 아직 움직이고 있다면 패스
                    return;
                GridManager.Instance.AddNewNumberTile(); //이동이 다 완료되었으므로 새로운 숫자타일 추가
                //숫자타일이 모두 활성화 된 상태에서 진행 여부 파악
                if (GridManager.Instance.IsGameOver() == true)
                    this.state = State.GameOver;
                else
                    this.state = State.WaitingForInput;
                break;
            case State.GameOver:
                ScoreManager.Instance.ActiveGameOverText();
                break;
        }//switch

	}//Update

    void OnApplicationPause(bool pause)
    {
        this.paused = pause;
    }
    
    //리셋버튼 클릭시 초기화
    public void ResetGame()
    {
        //숫자칸들 초기화하고 처음 2개 만들기로 가기
        GridManager.Instance.Reset();
        ScoreManager.Instance.InitialScore();
        this.state = State.Loaded;
        return;
    }
}
