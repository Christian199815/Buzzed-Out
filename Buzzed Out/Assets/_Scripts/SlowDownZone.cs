using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownZone : PoolItem
{
    [SerializeField] private int m_moveSpeedToRetract = 3;
    [SerializeField] private int m_lifeTime = 5;

    protected override void Reset()
    {
        StartCoroutine(countDownForDestroy());
    }

    private IEnumerator countDownForDestroy()
    {
        yield return new WaitForSeconds(m_lifeTime);
        ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        Ant ant = other.transform.root.gameObject.GetComponent<Ant>();
        if(ant != null)
        {
            ant.SlowDown(m_moveSpeedToRetract);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Ant ant = other.transform.root.gameObject.GetComponent<Ant>();
        if (ant != null)
        {
            ant.SpeedUp(m_moveSpeedToRetract);
        }
    }
}