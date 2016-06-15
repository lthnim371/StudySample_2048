using UnityEngine;
using System.Collections;
//using System;

public class NumberTile : MonoBehaviour {

    public float speed = 5f;

    //private SpriteRenderer mySpriteRenderer;
    private UISprite myUiSprite; //출력될 숫자이미지
    private EmptyTile currentTile; //나와 연결된 빈타일
    private EmptyTile goalTile; //합체 또는 이동할 빈타일
    //private int numberLevel = 0;
    private NumberLevel numberLevel = global::NumberLevel.NONE;
    private bool moveTile = false; //이동 명령
    private Vector2 tileSpacing; //타일간 거리계산시 필요

    void Awake()
    {
        //this.mySpriteRenderer = this.GetComponent<SpriteRenderer>();
        this.myUiSprite = this.GetComponent<UISprite>();

        BoxCollider2D col = this.GetComponent<BoxCollider2D>();
        this.tileSpacing = col.size;
    }
    
    //비활성과 동시에 초기화
    void OnDisable()
    {
        //this.numberLevel = -1; //일단 첫번째 등급이 아니게 처리..
        this.numberLevel = global::NumberLevel.NONE;
        //this.mySpriteRenderer.sprite = null;
        this.myUiSprite.spriteName = "2";
        this.currentTile = null;
    }

    //public int NumberLevel
    //{
    //    get { return this.numberLevel; }
    //}

    public NumberLevel NumberLevel
    {
        get { return this.numberLevel; }
    }

    public bool MoveTile
    {
        set { this.moveTile = value; }
        get { return this.moveTile; }
    }

    public EmptyTile CurrentTile
    {
        set { this.currentTile = value; }
        get { return this.currentTile; }
    }

    public EmptyTile GoalTile
    {
        set { this.goalTile = value; }
        get { return this.goalTile; }
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

    //이동을 시작해라
    public void NowMove()
    {        
        StartCoroutine("Coroutine_MoveTile");
    }

    //숫자이미지 이동
    IEnumerator Coroutine_MoveTile()
    {
        switch (GridManager.Instance.InputDirection)
        {
            //좌우 입력시
            case InputDirection.Left:
            case InputDirection.Right:
                //
                if(this.numberLevel == NumberLevel.NONE)
                    
                while (Mathf.Abs(this.goalTile.MyPos.x - this.transform.position.x) > this.tileSpacing.x)
                {
                    float next_x = Mathf.MoveTowards(this.transform.position.x, this.goalTile.MyPos.x, this.speed * Time.deltaTime);
                    this.transform.Translate(next_x, 0f, 0f);
                    yield return null;
                }
                break;
            //상하 입력시
            case InputDirection.Up:
            case InputDirection.Down:
                while (Mathf.Abs(this.goalTile.MyPos.y - this.transform.position.y) > this.tileSpacing.y)
                {
                    float next_y = Mathf.MoveTowards(this.transform.position.y, this.goalTile.MyPos.y, this.speed * Time.deltaTime);
                    this.transform.Translate(next_y, 0f, 0f);
                    yield return null;
                }
                break;
        }
        this.moveTile = false;
        this.goalTile.UpdateSprite();
        this.goalTile = null;
    }
}
