using UnityEngine;
using System.Collections;

public class InputTouch : MonoBehaviour {

    public float flickRange = 3f;

    private Touch firstTouch; //플릿 거리 계산을 위한 첫 터치의 위치 보관
    private bool waitTouch = true; //플릿을 했는데 아직 터치를 안 떼고 있을 경우를 대비

    public InputDirection GetFlickDirection()
    {
        //반환값 초기화
        InputDirection currInputDir = InputDirection.NONE;
        if (Input.touchCount > 0) //터치가 있다면
        {
            //일단은 무조건 첫번째 터치 정보만 갖고온다..만약에 멀티터치를 고려할 경우 수정이 필요하다..
            Touch currTouch = Input.GetTouch(0);
            switch (currTouch.phase) //갖고 온 터치 상태로 판단해보자
            {
                case TouchPhase.Began: //터치가 처음 들어오면
                    this.firstTouch = currTouch; //첫번째 터치 위치를 기억하자
                    this.waitTouch = true; //터치 입력을 받을 준비를 하자.
                    break;
                case TouchPhase.Moved:
                    if (this.waitTouch == false) //플릿 후에도 터치되어있는 상태를 방지하기 위함
                        break;

                    //좌표별로 현재 터치 위치와 처음 터치 위치간의 차이값을 가져온다.
                    Vector2 distVec = new Vector2(
                        currTouch.position.x - this.firstTouch.position.x,
                        currTouch.position.y - this.firstTouch.position.y);
                    //거리 비교를 위한 절대값이 필요하다.
                    Vector2 absDistVec =
                        new Vector2(Mathf.Abs(distVec.x), Mathf.Abs(distVec.y));
                    //절차적 진행으로 인해 어쩔 수 없이 x좌표를 우선순위로 하였다..
                    //결과값 좌표간에 수치가 큰쪽으로 결정하여 첫 터치와의 거리가 일정 수치 이상이라면..
                    if (absDistVec.x >= absDistVec.y && absDistVec.x >= this.flickRange)
                    {
                        //0보다 크냐 아니냐로 방향을 알아내자
                        currInputDir = (distVec.x < 0) ? InputDirection.LEFT : InputDirection.RIGHT;
                        this.waitTouch = false; //플릿하였으므로 계산안하게 하자
                    }
                    else if (absDistVec.y >= absDistVec.x && absDistVec.y >= this.flickRange)
                    {
                        currInputDir = (distVec.y < 0) ? InputDirection.DOWN : InputDirection.UP;
                        this.waitTouch = false;
                    }
                    break;
                case TouchPhase.Ended:
                    
                    break;
            }
        }
        //위의 if문이 true가 아니라면 NONE으로 반환될것이다.
        return currInputDir;
    }

    
}
