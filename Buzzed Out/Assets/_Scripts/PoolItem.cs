using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolItem : MonoBehaviour
{
    private ItemPool mPool;
    public ItemPool pool { set { mPool = value; } }

    protected abstract void Reset();

    public virtual void Initialize(Vector3 position, Quaternion rotation, Transform parent)
    {
        Reset();
        gameObject.SetActive(true);
        transform.position = position;
        transform.rotation = rotation;
    }

    public virtual void ReturnToPool()
    {
        mPool.AddItemToPool(this);
    }
}