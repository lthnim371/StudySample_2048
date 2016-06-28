using UnityEngine;
using System.Collections;

public class GoogleMenu : MenuPanel {

    public UITexture profileImagerTexture;
    public UILabel profileNameLabel;

    private bool workingCoroutine = false;

    void OnDisable()
    {
        //만약에 코루틴 동작 중 메뉴창 닫을 경우를 대비하여 사용가능으로 설정해두기
        this.workingCoroutine = false;
    }

    protected override void Open()
    {
        if(this.profileImagerTexture.mainTexture == null ||
            this.profileNameLabel.text.CompareTo(this.STR_ZERO) == 0) //프로필 정보가 없다면
            if(!this.workingCoroutine) //실행중인 코루틴이 없다면
                StartCoroutine("GetProfile"); //코루틴으로 프로필 가져오기
    }

    //시간이 걸리므로 코루틴으로 프로필정보 가져오자
    IEnumerator GetProfile()
    {
        this.workingCoroutine = true; //코루틴 사용불가능 설정

        //로그인 시도한 후 다 될때까지 기다리자
        GPGSManager.Instance.Login();
        int frameCnt = 0; //제한시간
        while (!GPGSManager.Instance.IsLogged()) //로그인 상태가 아니라면
        {
            yield return new WaitForSeconds(1f); //일단 1초 쉬자
            if (++frameCnt >= 10) //10초동안 기다렸는데 변화가 없다면
                yield break; //그냥 종료하자
        }

        //여기까지 오면 로그인이 성공한거다.

        //임시 보관
        Texture2D tempTex = null;
        string tempStr = this.STR_ZERO;

        //얻어올때까지 계속 반복하자
        while (tempTex == null || tempStr.CompareTo(this.STR_ZERO) == 0)
        {
            //만약에 혹시라도 로그인 풀리면 중지시키기
            if (!GPGSManager.Instance.IsLogged())
                break;

            //이름 및 이미지 가져오기 
            tempStr = GPGSManager.Instance.GetProfileName();
            tempTex = GPGSManager.Instance.GetProfileImage();

            yield return null;
        }//while

        //가져오나 못 가져오나 일단 결과값 넣기
        this.profileNameLabel.text = tempStr;
        this.profileImagerTexture.mainTexture = tempTex;

        this.workingCoroutine = false; //코루틴 사용 가능 설정
    }
}
