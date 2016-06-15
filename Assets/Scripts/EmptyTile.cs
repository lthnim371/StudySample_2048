using UnityEngine;
using System.Collections;

public class EmptyTile : MonoBehaviour {
    
    private int index_x;
    private int index_y;
    //private int numberLevel = 0;
    //private SpriteRenderer numberSprite;
    private Vector2 myPos;
    public NumberTile linkNumberSprite;

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

    // Update is called once per frame
    void Update() {

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

    public NumberTile LinkNumberSprite
    {
        get { return this.linkNumberSprite; }
    }

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
        if (this.linkNumberSprite != null)
        {
            print("연결된 숫자스프라이트가 이미 존재한다");
            return false;
        }

        this.linkNumberSprite =
            GameObjectPoolManager.Instance[GridManager.Instance.SpriteTilesGameObject.name].
            ActiveGameObject(this.myPos).GetComponent<NumberTile>();
        if (this.linkNumberSprite == null)
            return false;

        this.linkNumberSprite.ChangeNumber(numberLevel);
        return true;
    }

    public void RemoveNumberSprite()
    {
        this.linkNumberSprite.gameObject.SetActive(false);
        this.linkNumberSprite = null;
    }
}
