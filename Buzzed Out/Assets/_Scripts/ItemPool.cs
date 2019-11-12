using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Pool
{
    public PoolItem poolItem;// { get; set; }
    public int poolSize;// { get; set; }
    public string ParentObjectName;
    [HideInInspector] public List<PoolItem> pool;// { get; set; }
    [HideInInspector] public Transform parentObject;// { get; set; }
}

public class ItemPool : MonoBehaviour
{
    public Pool[] pools;
    private void Awake()
    {
        for (int p = 0; p < pools.Length; p++)
        {
            if(p == 2)
            {
                print("2");
            }

            for (int i = 0; i < pools[p].poolSize; i++)
            {   
                AddNewItemToPool(pools[p].poolItem);
            }
        }
    }

    private void AddNewItemToPool(PoolItem item)
    {
        Pool p = GetPool(item);
        Transform parentObject = null;

        if (transform.childCount == 0)
        {
            parentObject = CreateParentObject(p).transform;
        }
        else
        {
            bool foundParent = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name == p.ParentObjectName)
                {
                    parentObject = transform.GetChild(i);
                    foundParent = true;
                    break;
                }
            }
            if (!foundParent)
            {
                parentObject = CreateParentObject(p).transform;
            }
        }

        p.parentObject = parentObject;
        PoolItem go = Instantiate(item.gameObject, parentObject).GetComponent<PoolItem>();
        go.name = go.name.Substring(0, go.name.Length - 7);
        AddItemToPool(go);
        go.pool = this;
    }

    private GameObject CreateParentObject(Pool pool)
    {
        GameObject go = new GameObject(pool.ParentObjectName);
        go.transform.parent = transform;
        return go;
    }

    public void AddItemToPool(PoolItem item)
    {
        Pool p = GetPool(item);
        p.pool.Add(item);

        item.transform.parent = p.parentObject;
        item.gameObject.SetActive(false);
        #region oldCode
        /*
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].poolItem.Equals(item))
            {
                pools[i].pool.Add(item);
                item.transform.parent = transform;
                item.gameObject.SetActive(false);
            }
        }
        */
        #endregion
    }

    public GameObject ItemInstatiate(PoolItem itemToInstantiate, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if(GetPool(itemToInstantiate).pool.Count <= 0)
        {
            print(gameObject.name + ": pool is empty");
            AddNewItemToPool(itemToInstantiate);

            GetPool(itemToInstantiate).poolSize += 1;
        }
        Pool p = GetPool(itemToInstantiate);

        p.pool[0].Initialize(position, rotation, parent);
        GameObject item = p.pool[0].gameObject;
        p.pool.RemoveAt(0);
        item.transform.parent = p.parentObject;
        return item;
    }

    private Pool GetPool(PoolItem whatPool)
    {
        for (int i = 0; i < pools.Length; i++)
        {
            string name = "";
            /*
            if(whatPool.name.ToLower().Contains("(clone)"))
            {
                name = whatPool.name.ToLower().Replace("(clone)", "");
            }
            */
            name = whatPool.name.ToLower();
            try
            {
                name = name.ToLower().Replace("(clone)", "");
            }
            catch { continue; }

            if (pools[i].poolItem.name.ToLower().Equals(name))
            {
                return pools[i];
            }
        }
        return null;
    }

    public void IncreasePoolsArrayLength()
    {
        // make the array "pools" one longer
        Array.Resize(ref pools, pools.Length + 1);
    }

    public Pool[] GetPools()
    {
        return pools;
    }
}