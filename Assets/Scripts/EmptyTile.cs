using UnityEngine;
using System.Collections;

public class EmptyTile : MonoBehaviour {

    private int index_x;
    private int index_y;
    //private int numberLevel = 0;
    //private SpriteRenderer numberSprite;
    private Vector2 myPos; //내 위치를 편하게 전달하기 위함
    private NumberTile linkNumberTile; //연결된 숫자타일

    void Awake()
    {
        this.myPos = this.transform.position;
    }

    // Use this for initialization
    void Start() {

        /*Ray2D ray = new Ray2D(this.transform.position, Vector2.up);
        RaycastHit2D hit;

        if (this.UpTile == null)
        {
            hit = Physics2D.Raycast()
        }*/

    }

    public int Index_X
    {
        set { this.index_x = value; }
        get { return this.index_x; }
    }

    public int Index_Y
    {
        set { this.index_y = value; }
        get { return this.index_y; }
    }

    //public int NumberLevel
    //{
    //    set { this.numberLevel = value; }
    //    get { return this.numberLevel; }
    //}

    public Vector2 MyPos
    {
        get { return this.myPos; }
    }

    public NumberTile LinkNumberTile
    {
        get { return this.linkNumberTile; }
    }

    //레벨에 맞는 숫자타일을 추가해라
    //public bool AddNumberSprite(int numberLevel)
    public bool AddNumberSprite(NumberLevel numberLevel)
    {
        //this.numberLevel = numberLevel;
        //this.numberSprite =
        //    GameObjectPoolManager.Instance["NumberPanel"].ActiveGameObject(this.transform.position).GetComponent<SpriteRenderer>();
        //if (this.numberSprite == null)
        //{
        //    print(string.Format("Not Found NumberPanel GameObject"));
        //    return;
        //}

        //this.numberSprite.sprite = GridManager.Instance.numberSrcImgs[this.numberLevel].sprite;
        ////print(GridManager.Instance.numberSrcImgs.Length);

        //이미 연결된 숫자가 있다면 문제인거다..
        if (this.linkNumberTile != null)
        {
            print("연결된 숫자스프라이트가 이미 존재한다");
            return false;
        }
        //풀링객체를 가져온다.
        this.linkNumberTile =
            GameObjectPoolManager.Instance[GridManager.Instance.SpriteTilesGameObject.name].
            ActiveGameObject(this.myPos).GetComponent<NumberTile>();
        if (this.linkNumberTile == null)
            return false;

        this.linkNumberTile.CurrentTile = this;
        this.linkNumberTile.ChangeNumber(numberLevel, true); //스프라이트를 변경한다.
        return true;
    }

    //연결된 숫자타일을 비활성화 및 해제한다.
    public void DisconnectNumberTile(bool wantNumberTileInactive = false)
    {
        if(wantNumberTileInactive == true) //원한다면 연결된 숫자타일 비활성화하기
            this.linkNumberTile.gameObject.SetActive(false);
        this.linkNumberTile = null; //연결된 숫자타일 끊기
    }

    //연결된 숫자타일의 숫자이미지를 갱신해라
    public void UpdateSprite()
    {
        this.linkNumberTile.UpdateSprite();
    }

    //업그레이드 예약
    //public void ChangeNumberLevel(NumberLevel numberLevel)
    public void ReserveUpgrade(NumberLevel numberLevel, NumberTile targetNumberTile)
    {
        this.linkNumberTile.ChangeNumber(numberLevel, false); //일단 정보만 수정해놓기
        targetNumberTile.SetGoal(this, true); //합쳐질 숫자타일에 내 위치를 전달
    }

    //새로운 숫자타일과 연결한다
    //public void ChangeNumberTile()
    public void ConnectNumberTile(NumberTile targetNumberTile)
    {
        this.linkNumberTile = targetNumberTile; //숫자타일 갱신
        this.linkNumberTile.SetGoal(this, false); //숫자타일 갱신되었으므로 내 위치 알려주기
    }

    public void NowMove(InputDirection inputDir)
    {
        this.linkNumberTile.NowMove(inputDir);
    }
}
