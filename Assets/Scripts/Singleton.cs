using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance
    {
        get
        {
			T[] temps = FindObjectsOfType<T>();

            //如果没有
            if(temps.Length == 0)
            {
                return new GameObject(typeof(T).Name).AddComponent<T>();
            }

			//总是返回最后一个，也就是最新的（从之前场景保留下来的
			return temps[temps.Length - 1];
        }
    }

    void Awake()
    {
        //已有实例则自毁
        if(FindObjectsOfType<T>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
