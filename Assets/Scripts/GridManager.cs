using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : Singleton<GridManager> {

    public int row = 4; //행 갯수
    public int column = 4; //열 갯수
    //public float horizontalSpacingOffset = -2.25f;
    //public float verticalSpacingOffset = -2.5f;
    //public float cellSpacing = 0.5f;
    public BackTile standardTile; //[0,0]가 되는 기준 타일 가져오기
    public GameObject SpriteTilesGameObject; //미리 만들어진 풀링숫자타일객체의 부모게임오브젝트 가져오기
    public float tileMoveSpeed = 1f;

    //임시처리. 후에 xml데이터로 처리하시오
    //public SpriteRenderer[] numberSrcImgs;
    //public UIAtlas _numberSrcImgs;
    //public UISprite _imageNumber;

    private BackTile[,] allTiles; //모든 백타일들 보관
    private List<BackTile> emptyTiles; //백타일 중 빈타일만 보관
    private List<NumberTile> numberTiles; //활성화된 숫자타일만 보관
    private Bounds standardColliderBounds; //거리계산(충돌) 시 필요한 오프셋 파악을 위함

    protected override void Awake()
    {
        instance = this; //싱글톤으로 만들기 위함

        //숫자타일객체들 풀링처리
        //GameObjectPoolManager.Instance.ReadyPool("Prefabs", "NumberInstance", this.row * this.column);
        //GameObjectPoolManager.Instance.ReadyPool("NumberPanel", this.row * this.column);
        GameObjectPoolManager.Instance.AddPool(SpriteTilesGameObject.name, this.SpriteTilesGameObject);
        this.allTiles = new BackTile[this.row, this.column];
        this.emptyTiles = new List<BackTile>();
        this.numberTiles = new List<NumberTile>();

        //_imageNumber.atlas = _numberSrcImgs;
        //_imageNumber.spriteName = "2048";
        //Print(_imageNumber.spriteName);

        BoxCollider2D boxCol = this.standardTile.GetComponent<BoxCollider2D>();
        this.standardColliderBounds = boxCol.bounds;

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
        //타일보관은 맨좌측상단(0,0)을 기준으로 1행 4열로 보관하고 있다.
        //ex) [0,0] [0,1] [0,2]...우측방향의 타일들 //[0,0] [1,0] [2,0]...아래측방향의 타일들
        this.allTiles[0, 0] = this.standardTile;
        this.emptyTiles.Add(this.allTiles[0, 0]);
        this.allTiles[0, 0].Index_X = 0;
        this.allTiles[0, 0].Index_Y = 0;
        //print(string.Format("{0}max , {1}size , {2}extents , {3}center",
        //  col.bounds.max, col.bounds.size, col.bounds.extents, col.bounds.center));
        for (int i = 0; i < this.row; i++)
        {
            Ray2D ray;
            BackTile findTile;
            int temp_i = i + 1; //다음 행 검사를 위함
            Vector2 tempPos; //기준타일의 위치로 활용

            for (int j = 1; j < this.column; j++) //다음 열부터 시작
            {
                //이전 열 위치에서 오프셋만큼 이동하여 레이캐스트 검사
                tempPos = this.allTiles[i, j - 1].transform.position; //이전 열 위치에서
                tempPos.x += this.standardColliderBounds.size.x; //오프셋만큼 이동하여(우측 방향으로 인덱스 증가)
                ray = new Ray2D(tempPos, Vector2.right); //다음 열 방향으로 레이캐스트
                findTile = FindTile(ray);
                if (findTile == null) //기준타일이 맨 오른쪽 마지막이라면 현재 for문은 종료될 것이다.
                    continue;
                this.allTiles[i, j] = findTile; //레이 체크한 타일을 보관
                this.allTiles[i, j].Index_X = i;
                this.allTiles[i, j].Index_Y = j;
                this.emptyTiles.Add(this.allTiles[i, j]); //현재로선 빈 타일이므로 모두 기억해둔다.
            }
            tempPos = this.allTiles[i, 0].transform.position; //첫 번째 열 위치로 돌아와서
            tempPos.y -= this.standardColliderBounds.size.y; //다음 행을 향해(아래측방향으로 인덱스 증가)
            ray = new Ray2D(tempPos, Vector2.down); //아래로 레이캐스트
            findTile = FindTile(ray);
            if (findTile == null) //현재가 맨 아래 타일이라면
                continue;
            this.allTiles[temp_i, 0] = findTile; //다음 행을 가리키는 인덱스에 보관
            this.allTiles[temp_i, 0].Index_X = temp_i;
            this.allTiles[temp_i, 0].Index_Y = 0;
            this.emptyTiles.Add(this.allTiles[temp_i, 0]);
        }

        this.tileMoveSpeed =
            XmlManager.Instance.FindGameSystemSettingValue("타일이동속도", this.tileMoveSpeed);        
    }

    public Bounds StandardColliderBounds
    {
        get { return this.standardColliderBounds; }
    }

    /*//인덱스 값으로 월드 좌표 찾기
    public Vector2 GridToWorldPoint(int x, int y)
    {
        return new Vector2(x + this.horizontalSpacingOffset + this.cellSpacing * x,
            -y + this.verticalSpacingOffset - cellSpacing * y);
    }

    //레이캐스트로 그리드셀 찾기
    public Tile FindTile(int index_x, int index_y)
    {
        Ray2D ray = new Ray2D(this.GridToWorldPoint(index_x, index_y), Vector2.right);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, this.cellSpacing);
        if (hit.collider != null)
        {
            return hit.collider.gameObject.GetComponent<Tile>();
        }
        print("그리드셀을 찾을수가 없다");
        return null;
    }*/

    //레이캐스트로 그리드타일 찾기
    public BackTile FindTile(Ray2D ray) 
    {
        //그냥 대강 지름만큼으로 레이캐스트 길이 결정
        float tileSpacing =
            (this.standardColliderBounds.size.x + this.standardColliderBounds.size.y) * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, tileSpacing);
        if (hit.collider != null)
        {
            //print(string.Format("{0}pos , {1}pos", ray.origin, hit.transform.position));
            return hit.collider.gameObject.GetComponent<BackTile>();
        }
#if SHOW_DEBUG_MESSAGES
        print("그리드셀을 찾을수가 없다");
#endif
        return null;
    }

    //랜덤으로 빈 셀을 뽑아 새로운 숫자 투입
    public void AddNewNumberTile(NumberLevel numberLevel = NumberLevel._2)
    {
        //일단 임시로 모든 셀이 활성화되어있다면 입력 받지 않기...추후에 변경 바람
        //if (this.inactiveTiles.Count <= 0)
        if(this.numberTiles.Count >= this.allTiles.Length)
        {
#if SHOW_DEBUG_MESSAGES
            print("새로 추가하려했지만 모든 셀 활성화되었음");
#endif
            return;
        }

        //int rand_x = Random.Range(0, this.row);
        //int rand_y = Random.Range(0, this.column);
        int randNum = Random.Range(0, this.emptyTiles.Count); //현재 빈타일 갯수 내에서 랜덤 숫자 뽑기
        //해당 랜덤인덱스로 기본숫자타일을 정상적으로 추가하였다면
        if (this.emptyTiles[randNum].AddNumberSprite(numberLevel) == true)
        {
            this.SetNumberTiles(this.emptyTiles[randNum].LinkNumberTile, true); //활성화숫자타일목록에 추가
            this.SetEmptyTiles(this.emptyTiles[randNum], false); //빈타일리스트에서는 제거
        }
        //this.inactiveTiles.RemoveAt(randNum);
    }

    //빈타일리스트 정리
    public void SetEmptyTiles(BackTile backTile, bool addOrRemove)
    {
        if (addOrRemove == true) //목록에 추가라면..
        {
            //빈타일목록에 없다면..
            if (this.emptyTiles.Contains(backTile) == false)
                this.emptyTiles.Add(backTile); //빈타일목록에 추가               
#if SHOW_DEBUG_MESSAGES
            else //문제가 있는거다..
                print("추가 대상이 목록에 이미 있다.");
#endif
        }
        else //위와 반대
        {
            //빈타일목록에 있다면
            if (this.emptyTiles.Contains(backTile) == true)
                this.emptyTiles.Remove(backTile);
#if SHOW_DEBUG_MESSAGES
            else
                print("제거 대상이 목록에 없다.");
#endif
        }
    }

    //숫자타일리스트 정리
    public void SetNumberTiles(NumberTile numberTile, bool addOrRemove)
    {
        if (addOrRemove == true) //목록에 추가라면..
        {
            //빈타일목록에 없다면..
            if (this.numberTiles.Contains(numberTile) == false)
                this.numberTiles.Add(numberTile); //빈타일목록에 추가               
#if SHOW_DEBUG_MESSAGES
            else //문제가 있는거다..-> 딱히 문제될게 없는것같음
                print("추가 대상이 목록에 이미 있다.");
#endif
        }
        else //위와 반대
        {
            //빈타일목록에 있다면
            if (this.numberTiles.Contains(numberTile) == true)
                this.numberTiles.Remove(numberTile);
#if SHOW_DEBUG_MESSAGES
            else
                print("제거 대상이 목록에 없다.");
#endif
        }
    }
    
    //모든 활성화된 숫자들 제거
    public void Reset()
    {
        //활성화목록을 돌면서 
        //foreach (NumberTile tempTile in this.numberTiles)
        //{
        //    this.SetEmptyTiles(tempTile.CurrentTile, true); //연결되어 있는 백타일을 비활성화 목록에 추가
        //    tempTile.CurrentTile.DisconnectNumberTile(); //백타일에 연결된 숫자타일들 연결끊기
        //    tempTile.gameObject.SetActive(false); //숫자타일객체를 비활성화
        //}
        for (int i = 0; i < this.numberTiles.Count; i++)
        {
            this.SetEmptyTiles(this.numberTiles[i].CurrentTile, true);
            this.numberTiles[i].CurrentTile.DisconnectNumberTile();
            this.numberTiles[i].gameObject.SetActive(false);
        }
        this.numberTiles.Clear(); //모든 작업이 완료되면 활성화목록 모두 비우기
    }

    //이동키에 따른 정보 전달
    public State ReadyMove(InputDirection inputDirection)
    {
        if (inputDirection == InputDirection.NONE) //입력키가 없으면
            return State.WaitingForInput; //다시 대기모드로 돌아간다

        bool unableMove = true;
        //일단 임시로 모든 셀이 활성화되어있다면 입력 받지 않기...추후에 변경 바람
        //if (this.inactiveTiles.Count <= 0)
        //{
        //    print("방향키 입력하였지만 모든 셀 활성화되었음");
        //    return;
        //}
        switch (inputDirection)
        {
            case InputDirection.LEFT:
                for (int i = 0; i < this.row; i++) //행부터
                {
                    //Tile[] tempTiles = new Tile[this.column];

                    for (int j = 0; j < this.column; j++) //좌측 열부터
                    {
                        //tempTiles[j] = this.allTiles[i, j];
                        BackTile tempTile = this.allTiles[i, j]; //비교할 기준타일 결정

                        for (int k = 1; (j + k) < this.column; k++) //현재 기준 열과 다음 열의 타일 비교를 위함
                        {
                            //if (this.allTiles[i, j + k].LinkNumberTile != null) //다음 열에 숫자타일이 존재한다면..
                            {
                                //업그레이드 파악이었다면 기준 타일을 다음 타일로 하기(이동이면 그 다음 열로 변경하여 계속 검사)
                                if (this.UpgradeOrMove(tempTile, this.allTiles[i, j + k], ref unableMove) == true)
                                    break;
                            }
                        }//for k
                    }//for j
                }//for i
                break;
            case InputDirection.RIGHT:
                for (int i = 0; i < this.row; i++)
                {
                    //Tile[] tempTiles = new Tile[this.column];

                    for (int j = this.column - 1; j >= 0; j--)
                    {
                        //tempTiles[j] = this.allTiles[i, j];
                        BackTile tempTile = this.allTiles[i, j];

                        for (int k = 1; (j - k) >= 0; k++)
                        {
                            //if (this.allTiles[i, j - k].LinkNumberTile != null)
                            {
                                if (this.UpgradeOrMove(tempTile, this.allTiles[i, j - k], ref unableMove) == true)
                                    break;
                            }
                        }//for k
                    }//for j
                }//for i
                break;
            case InputDirection.UP:
                for (int i = 0; i < this.column; i++)
                {
                    //Tile[] tempTiles = new Tile[this.column];

                    for (int j = 0; j < row; j++)
                    {
                        //tempTiles[j] = this.allTiles[i, j];
                        BackTile tempTile = this.allTiles[j, i];

                        for (int k = 1; (j + k) < this.row; k++)
                        {
                            //if (this.allTiles[j + k, i].LinkNumberTile != null)
                            {
                                if (this.UpgradeOrMove(tempTile, this.allTiles[j + k, i], ref unableMove) == true)
                                    break;
                            }
                        }//for k
                    }//for j
                }//for i
                break;
            case InputDirection.DOWN:
                for (int i = 0; i < this.column; i++)
                {
                    //Tile[] tempTiles = new Tile[this.column];

                    for (int j = this.row - 1; j >= 0; j--)
                    {
                        //tempTiles[j] = this.allTiles[i, j];
                        BackTile tempTile = this.allTiles[j, i];

                        for (int k = 1; (j - k) >= 0; k++)
                        {
                            //if (this.allTiles[j - k, i].LinkNumberTile != null)
                            {
                                if (this.UpgradeOrMove(tempTile, this.allTiles[j - k, i], ref unableMove) == true)
                                    break;
                            }
                        }//for k
                    }//for j
                }//for i
                break;
        }//switch

        if (unableMove == true) //다 확인해보았지만 하나라도 움직일 수 없다면
            return State.WaitingForInput; //다시 입력 상태로 돌아간다.

        //여기까지 오면 무사히 이동준비를 마친거다.
        this.MoveTile(inputDirection); //입력키가 있으므로 타일을 이동시키자
        return State.CheckingMatches; //게임결과 체크로 넘어가자
    }

    public State CheckGameEnd()
    {
        //활성화 칸이 아직 다 차지 않았다면 그냥 검사 패스
        if (this.numberTiles.Count < this.allTiles.Length)
        {
            //if(test == true)
            //print(string.Format("activeTiles.Count:{0} , allTiles.Length:{1}", activeTiles.Count, allTiles.Length));
            return State.WaitingForInput;
        }

        //bool allMaxNumber = false; //올클리어인지 확인을 위한 용도

        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.column; j++)
            {
                //현재 검사 기준 타일셀 숫자와 우측 및 아래측 타일셀 숫자와 비교하여 등급 동일하면 패스
                //int currTileNumLev = this.allTiles[i, j].LinkNumberSprite.NumberLevel;
                NumberLevel currTileNumLev = this.allTiles[i, j].LinkNumberTile.NumberLevel;
                if (currTileNumLev == NumberLevel._2048) //최대등급이라면 검사할 필요없음
                    continue;

                //allMaxNumber = false; //여기로 오면 올클리어는 실패임

                //다음 열과 동급인지 확인
                if (j + 1 < this.column &&
                    currTileNumLev == this.allTiles[i, j + 1].LinkNumberTile.NumberLevel)
                {
                    //print(string.Format("sour:{0} , dest:{1}", currTileNumLev, this.allTiles[i, j + 1].LinkNumberSprite.NumberLevel));
                    return State.WaitingForInput;
                }
                //다음 행과 동급인지 확인
                if (i + 1 < this.row &&
                    currTileNumLev == this.allTiles[i + 1, j].LinkNumberTile.NumberLevel)
                {
                    //print(string.Format("sour:{0} , dest:{1}", currTileNumLev, this.allTiles[i + 1, j].LinkNumberSprite.NumberLevel));
                    return State.WaitingForInput;
                }
            }
        }
        //모두 검사해보았지만 도저히 합쳐질만한 요소 없음
        //올클리어 여부 확인하여 반환값 결정
        //return allMaxNumber == true ? State.Perfect : State.GameOver;
        return State.GameOver;
    }
                
    //숫자타일들이 움직이는지 확인
    public bool TileMoving()
    {
        //활성화 숫자타일들을 순회하며
        //foreach(NumberTile numberTile in this.numberTiles)
        //    if (numberTile.IsMove == true) //하나라도 움직이고 있다면..
        //        return true;
        for (int i = 0; i < this.numberTiles.Count; i++)
            if (this.numberTiles[i].IsMove)
                return true;

        return false;
    }

    public void SaveCurrentInfo()
    {
        //List<sSaveInfo> saveInfoList = new List<sSaveInfo>();
        cSaveInfo saveInfos = new cSaveInfo(this.numberTiles.Count);
        //saveInfos.saveInfoArray = new sSaveInfo[this.numberTiles.Count];
        for (int i = 0; i < this.numberTiles.Count; i++)
        {
            //if (this.numberTiles[i].CurrentTile.Equals(null))
            //    continue;

            //sSaveInfo newSaveInfo = new sSaveInfo(
            //saveInfos.saveInfoArray[i] = new sSaveInfo(
            //    this.numberTiles[i].CurrentTile.Index_X,
            //    this.numberTiles[i].CurrentTile.Index_Y,
            //    this.numberTiles[i].NumberLevel);

            //saveInfoList.Add(newSaveInfo);

            saveInfos.tileIndex_x[i] = this.numberTiles[i].CurrentTile.Index_X;
            saveInfos.tileIndex_y[i] = this.numberTiles[i].CurrentTile.Index_Y;
            saveInfos.numberLevel[i] = this.numberTiles[i].NumberLevel;
        }
        //cSaveInfo saveInfos = new cSaveInfo(saveInfoList.ToArray());
        string strJson = JsonUtility.ToJson(saveInfos);
        PlayerPrefs.SetString("SaveInfos", strJson);
        PlayerPrefs.Save();
        //print("마지막 위치 정보 갯수 : " + saveInfoList.Count);
    }

    public bool LoadLastInfo()
    {
        if (PlayerPrefs.HasKey("SaveInfos"))
        {
            cSaveInfo loadInfos =
                JsonUtility.FromJson<cSaveInfo>(PlayerPrefs.GetString("SaveInfos"));
            if (loadInfos == null || loadInfos.tileIndex_x == null ||
                loadInfos.tileIndex_y == null || loadInfos.numberLevel == null)
            {
#if SHOW_DEBUG_MESSAGES
                print("Fail PlayerPrefs Json Load");
#endif
                return false;
            }

            for (int i = 0; i < loadInfos.tileIndex_x.Length; i++)
            {
                this.allTiles[
                    loadInfos.tileIndex_x[i], loadInfos.tileIndex_y[i]
                    ].AddNumberSprite(loadInfos.numberLevel[i]);
                this.SetEmptyTiles(
                    this.allTiles[loadInfos.tileIndex_x[i], loadInfos.tileIndex_y[i]], false);
                this.SetNumberTiles(
                    this.allTiles[loadInfos.tileIndex_x[i], loadInfos.tileIndex_y[i]].LinkNumberTile, true);
            }
            return true;
        }//if

        return false;
    }

    //true이면 업그레이드 여부 확인한거고 false이면 그냥 이동 여부 확인한것
    private bool UpgradeOrMove(BackTile sourTile, BackTile destTile, ref bool unableMove)
    {
        //print(string.Format("sourName:{0} , destName:{1}", sourTile.name, destTile.name));

        if (destTile.LinkNumberTile == null) //비교 대상 타일에 숫자타일이 없다면 패스
            return false;

        if (sourTile.LinkNumberTile != null) //기준 타일에 숫자가 존재하면 업그레이드 파악하자
        {
            //int sourNumLev = sourTile.LinkNumberSprite.NumberLevel;
            NumberLevel sourNumLev = sourTile.LinkNumberTile.NumberLevel; //기준 타일의 숫자레벨 기억
            //등급 같아서 업그레이드 가능하다면
            //if (sourNumLev < this.numberSrcImgs.Length - 1 && //최대 등급이 아니고
            if (sourNumLev < NumberLevel._2048 && //기준타일이 최대 등급이 아니고
                sourNumLev == destTile.LinkNumberTile.NumberLevel) //비교타일과 등급이 서로 같다면
            {
                //일단 임시로 움직임 없이 바로 변경되도록..추후에 변경바람
                //sourTile.LinkNumberTile.ChangeNumber(++sourNumLev);
                //destTile.RemoveNumberSprite();
                //this.SetTileList(destTile, false);

                //일단 기준타일에 업글 예약 및 이동할 비교 숫자타일 전달
                sourTile.ReserveUpgrade(destTile.LinkNumberTile);
                destTile.DisconnectNumberTile(); //원래 주인 백타일은 연결을 끊는다
                this.SetEmptyTiles(destTile, true); //비활성화 목록 추가
                unableMove = false; //단 하나라도 움직일 수 있다.

                //스코어매니저에 전달
                //ScoreManager.Instance.AddScore(sourTile.LinkNumberSprite.NumberLevel);
            }

            //업그레이드 성립 여부 상관없이 더 이상의 검사 진행은 무의미
            return true;
        }
        else //기준 타일에 숫자가 없다면 이동 명령
        {
            //일단 임시로 움직임 없이 바로 변경되도록..추후에 변경바람
            //sourTile.AddNumberSprite(destTile.LinkNumberSprite.NumberLevel);
            //this.SetTileList(sourTile, true);
            //destTile.RemoveNumberSprite();
            //this.SetTileList(destTile, false);

            sourTile.ConnectNumberTile(destTile.LinkNumberTile); //기준타일에 새로운 숫자타일 연결하기
            destTile.DisconnectNumberTile(); //이동할려는 숫자타일의 원래 주인 백타일 연결 끊기
            this.SetEmptyTiles(sourTile, false); //기준 백타일은 목록에서 제거
            this.SetEmptyTiles(destTile, true); //비교 타일은 비활성화 목록 추가
            unableMove = false; //단 하나라도 움직일 수 있다.

            return false;
        }
    }

    //숫자타일들을 이동시켜라
    private void MoveTile(InputDirection inputDir)
    {
        if (inputDir == InputDirection.NONE)
        {
#if SHOW_DEBUG_MESSAGES
            print("입력한게 없는데 호출되었다");
#endif
            return;
        }

        //foreach (NumberTile numberTile in this.numberTiles)
        //    numberTile.NowMove(inputDir);
        for (int i = 0; i < this.numberTiles.Count; i++)
            this.numberTiles[i].NowMove(inputDir);
    }
}
