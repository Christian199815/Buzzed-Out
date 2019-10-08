using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Ant : MonoBehaviour
{
    [SerializeField] private int m_playerNumber;
    [SerializeField] private int m_moveSpeed;

    private void Start()
    {
        StartCoroutine(GetUserInput());
    }

    private IEnumerator GetUserInput()
    {
        while (true) // "game !paused" ?,   add a check if any button is being pressed.
        {
            transform.position += new Vector3(Input.GetAxis("Horizontalxbox" + m_playerNumber.ToString()), 0, Input.GetAxis("Verticalxbox" + m_playerNumber.ToString())) * Time.deltaTime * m_moveSpeed;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Hit()
    {
        print(name + ": was hit");
    }
}
