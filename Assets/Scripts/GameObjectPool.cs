using UnityEngine;
using System.Collections;
using System.Text;

public class GameObjectPool : MonoBehaviour {

    private GameObject srcGameObject;
    private int poolNum;
    public GameObject[] cloneGameObjects;
    private int activeCount = 0;

    public bool ReadyPool(string upperPath, string objectName, int poolNum)
    {
        this.poolNum = poolNum;
        StringBuilder strBldr = new StringBuilder(upperPath);
        strBldr.Append(objectName);
        this.srcGameObject = Resources.Load(strBldr.ToString()) as GameObject;
            //string.Format("{0}/{1}", upperPath, objectName)) as GameObject;
            
        if (this.srcGameObject == null)
        {
            Debug.Log("Not Found SrcGameObject");
            return false;
        }

        this.cloneGameObjects = new GameObject[this.poolNum];

        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;

        for (int i = 0; i < this.poolNum; i++)
        {
            GameObject newClone = Instantiate(
                this.srcGameObject,
                Vector3.zero,
                Quaternion.identity) as GameObject;

            newClone.SetActive(false);

            newClone.transform.parent = this.transform;

            this.cloneGameObjects[i] = newClone;
        }

        return true;
    }
    
    public GameObject ActiveGameObject(Vector2 position)//, Quaternion rotation)
    {
        GameObject newActive = this.cloneGameObjects[this.activeCount++];

        int len = this.poolNum; //조건문 검사가 헷갈리기 때문에 주의 바람.
        while (newActive.activeSelf == true) //찾은 풀이 활성화상태이면 다시 검사
        {
            //모든 풀을 검사하였지만 전부 다 활성화 상태이다..
            if (len <= 0)
            {
                print("모든 풀이 활성화 상태이다.");
                return null;
            }
            --len;
            if (this.activeCount >= this.poolNum)
                this.activeCount = 0;
            newActive = this.cloneGameObjects[activeCount++];            
        }
        if (this.activeCount >= this.poolNum)
            this.activeCount = 0;

        newActive.SetActive(true);

        newActive.transform.position = position;
        //newActive.transform.rotation = rotation;

        return newActive;        
    }

    public bool AddPool(GameObject parentGameObj)
    {
        Transform[] tempTMs = parentGameObj.GetComponentsInChildren<Transform>();
        if (tempTMs.Length <= 1)
        {
            print("요청한 게임오브젝트의 하위자식들이 존재하지 않는다");
            return false;
        }

        this.poolNum = tempTMs.Length - 1;

        this.cloneGameObjects = new GameObject[this.poolNum];

        for (int i = 0; i < this.poolNum; i++)
        {
            //print(tempTMs[i].gameObject.name);
            this.cloneGameObjects[i] = tempTMs[i+1].gameObject;
            this.cloneGameObjects[i].SetActive(false);
        }

        return true;
    }
}
