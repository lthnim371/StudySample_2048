using UnityEngine;
using System.Collections;

public abstract class MenuPanel : MonoBehaviour {

    //읽기전용 str
    protected readonly string STR_ZERO = "";
    protected readonly string STR_CLOSENOW = "CloseNow";

    private TweenScale myTweenScale; //조정할 본체 트윈스케일

    void Awake()
    {
        //갖고온다
        this.myTweenScale = this.GetComponent<TweenScale>();
        //this.gameObject.SetActive(false);
        //본체에 설정된 박스컬리더로 인해 그리드 백타일 설정에 오류가 생기므로
        //처음부터 객체 비활성화상태로 두어야 한다.
    }

    void OnEnable() //활성화될때마다 메뉴창 등장효과를 표현하자
    {
        this.myTweenScale.Play(true); //이전에 셋팅이 다 되어있으므로 실행만 해주자
        //GameManager.Instance.OnMenu(this); //게임매니저에 메뉴창 활성화되었다고 알리자
        GameManager.Instance.SetPause(true);
        this.Open();
    }

    //void OnDisable()
    //{
    //    this.myTweenScale.Reset();
    //    this.myTweenScale.callWhenFinished = this.STR_ZERO;
    //}

    //뒤로가기 버튼 누르면 게임매니저에서 호출
    public void ReadyClose()
    {
        //퇴장효과 후 비활성화를 위한 피니쉬함수 적용
        this.myTweenScale.callWhenFinished = STR_CLOSENOW;
        //퇴장효과를 위해 프롬, 투 위치 반대로 해주기
        Vector3 tempVec = this.myTweenScale.from;
        this.myTweenScale.from = this.myTweenScale.to;
        this.myTweenScale.to = tempVec;
        //리셋하고 실행
        this.myTweenScale.Reset();
        this.myTweenScale.Play(true);
    }

    //퇴장효과 끝나고 본체 비활성화를 위함
    public void CloseNow()
    {
        this.myTweenScale.callWhenFinished = this.STR_ZERO; //다음 활성화를 위해 피니쉬함수 제거
        //원상복귀
        Vector3 tempVec = this.myTweenScale.from;
        this.myTweenScale.from = this.myTweenScale.to;
        this.myTweenScale.to = tempVec;
        //활성화 효과를 위해 리셋해주고 비활성화
        this.myTweenScale.Reset();
        this.gameObject.SetActive(false);
        //게임매니저 일시정지 풀기
        GameManager.Instance.SetPause(false);
    }

    //메뉴창 열릴 때 필요한 내용이 있다면 자식클래스에서 작성
    protected abstract void Open();
}
