using System.Collections;
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

        yield return new WaitForSeconds(1.5f);
        StartCoroutine(RotateUp());
    }

    private void checkForCollision()
    {
        //debug
        print(hitter.transform.position);
        Collider[] colls = Physics.OverlapBox // transforms of the hitter gameobject
        #region Get colliders hit by the hitter gameobject
            (
            hitter.transform.position,
            new Vector3(hitter.GetComponent<MeshRenderer>().bounds.size.x,
            hitter.GetComponent<MeshRenderer>().bounds.size.y,
            hitter.GetComponent<MeshRenderer>().bounds.size.z));
        #endregion

        List<Ant> hitAnts = new List<Ant>();

        // if the hit collider is an ant (player) add it to "hitAnts"
        for (int i = 0; i < colls.Length; i++)
        {
            Ant ant = colls[i].gameObject.transform.root.gameObject.GetComponent<Ant>();
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

    private IEnumerator RotateUp()
    {
        int rotatedDegrees = 0;
        while (rotatedDegrees < 30)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x - 3, transform.eulerAngles.y, transform.eulerAngles.z);
            rotatedDegrees += 1;
            yield return new WaitForSeconds(0.025f);
        }
    }
}
