using UnityEngine;
using System.Collections;


public enum State
{
    Loaded, WaitingForInput, CheckingMatches, GameOver, Perfect
}

public enum InputDirection
{
    NONE, LEFT, RIGHT, UP, DOWN, RESET
}

public enum NumberLevel
{
    NONE, _2, _4, _8, _16, _32, _64, _128, _256, _512, _1024, _2048
}

[System.Serializable]
public class cSaveInfo
{
    //public sSaveInfo[] saveInfoArray;

    //public cSaveInfo(sSaveInfo[] saveInfoArray)
    //{
    //    this.saveInfoArray = saveInfoArray;
    //}

    public int[] tileIndex_x;
    public int[] tileIndex_y;
    public NumberLevel[] numberLevel;

    public cSaveInfo(int arrayLength)
    {
        this.tileIndex_x = new int[arrayLength];
        this.tileIndex_y = new int[arrayLength];
        this.numberLevel = new NumberLevel[arrayLength];
    }

    public cSaveInfo(int[] index_Xs, int[] index_Ys, NumberLevel[] numberLevels)
    {
        this.tileIndex_x = index_Xs;
        this.tileIndex_y = index_Ys;
        this.numberLevel = numberLevels;
    }
}

public static class cSimpleMath
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

    private bool cheatKey = false; //오로지 치트용

    void Awake()
    {
        sInstance = this; //싱글톤화

        this.inputTouch = this.GetComponent<InputTouch>(); //입력만 받는 매니저스크립트 가져오기

        //일단 임시로 해상도 2:3으로 강제 맞추기
        Screen.SetResolution(Screen.width, (int)((Screen.width * 0.5f) * 3), true);
    }

    void Start() //게임시작 전
    {
        ScoreManager.Instance.LoadScoreInfo(); //저장된 최종점수 가져오기
        if (GridManager.Instance.LoadLastInfo()) //저장된 마지막 위치정보 가져오기
            this.state = State.WaitingForInput; //정상적으로 가져왔다면 바로 입력대기로 이동
    }

    // Update is called once per frame
    void Update() {

        //일단 뒤로가기키로 게임종료한다. 추후에 바로 종료 안되게 설정해도 된다.
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        else if (Input.GetKeyDown(KeyCode.Menu)) //일단 임시 치트키(게임올클리어용)
        {
            if(this.cheatKey == true) //이미 사용상태라면 취소
        }
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
                this.state = GridManager.Instance.CheckGameEnd();
                break;
            case State.GameOver:
                ScoreManager.Instance.ActiveGameEndText(true);
                break;
            case State.Perfect:
                ScoreManager.Instance.ActiveGameEndText(false);
                break;
        }//switch

	}//Update

    //홈키로 어플 바깥으로 나간 경우
    void OnApplicationPause(bool pause)
    {
        this.paused = pause; //동작되지 않게 하자
    }

    //어플이 종료될 때
    void OnApplicationQuit()
    {
        //한창 게임 중이라면
        if (this.state == State.WaitingForInput || this.state == State.CheckingMatches)
        {
            GridManager.Instance.SaveCurrentInfo(); //마지막 숫자 위치와 숫자 레벨 기억하자
            ScoreManager.Instance.SaveScoreInfo(true); //최종점수 및 진행점수 저장하자
        }
        else //그 외의 최초 상태, 게임오버
        {
            PlayerPrefs.DeleteAll(); //다 비운다.
            ScoreManager.Instance.SaveScoreInfo(false); //무조건 최종점수만 저장하자
        }
    }
    
    //리셋버튼 클릭시 초기화
    public void ResetGame()
    {
        //숫자칸들 초기화하고 처음 2개 만들기로 가기
        GridManager.Instance.Reset();
        ScoreManager.Instance.InitialScore();
        this.state = State.Loaded;

        if (PlayerPrefs.HasKey("SaveInfos")) //만약 저장한 위치 정보 있으면
            PlayerPrefs.DeleteKey("SaveInfos"); //없애자
        if (PlayerPrefs.HasKey("CurrentScore")) //만약 저장한 진행점수 있으면
            PlayerPrefs.DeleteKey("CurrentScore"); //없애자

        //일단 임시 치트키 (치트도 초기화)
        this.cheatKey = false;

        return;
    }
}
