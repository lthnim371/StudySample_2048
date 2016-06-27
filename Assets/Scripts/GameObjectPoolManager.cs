using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPoolManager : Singleton<GameObjectPoolManager> {

    private Dictionary<string, GameObjectPool> poolTable;

    protected override void Awake()
    {
        this.gameObject.name = "_GameObjectPoolManager";

        this.poolTable = new Dictionary<string, GameObjectPool>();

        DontDestroyOnLoad(this.gameObject);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public GameObjectPool this[string name]
    {
        get
        {
            if (this.poolTable.ContainsKey(name) == false)
            {
                return null;
            }

            return this.poolTable[name];
        }
    }

    public GameObjectPool ReadyPool(string upperPath, string name, int num)
    {
        GameObject newGameObj = new GameObject(string.Format("GameObjectPool_{0}", name));
        GameObjectPool newPool = newGameObj.AddComponent<GameObjectPool>();

        if (newPool.ReadyPool(upperPath, name, num) == true)
        {
            newGameObj.transform.parent = this.transform;

            this.poolTable.Add(name, newPool);

            return newPool;
        }

        return null;
    }

    public GameObjectPool AddPool(string poolName, GameObject parentGameObj)
    {
        GameObjectPool newPool = parentGameObj.AddComponent<GameObjectPool>();
        if (newPool.AddPool(parentGameObj) == true)
        {
            this.poolTable.Add(poolName, newPool);
            return newPool;
        }
        return null;
    }
}
