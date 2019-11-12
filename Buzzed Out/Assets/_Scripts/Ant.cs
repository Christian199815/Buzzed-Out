using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;
using System;

public class Ant : MonoBehaviour
{
    [Header("Ant Settings")]
    [SerializeField] private int m_playerNumber;
    [SerializeField] private Image m_healthBar;
    [SerializeField] private Canvas m_canvas;
    [SerializeField, Range(0.01f, 1f)] private float m_negationInputStrenghtThreshhold = 0.3f;

    [Header("Ant Stats")]
    [Tooltip("if the inputStrenght from the joystick is lower than the threshhold, it is ignored (set to 0), so the ant doesn't move\nDefault: 0.3")]
    [SerializeField] private float m_maxHealth;
    [SerializeField] private int m_moveSpeed;
    [SerializeField, Range(1, 10)] private float m_rotationSpeed;

    private Rigidbody rb;
    private GameManager m_game;
    private AudioSource m_aahAudio;
    private float currentHealth;
    private int m_originalMoveSpeed;
    private bool m_slowed = false;

    private void Start()
    {
        m_originalMoveSpeed = m_moveSpeed;
        m_aahAudio = GetComponent<AudioSource>();
        m_game = FindObjectOfType<GameManager>();
        if(m_game == null)
        {
            Debug.LogError(gameObject.name + " could not find the GameManager");
            return;
        }
        m_game.AddAntToList(this);
        RotateCanvasTowardsCamera();
        currentHealth = m_maxHealth;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(HandleMovementByUserInput());
        //StartCoroutine(RestrictRigidBodyY(1));
    }

    private IEnumerator HandleMovementByUserInput()
    {
        while (true) // "game !paused" ?,   add a check if any button is being pressed.
        {
            if (!m_game.GetGamePaused())
            {
                Vector3 xboxInput = new Vector3(Input.GetAxis("Horizontalxbox" + m_playerNumber.ToString()), 0, Input.GetAxis("Verticalxbox" + m_playerNumber.ToString()));

                Vector3 targetDir = (transform.position + xboxInput) - transform.position;
                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * m_rotationSpeed, 0.0f));

                float positiveXboxInputX;
                float positiveXboxInputZ;
                if (xboxInput.x < 0) { positiveXboxInputX = (float)xboxInput.x * -1f; } else { positiveXboxInputX = xboxInput.x; }
                if (xboxInput.z < 0) { positiveXboxInputZ = (float)xboxInput.z * -1f; } else { positiveXboxInputZ = xboxInput.z; }

                int nonZerosInXboxInput = 0;
                if (xboxInput.x != 0) { nonZerosInXboxInput += 1; }
                if (xboxInput.z != 0) { nonZerosInXboxInput += 1; }
                float inputStrenth = ((positiveXboxInputX + positiveXboxInputZ) / nonZerosInXboxInput);

                if (inputStrenth >= m_negationInputStrenghtThreshhold)
                {
                    Vector3 forward;
                    forward = new Vector3(0, 0, inputStrenth);
                    transform.Translate(forward * m_moveSpeed * Time.deltaTime, Space.Self);
                    RotateCanvasTowardsCamera();
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator ResetMoveSpeed(float _duration)
    {
        yield return new WaitForSeconds(_duration);
        m_moveSpeed = m_originalMoveSpeed;
    }

    private IEnumerator RestrictRigidBodyY(float _timeUntilREstriction)
    {
        yield return new WaitForSeconds(_timeUntilREstriction);
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    public void Hit(float _damage)
    {
        print(name + ": was hit");

        m_aahAudio.Play();
        bool alive = ChangeHealth(-_damage);

        if (!alive)
        {
            Death();
        }
    }
    /// <summary>
    /// chages the currentHealth of the ant, returns a bool: true = alive, false = dead
    /// </summary>
    /// <param name="amountToAdd">health to be added / substracted</param>
    /// <returns>alive = true, dead = false</returns>
    private bool ChangeHealth(float amountToAdd)
    {
        currentHealth += amountToAdd;
        UpdateHealthBar();
        if(currentHealth <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void RotateCanvasTowardsCamera()
    {
        m_canvas.transform.LookAt(Camera.main.transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.up);
    }

    private void UpdateHealthBar()
    {
        m_healthBar.fillAmount = (float)currentHealth / (float)m_maxHealth;
    }

    private void Death()
    {
        print(name + ": ded, dead, died");

        m_game.AntDied(this);

        // remove this when we get an ant animation for dying
        DisableAnt();
    }

    /// <summary>
    /// call this when you want the ant object to be disabled
    /// </summary>
    private void DisableAnt()
    {
        transform.root.gameObject.SetActive(false);
    }

    public void GiveMoveSpeedBoost(int _extraSpeed, float _duration)
    {
        m_moveSpeed += _extraSpeed;
        StartCoroutine(ResetMoveSpeed(_duration));
    }

    public void SlowDown(int amount)
    {
        if (!m_slowed)
        {
            m_slowed = true;
            m_moveSpeed -= amount;
            print("slow: " + m_moveSpeed);
        }
    }

    public void SpeedUp(int amount)
    {
        if (m_slowed)
        {
            m_slowed = false;
            m_moveSpeed += amount;
            print("fast: " + m_moveSpeed);
        }
    }
}