using UnityEngine;
using System.Collections;
//using System;

public class NumberTile : MonoBehaviour {

    private SpriteRenderer mySpriteRenderer;
    private UISprite myUiSprite;
    private EmptyTile currentTile;
    private EmptyTile goalTile;
    //private int numberLevel = 0;
    private NumberLevel numberLevel = global::NumberLevel.NONE;

    void Awake()
    {
        this.mySpriteRenderer = this.GetComponent<SpriteRenderer>();
        this.myUiSprite = this.GetComponent<UISprite>();
    }

    //비활성과 동시에 초기화
    void OnDisable()
    {
        //this.numberLevel = -1; //일단 첫번째 등급이 아니게 처리..
        this.numberLevel = global::NumberLevel.NONE;
        //this.mySpriteRenderer.sprite = null;
        this.myUiSprite.spriteName = "2";
    }

    //public int NumberLevel
    //{
    //    get { return this.numberLevel; }
    //}

    public NumberLevel NumberLevel
    {
        get { return this.numberLevel; }
    }

    //숫자등급 및 이미지 변경
    //public bool ChangeNumber(int numberLevel)
    public bool ChangeNumber(NumberLevel numberLevel)
    {
        //동일하다면 문제가 있다.
        //if (this.numberLevel == numberLevel)
        if (this.numberLevel == numberLevel)
        {
            print("등급이 중복된다.");
            return false;
        }
        this.numberLevel = numberLevel;
        //this.mySpriteRenderer.sprite = GridManager.Instance.numberSrcImgs[this.numberLevel].sprite;
        this.myUiSprite.spriteName = numberLevel.ToString().Substring(1);

        return true;
    }
}
