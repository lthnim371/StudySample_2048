using UnityEngine;
using System.Collections;

//유용한 방법이나 씬에 이미 존재해야만 가능
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;
    public static T Instance
    {
        get {
            if (instance == null)
            {
                //씬에 존재하는 객체를 대상으로 하였기 때문에
                //씬 사이를 오고갈 경우 문제가 될 수 있다.
                instance = FindObjectOfType(typeof(T)) as T;

                if (instance == null)
                {
                    Debug.Log("Nothing Singleton");
                    return null;
                }//if
            }//if

            return instance;

        }//get
    }

}
