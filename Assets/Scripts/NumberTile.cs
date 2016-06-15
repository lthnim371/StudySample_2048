using UnityEngine;
using System.Collections;
//using System;

public class NumberTile : MonoBehaviour {

    public float speed = 1f;

    //private SpriteRenderer mySpriteRenderer;
    private UISprite myUiSprite; //출력될 숫자이미지
    private EmptyTile currentTile; //나와 연결된 빈타일
    private EmptyTile goalTile; //합체 또는 이동할 빈타일
    //private int numberLevel = 0;
    private NumberLevel numberLevel = global::NumberLevel.NONE;
    //private Bounds myColliderBounds; //타일간 거리계산시 필요
    private bool isUpgrade; //업그레이드와 단순 이동 중 판단에 이용

    void Awake()
    {
        //this.mySpriteRenderer = this.GetComponent<SpriteRenderer>();
        this.myUiSprite = this.GetComponent<UISprite>();

        //this.myColliderBounds = this.GetComponent<BoxCollider2D>().bounds;
    }
    
    //비활성과 동시에 초기화
    void OnDisable()
    {
        //this.numberLevel = -1; //일단 첫번째 등급이 아니게 처리..
        this.numberLevel = global::NumberLevel.NONE;
        //this.mySpriteRenderer.sprite = null;
        this.myUiSprite.spriteName = "2";
        this.currentTile = this.goalTile = null;
    }

    //public int NumberLevel
    //{
    //    get { return this.numberLevel; }
    //}

    public NumberLevel NumberLevel
    {
        get { return this.numberLevel; }
    }

    public EmptyTile CurrentTile
    {
        set { this.currentTile = value; }
        get { return this.currentTile; }
    }

    //숫자등급 및 이미지 변경
    //public bool ChangeNumber(int numberLevel)
    public bool ChangeNumber(NumberLevel numberLevel, bool isNow)
    {
        //동일하다면 문제가 있다.
        //if (this.numberLevel == numberLevel)
        if (this.numberLevel == numberLevel)
        {
            print("등급이 중복된다.");
            return false;
        }
        this.numberLevel = numberLevel; //새로운 숫자로 갱신
        if (isNow == true) //바로 변경?
            //this.mySpriteRenderer.sprite = GridManager.Instance.numberSrcImgs[this.numberLevel].sprite;
            //this.myUiSprite.spriteName = numberLevel.ToString().Substring(1);
            this.UpdateSprite();

        return true;
    }

    //숫자 스프라이트 갱신
    public void UpdateSprite()
    {
        if(this.numberLevel <= NumberLevel.NONE)
        {
            print("없는 숫자이다.");
            return;
        }

        this.myUiSprite.spriteName = this.numberLevel.ToString().Substring(1);
    }

    //이동할 목적지 설정
    public void SetGoal(EmptyTile goalTile, bool isUpgrade)
    {
        this.goalTile = goalTile;
        this.isUpgrade = isUpgrade;
    }

    //이동을 시작해라
    public void NowMove(InputDirection inputDir)
    {
        if (this.goalTile == null) //제자리인 타일은 정지
            return;

        StartCoroutine("Coroutine_MoveTile", inputDir);
    }

    //숫자이미지 이동
    IEnumerator Coroutine_MoveTile(InputDirection inputDir)
    {
        float colSpacing;

        switch (inputDir)
        {
            //좌우 입력시
            case InputDirection.LEFT:
            case InputDirection.RIGHT:
                //업그레이드라면 목적지타일 외곽까지만 접근
                if (this.isUpgrade == true)
                    //colSpacing = this.myColliderBounds.size.x;
                    colSpacing = GridManager.Instance.StandardColliderBounds.size.x;
                else //단순 이동이면 중심지까지 접근
                    colSpacing = GridManager.Instance.StandardColliderBounds.extents.x;
                //목적지 좌표와 내 좌표와의 거리를 판단하여 반복 및 중단 결정
                while (Mathf.Abs(this.goalTile.MyPos.x - this.transform.position.x) > colSpacing)
                {
                    Vector2 currPos = this.transform.position;
                    //일정 속도로 목적지를 가기
                    float next_x =
                        Mathf.MoveTowards(currPos.x, this.goalTile.MyPos.x, this.speed * Time.deltaTime);
                    //print(next_x.ToString());
                    this.transform.position = new Vector2(next_x, currPos.y);
                    yield return null;
                }
                break;
            //상하 입력시
            case InputDirection.UP:
            case InputDirection.DOWN:         
                if (this.isUpgrade == true)
                    colSpacing = GridManager.Instance.StandardColliderBounds.size.y;
                else
                    colSpacing = GridManager.Instance.StandardColliderBounds.extents.y;
                while (Mathf.Abs(this.goalTile.MyPos.y - this.transform.position.y) > colSpacing)
                {
                    Vector2 currPos = this.transform.position;
                    float next_y =
                        Mathf.MoveTowards(currPos.y, this.goalTile.MyPos.y, this.speed * Time.deltaTime);
                    this.transform.position = new Vector2(currPos.x, next_y);
                    yield return null;
                }
                break;
        }
        //이동 완료 후
        if (this.isUpgrade == true) //업그레이드라면..
        {
            this.goalTile.UpdateSprite(); //목적지 숫자 갱신
            this.gameObject.SetActive(false); //비활성화하기
            GridManager.Instance.SetTileList(this.currentTile, false); //원래 연결했었던 타일 비활성화하기
            this.currentTile = null; //초기화
        }
        else //단순 이동이라면..
        {
            //print(string.Format("myName:{0} , goalName:{1}", this.name, goalTile.name));
            this.transform.position = goalTile.MyPos; //위치 맞추기
            this.currentTile = this.goalTile; //나와 연결된 타일 기억하기
            GridManager.Instance.SetTileList(this.currentTile, true); //연결 타일 활성화시키기
        }

        this.goalTile = null;
    }
}
