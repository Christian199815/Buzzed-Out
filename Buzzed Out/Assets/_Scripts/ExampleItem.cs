using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleItem : PoolItem
{
    private float lifeTimer;

    protected override void Reset()
    {
        lifeTimer = 0;
    }

    void Update()
    {
        transform.Translate(transform.forward * Time.deltaTime * 10);

        lifeTimer += Time.deltaTime;

        if(lifeTimer >= 3  )
        {
            ReturnToPool();
        }
    }
}