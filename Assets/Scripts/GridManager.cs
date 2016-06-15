using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

    private static GridManager sInstance;
    public static GridManager Instance
    {
        get { return sInstance; }
    }

    public bool test = false;

    public int row = 4;
    public int column = 4;
    public float horizontalSpacingOffset = -2.25f;
    public float verticalSpacingOffset = -2.5f;
    public float cellSpacing = 0.5f;
    public EmptyTile standardTile;
    public GameObject SpriteTilesGameObject;


    //임시처리. 후에 xml데이터로 처리하시오
    //public SpriteRenderer[] numberSrcImgs;
    //public UIAtlas _numberSrcImgs;
    //public UISprite _imageNumber;

    private EmptyTile[,] allTiles;
    private List<EmptyTile> inactiveTiles;
    public List<EmptyTile> activeTiles;
    private NumberTile numberTiles;

    void Awake()
    {
        sInstance = this; //싱글톤으로 만들기 위함

        //GameObjectPoolManager.Instance.ReadyPool("Prefabs", "NumberInstance", this.row * this.column);
        //GameObjectPoolManager.Instance.ReadyPool("NumberPanel", this.row * this.column);
        GameObjectPoolManager.Instance.AddPool(SpriteTilesGameObject.name, this.SpriteTilesGameObject);
        this.allTiles = new EmptyTile[this.row, this.column];
        this.inactiveTiles = new List<EmptyTile>();
        this.activeTiles = new List<EmptyTile>();

        //_imageNumber.atlas = _numberSrcImgs;
        //_imageNumber.spriteName = "2048";
        //Debug.Log(_imageNumber.spriteName);
            }

    // Use this for initialization
    void Start() {

        ////모든 그리드 셀의 위치를 확보하기(좌측상단부터 우측하단방향으로)
        //for (int i = 0; i < this.row; i++)
        //{
        //    for (int j = 0; j < this.column; j++)
        //    {
        //        this.allTiles[i, j] = FindTile(j, i);
        //        if (this.allTiles[i, j] == null)
        //            continue;
        //        this.allTiles[i, j].Index_X = i;
        //        this.allTiles[i, j].Index_Y = j;
        //        this.inactiveTiles.Add(this.allTiles[i, j]);
        //    }
        //}
        //모든 그리드 셀의 위치를 확보하기(좌측상단부터 우측하단방향으로)
        this.allTiles[0, 0] = this.standardTile;
        this.inactiveTiles.Add(this.allTiles[0, 0]);
        BoxCollider2D col = this.standardTile.GetComponent<BoxCollider2D>();
        float colSizeOffset_x = col.bounds.size.x;
        float colSizeOffset_y = col.bounds.size.y;
        //print(string.Format("{0}max , {1}size , {2}extents , {3}center",
          //  col.bounds.max, col.bounds.size, col.bounds.extents, col.bounds.center));
        for (int i = 0; i < this.row; i++)
        {
            Ray2D ray;
            EmptyTile findTile;
            int temp_i = i + 1;
            Vector2 tempPos;
            
            for (int j = 1; j < this.column; j++)
            {
                tempPos = this.allTiles[i, j - 1].transform.position;
                tempPos.x += colSizeOffset_x;
                ray = new Ray2D(tempPos, Vector2.right);
                findTile = FindTile(ray);
                if (findTile == null)
                    continue;
                this.allTiles[i, j] = findTile;
                this.allTiles[i, j].Index_X = i;
                this.allTiles[i, j].Index_Y = j;
                this.inactiveTiles.Add(this.allTiles[i, j]);
            }
            tempPos = this.allTiles[i, 0].transform.position;
            tempPos.y -= colSizeOffset_y;
            ray = new Ray2D(tempPos, Vector2.down);
            findTile = FindTile(ray);
            if (findTile == null)
                continue;
            this.allTiles[temp_i, 0] = findTile;
            this.allTiles[temp_i, 0].Index_X = i;
            this.allTiles[temp_i, 0].Index_Y = 0;
            this.inactiveTiles.Add(this.allTiles[temp_i, 0]);
        }
    }

    // Update is called once per frame
    void Update() {
    }

    ////인덱스 값으로 월드 좌표 찾기
    //public Vector2 GridToWorldPoint(int x, int y)
    //{
    //    return new Vector2(x + this.horizontalSpacingOffset + this.cellSpacing * x,
    //        -y + this.verticalSpacingOffset - cellSpacing * y);
    //}

    ////레이캐스트로 그리드셀 찾기
    //public Tile FindTile(int index_x, int index_y)
    //{
    //    Ray2D ray = new Ray2D(this.GridToWorldPoint(index_x, index_y), Vector2.right);
    //    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, this.cellSpacing);
    //    if (hit.collider != null)
    //    {
    //        return hit.collider.gameObject.GetComponent<Tile>();
    //    }
    //    print("그리드셀을 찾을수가 없다");
    //    return null;
    //}

    //레이캐스트로 그리드셀 찾기
    public EmptyTile FindTile(Ray2D ray)
    {
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, this.cellSpacing);
        if (hit.collider != null)
        {
            //print(hit.collider.name);
            //print(string.Format("{0}pos , {1}pos", ray.origin, hit.transform.position));
            return hit.collider.gameObject.GetComponent<EmptyTile>();
        }
        print("그리드셀을 찾을수가 없다");
        return null;
    }

    //랜덤으로 빈 셀을 뽑아 새로운 숫자 투입
    public void AddNewNumberCell()
    {
        //일단 임시로 모든 셀이 활성화되어있다면 입력 받지 않기...추후에 변경 바람
        if (this.inactiveTiles.Count <= 0)
        {
            print("새로 추가하려했지만 모든 셀 활성화되었음");
            return;
        }

        //int rand_x = Random.Range(0, this.row);
        //int rand_y = Random.Range(0, this.column);
        int randNum = Random.Range(0, this.inactiveTiles.Count);
        if(this.inactiveTiles[randNum].AddNumberSprite(NumberLevel._2) == true)
            this.SetTileList(this.inactiveTiles[randNum], true);
        //this.inactiveTiles.RemoveAt(randNum);
    }

    //비활성화 셀목록 정리
    public void SetTileList(EmptyTile tile, bool isActive)
    {
        if (isActive == true)
        {
            if (this.inactiveTiles.Contains(tile) == true)
            {
                this.inactiveTiles.Remove(tile);
                this.activeTiles.Add(tile);
            }
        }
        else
        {
            if (this.activeTiles.Contains(tile) == true)
            {
                this.activeTiles.Remove(tile);
                this.inactiveTiles.Add(tile);
            }
        }//else
    }

    //모든 활성화된 숫자들 제거
    public void Reset()
    {
        foreach(EmptyTile tempTile in this.activeTiles)
        {
            tempTile.RemoveNumberSprite();
            this.inactiveTiles.Add(tempTile);
        }
        this.activeTiles.Clear();
    }

    //이동키에 따른 정보 전달
    public void ReadyMove(InputDirection inputDirection)
    {
        //일단 임시로 모든 셀이 활성화되어있다면 입력 받지 않기...추후에 변경 바람
        //if (this.inactiveTiles.Count <= 0)
        //{
        //    print("방향키 입력하였지만 모든 셀 활성화되었음");
        //    return;
        //}

        switch (inputDirection)
        {
            case InputDirection.Left:
                for (int i = 0; i < this.row; i++)
                {
                    //Tile[] tempTiles = new Tile[this.column];

                    for (int j = 0; j < this.column; j++)
                    {
                        //tempTiles[j] = this.allTiles[i, j];
                        EmptyTile tempTile = this.allTiles[i, j]; //한 행의 열을 계속 검사하는거다..

                        for (int k = 1; (j + k) < this.column; k++)
                        {
                            if (this.allTiles[i, j + k].LinkNumberSprite != null)
                            {
                                //업그레이드 파악이었다면 기준 타일을 다음 타일로 하기(이동이면 혹시 모르니 계속 검사)
                                if (this.UpgradeOrMove(tempTile, this.allTiles[i, j + k]) == true)
                                    break;
                            }
                        }//for k
                    }//for j
                }//for i
                break;
            case InputDirection.Right:
                for (int i = 0; i < this.row; i++)
                {
                    //Tile[] tempTiles = new Tile[this.column];

                    for (int j = this.column - 1; j >= 0; j--)
                    {
                        //tempTiles[j] = this.allTiles[i, j];
                        EmptyTile tempTile = this.allTiles[i, j]; //한 행의 열을 계속 검사하는거다..

                        for (int k = 1; (j - k) >= 0; k++)
                        {
                            if (this.allTiles[i, j - k].LinkNumberSprite != null)
                            {
                                //업그레이드 파악이었다면 기준 타일을 다음 타일로 하기(이동이면 혹시 모르니 계속 검사)
                                if (this.UpgradeOrMove(tempTile, this.allTiles[i, j - k]) == true)
                                    break;
                            }
                        }//for k
                    }//for j
                }//for i
                break;
            case InputDirection.Up:
                for (int i = 0; i < this.column; i++)
                {
                    //Tile[] tempTiles = new Tile[this.column];

                    for (int j = 0; j < row; j++)
                    {
                        //tempTiles[j] = this.allTiles[i, j];
                        EmptyTile tempTile = this.allTiles[j, i]; //한 열의 행을 계속 검사하는거다..

                        for (int k = 1; (j + k) < this.row; k++)
                        {
                            if (this.allTiles[j + k, i].LinkNumberSprite != null)
                            {
                                //업그레이드 파악이었다면 기준 타일을 다음 타일로 하기(이동이면 혹시 모르니 계속 검사)
                                if (this.UpgradeOrMove(tempTile, this.allTiles[j + k, i]) == true)
                                    break;
                            }
                        }//for k
                    }//for j
                }//for i
                break;
            case InputDirection.Down:
                for (int i = 0; i < this.column; i++)
                {
                    //Tile[] tempTiles = new Tile[this.column];

                    for (int j = this.row - 1; j >= 0; j--)
                    {
                        //tempTiles[j] = this.allTiles[i, j];
                        EmptyTile tempTile = this.allTiles[j, i]; //한 열의 행을 계속 검사하는거다..

                        for (int k = 1; (j - k) >= 0; k++)
                        {
                            if (this.allTiles[j - k, i].LinkNumberSprite != null)
                            {
                                //업그레이드 파악이었다면 기준 타일을 다음 타일로 하기(이동이면 혹시 모르니 계속 검사)
                                if (this.UpgradeOrMove(tempTile, this.allTiles[j - k, i]) == true)
                                    break;
                            }
                        }//for k
                    }//for j
                }//for i
                break;
        }
    }

    public bool IsGameOver()
    {
        //활성화 칸이 아직 다 차지 않았다면 그냥 검사 패스
        if (this.activeTiles.Count < this.allTiles.Length)
        {
            if(test == true)
            print(string.Format("activeTiles.Count:{0} , allTiles.Length:{1}", activeTiles.Count, allTiles.Length));
            return false;
        }

        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.column; j++)
            {
                //현재 검사 기준 타일셀 숫자와 우측 및 아래측 타일셀 숫자와 비교하여 등급 동일하면 패스
                //int currTileNumLev = this.allTiles[i, j].LinkNumberSprite.NumberLevel;
                NumberLevel currTileNumLev = this.allTiles[i, j].LinkNumberSprite.NumberLevel;

                if (j + 1 < this.column &&
                    currTileNumLev == this.allTiles[i, j + 1].LinkNumberSprite.NumberLevel)
                {
                    //print(string.Format("sour:{0} , dest:{1}", currTileNumLev, this.allTiles[i, j + 1].LinkNumberSprite.NumberLevel));
                    return false;
                }
                if (i + 1 < this.row &&
                    currTileNumLev == this.allTiles[i + 1, j].LinkNumberSprite.NumberLevel)
                {
                    //print(string.Format("sour:{0} , dest:{1}", currTileNumLev, this.allTiles[i + 1, j].LinkNumberSprite.NumberLevel));
                    return false;
                }
            }
        }
        //모두 검사해보았지만 도저히 합쳐질만한 요소 없음
        print("게임오버");
        return true;
    }

    //true이면 업그레이드 여부 확인한거고 false이면 그냥 이동 여부 확인한것
    private bool UpgradeOrMove(EmptyTile sourTile, EmptyTile destTile)
    {
        if (sourTile.LinkNumberSprite != null) //기준 타일에 숫자가 존재하면 업그레이드 파악
        {
            //int sourNumLev = sourTile.LinkNumberSprite.NumberLevel;
            NumberLevel sourNumLev = sourTile.LinkNumberSprite.NumberLevel;
            //등급 같아서 업그레이드 가능하다면
            //if (sourNumLev < this.numberSrcImgs.Length - 1 && //최대 등급이 아니고
            if (sourNumLev < NumberLevel._2048 && //최대 등급이 아니고
                sourNumLev == destTile.LinkNumberSprite.NumberLevel) //등급이 서로 같다면
            {
                //print(sourNumLev.ToString());
                //일단 임시로 움직임 없이 바로 변경되도록..추후에 변경바람
                sourTile.LinkNumberSprite.ChangeNumber(++sourNumLev);
                destTile.RemoveNumberSprite();
                this.SetTileList(destTile, false);

                //스코어매니저에 전달
                ScoreManager.Instance.AddScore(sourTile.LinkNumberSprite.NumberLevel);
            }

            //업그레이드 성립 여부 상관없이 더 이상의 검사 진행은 무의미
            return true;
        }
        else //기준 타일에 숫자가 없다면 이동 명령
        {
            //일단 임시로 움직임 없이 바로 변경되도록..추후에 변경바람
            sourTile.AddNumberSprite(destTile.LinkNumberSprite.NumberLevel);
            this.SetTileList(sourTile, true);
            destTile.RemoveNumberSprite();
            this.SetTileList(destTile, false);
            return false;
        }
    }
}
