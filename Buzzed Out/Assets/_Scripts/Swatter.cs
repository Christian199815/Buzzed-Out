﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Swatter : MonoBehaviour
{
    [SerializeField] private GameObject hitter;

    private bool slamming = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!slamming)
            {
                StartCoroutine(Slam());
            }
        }
    }

    /// <summary>
    /// Slams down the swatter, in an attemot to hit the ants (players)
    /// </summary>
    private IEnumerator Slam()
    {
        slamming = true;
        yield return StartCoroutine(RotateDown());

        checkForCollision();
    }

    private void checkForCollision()
    {
        Collider[] colls = Physics.OverlapBox // transforms of the hitter gameobject
        #region Get colliders hit by the hitter gameobject
            (
            hitter.transform.position,
            new Vector3(hitter.transform.position.x / 2,
            hitter.transform.position.y / 2,
            hitter.transform.position.z / 2));
        #endregion

        List<Ant> hitAnts = new List<Ant>();

        // if the hit collider is an ant (player) add it to "hitAnts"
        for (int i = 0; i < colls.Length; i++)
        {
            Ant ant = colls[i].gameObject.GetComponent<Ant>();
            if (ant != null)
            {
                hitAnts.Add(ant);
            }
        }
        // call the Hit() method on all the hit ants
        foreach (Ant a in hitAnts)
        {
            a.Hit();
        }
    }

    private IEnumerator RotateDown()
    {
        int rotatedDegrees = 0;
        while(rotatedDegrees < 30)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x + 3, transform.eulerAngles.y, transform.eulerAngles.z);
            rotatedDegrees += 1;
            yield return new WaitForSeconds(0.025f);
        }
    }
}
