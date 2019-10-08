using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public enum SwatterState
{
    Upright = 0,
    Floored,
    Slamming,
    Retracting,
    ReCharging
}

public class Swatter : MonoBehaviour
{
    [Header("Swatter Settings")]
    [SerializeField] private GameObject m_hitter;
    [SerializeField] private float m_rechargeTime;
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private int m_SlamdegreesPerSecond;
    [SerializeField] private int m_RetractdegreesPerSecond;

    [Header("Player Settings")]
    [SerializeField] private int m_playerNumber;

    private SwatterState m_swatterState;

    private void Start()
    {
        if(m_SlamdegreesPerSecond <= 0)
        {
            Debug.LogError("Swatter: \"m_SlamdegreesPerSecond\" <= 0, please make this more then 0 in the inspector");
            return;
        }
        if(m_RetractdegreesPerSecond <= 0)
        {
            Debug.LogError("Swatter: \"m_RetractdegreesPerSecond\" <= 0, please make this more then 0 in the inspector");
            return;
        }

        StartCoroutine(GetUserInput());
    }

    private IEnumerator GetUserInput()
    {
        while (true) // "game !paused" ?,   add a check if any button is being pressed.
        {
            if (Input.GetAxis("SwatterSlam") != 0)
            {
                if (m_swatterState == SwatterState.Upright)
                {
                    StartCoroutine(Slam());
                }
            }
            if (m_swatterState == SwatterState.Upright || m_swatterState == SwatterState.ReCharging)
            {
                Vector3 xboxInput = new Vector3(Input.GetAxis("Horizontalxbox" + m_playerNumber.ToString()), 0, Input.GetAxis("Verticalxbox" + m_playerNumber.ToString()));
                transform.position += xboxInput * Time.deltaTime * m_moveSpeed;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Slams down the swatter, in an attemot to hit the ants (players)
    /// </summary>
    private IEnumerator Slam()
    {
        m_swatterState = SwatterState.Slamming;
        yield return StartCoroutine(RotateDown());

        m_swatterState = SwatterState.Floored;
        checkForCollision();

        yield return new WaitForSeconds(1.5f);
        StartCoroutine(RotateUp());
    }

    private void checkForCollision()
    {
        //debug
        print(m_hitter.transform.position);
        Collider[] colls = Physics.OverlapBox // transforms of the hitter gameobject
        #region Get colliders hit by the hitter gameobject
            (
            m_hitter.transform.position,
            new Vector3(m_hitter.GetComponent<MeshRenderer>().bounds.size.x * 0.5f,
            m_hitter.GetComponent<MeshRenderer>().bounds.size.y * 0.5f,
            m_hitter.GetComponent<MeshRenderer>().bounds.size.z * 0.5f)
            );
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
        while(rotatedDegrees < 90 / m_SlamdegreesPerSecond)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x + m_SlamdegreesPerSecond, transform.eulerAngles.y, transform.eulerAngles.z);
            rotatedDegrees += 1;
            yield return new WaitForSeconds(0.025f);
        }
    }

    private IEnumerator RotateUp()
    {
        m_swatterState = SwatterState.Retracting;
        int rotatedDegrees = 0;
        while (rotatedDegrees < 90 / m_RetractdegreesPerSecond)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x - m_RetractdegreesPerSecond, transform.eulerAngles.y, transform.eulerAngles.z);
            rotatedDegrees += 1;
            yield return new WaitForSeconds(0.025f);
        }
        m_swatterState = SwatterState.ReCharging;
        StartCoroutine(Recharge(m_rechargeTime));
    }

    private IEnumerator Recharge(float _rechargeTime)
    {
        yield return new WaitForSeconds(_rechargeTime);
        m_swatterState = SwatterState.Upright;
    }
}