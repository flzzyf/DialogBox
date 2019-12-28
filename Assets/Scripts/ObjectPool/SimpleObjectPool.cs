using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectPool : Singleton<SimpleObjectPool>
{
    #region 字段

    public Dictionary<string, Pool> poolDic;

    public class Pool
    {
        public GameObject prefab;
        public Queue<GameObject> objectQueue;

        public Transform parent;

        public int amount;

        public Pool(GameObject prefab, Queue<GameObject> objectQueue, Transform parent, int amount)
        {
            this.prefab = prefab;
            this.objectQueue = objectQueue;

            this.parent = parent;
            this.amount = amount;
        }
    }

	#endregion

	#region Unity回调

	void Awake()
    {
        poolDic = new Dictionary<string, Pool>();
    }

    void Update()
    {
        if(Input.GetKeyDown("z"))
        {
            //输出未完全回收的池子
            Debug.Log("-----------------------------------------------------");
            foreach (var item in poolDic)
            {
                if(item.Value.amount != item.Value.objectQueue.Count)
                {
                    Debug.Log(string.Format("当前未完全回收的池子：{0}({1}/{2})", item.Key, item.Value.objectQueue.Count, item.Value.amount));
                }
            }
        }
    }

	#endregion

	#region 方法

    //生成新物体池子
    public void NewPool(string tag, GameObject prefab, int amount, Transform parent = null)
    {
        Queue<GameObject> objQueue = new Queue<GameObject>();
        
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.SetActive(false);

            objQueue.Enqueue(obj);
        }

        Pool pool = new Pool(prefab, objQueue, parent, amount);

        poolDic.Add(tag, pool);
    }

    //从对象池取出物体，可选是否取消激活（如果需要立即使用就false
    public GameObject SpawnFromPool(string tag, bool setInactive = true)
    {
        //DebugUtil.Out(string.Format("取出物体，Tag：{0}", tag), default, "1");

        //如果对象池已空，新建物体
        if (poolDic[tag].objectQueue.Count == 0)
        {
            AddObjectToPool(tag, 1, setInactive);
        }

        GameObject obj = poolDic[tag].objectQueue.Dequeue();

        obj.SetActive(true);

        IPoolObject poolObject = obj.GetComponent<IPoolObject>();
        if(poolObject != null)
        {
            poolObject.OnSpawn();
        }

        return obj;
    }

    //将物体放回对象池
    public void PutBackObject(string tag, GameObject obj)
    {
        //DebugUtil.Out(string.Format("回收物体：{0}，Tag：{1}", obj.name, tag));

        //重复放入判断
        if (poolDic[tag].objectQueue.Contains(obj))
        {
            Debug.Log(string.Format("回收物体：{0}，Tag：{1}", obj.name, tag));
            Debug.Log(string.Format("对象池重复放入同一物体：{0}", tag));

            return;
        }

        poolDic[tag].objectQueue.Enqueue(obj);

        obj.SetActive(false);

        obj.transform.SetParent(poolDic[tag].parent);

        IPoolObject poolObject = obj.GetComponent<IPoolObject>();
        if (poolObject != null)
        {
            poolObject.OnPutBack();
        }
    }

    //在某池添加新物体
    public void AddObjectToPool(string tag, int count = 1, bool setInactive = true)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(poolDic[tag].prefab, poolDic[tag].parent);

            if(setInactive)
                obj.SetActive(false);

            poolDic[tag].objectQueue.Enqueue(obj);
        }

        poolDic[tag].amount += count;
    }

    #endregion
}
