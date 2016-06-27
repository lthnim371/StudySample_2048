using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance = null;
    public static T Instance
    {
        get {
            if (instance == null)
            {
                //씬에 존재하는 객체를 대상으로 하였기 때문에
                //씬 사이를 오고갈 경우 문제가 될 수 있다.
                //instance = FindObjectOfType(typeof(T)) as T;

                //그래서 임의의 이름으로 게임오브젝트를 새로 추가하고
                //자식클래스의 Awake에서 이름을 변경해주자
                GameObject newGameObj = new GameObject("_TempManager");
                instance = newGameObj.AddComponent<T>();
                
            }//if

            return instance;

        }//get
    }

    protected abstract void Awake(); //자식클래스에서 게임오브젝트 이름을 변경해주자
}
