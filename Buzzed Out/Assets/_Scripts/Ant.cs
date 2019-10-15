using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Ant : MonoBehaviour
{
    [SerializeField] private int m_playerNumber;
    [SerializeField] private int m_moveSpeed;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(GetUserInput());
        StartCoroutine(RestrictRigidBodyY(1));
    }

    private IEnumerator GetUserInput()
    {
        while (true) // "game !paused" ?,   add a check if any button is being pressed.
        {
            Vector3 xboxInput = new Vector3(Input.GetAxis("Horizontalxbox" + m_playerNumber.ToString()), 0, Input.GetAxis("Verticalxbox" + m_playerNumber.ToString()));
            transform.position += xboxInput * Time.deltaTime * m_moveSpeed;
            transform.LookAt(transform.position + xboxInput);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator RestrictRigidBodyY(float _timeUntilREstriction)
    {
        yield return new WaitForSeconds(_timeUntilREstriction);
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    public void Hit()
    {
        print(name + ": was hit");
        Death();
    }

    private void Death()
    {
        print(name + ": died");
    }
}
