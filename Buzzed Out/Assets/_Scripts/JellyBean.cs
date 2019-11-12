using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JellyBeanType
{
    Health,
    MoveSpeed
}

public class JellyBean : PoolItem
{
    [SerializeField] private JellyBeanType m_beanType;

    protected override void Reset()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Ant ant = other.transform.root.GetComponent<Ant>();
        if(ant != null)
        {
            ant.PickedUpJellyBean(this);
            ReturnToPool();
        }
    }

    public JellyBeanType GetBeanType()
    {
        return m_beanType;
    }
}