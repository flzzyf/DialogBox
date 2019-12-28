using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    //当被取出
    void OnSpawn();

    //当被回收
    void OnPutBack();
}
