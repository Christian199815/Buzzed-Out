using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;
using System;

public enum AntSpeed
{
    Slow,
    Normal,
    Fast
}

public class Ant : MonoBehaviour
{
    [Header("Object Settings")]
    [SerializeField] private int m_playerNumber;
    [SerializeField] private Image m_healthBarImage;
    [SerializeField] private Image m_speedBarImage;
    [SerializeField] private Canvas m_canvas;
    [SerializeField, Range(0.01f, 1f)] private float m_negationInputStrenghtThreshhold = 0.3f;

    [Header("Ant Stats")]
    [Tooltip("if the inputStrenght from the joystick is lower than the threshhold, it is ignored (set to 0), so the ant doesn't move\nDefault: 0.3")]
    [SerializeField] private float m_maxHealth;
    [SerializeField] private int m_moveSpeed;
    [SerializeField] private int m_speedBarMax;
    [SerializeField, Range(1, 10)] private float m_rotationSpeed;
    [SerializeField] private int m_speedBoostStrenght;
    [SerializeField] private float m_speedBoostDuration;

    private Rigidbody m_rb;
    private GameManager m_game;
    private AudioSource m_antHitAudio;
    private AntSpeed m_speedType;

    private float m_currentHealth;
    private float m_speedBarProgress = 0;

    private int m_originalMoveSpeed;

    private bool m_slowed = false;
    private bool m_spedUp = false;
    private bool m_UsingSpeedBoostAbility = false;

    private void Start()
    {
        m_originalMoveSpeed = m_moveSpeed;
        m_antHitAudio = GetComponent<AudioSource>();
        m_game = FindObjectOfType<GameManager>();
        if(m_game == null)
        {
            Debug.LogError(gameObject.name + " could not find the GameManager");
            return;
        }
        m_game.AddAntToList(this);
        RotateCanvasTowardsCamera();
        m_currentHealth = m_maxHealth;
        m_rb = GetComponent<Rigidbody>();
        StartCoroutine(HandleMovementByUserInput());
        UpdateBars();
    }

    private IEnumerator HandleMovementByUserInput()
    {
        while (true)
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

            if(Input.GetAxis("AntSpeedBoost" + m_playerNumber) != 0)
            {
                if (m_speedType != AntSpeed.Fast)
                {
                    StartCoroutine(DoSpeedBoostAbility());
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator DoSpeedBoostAbility()
    {
        print("ant " + m_playerNumber + ": speedBoost started.");
        m_UsingSpeedBoostAbility = true;
        SpeedUp(m_speedBoostStrenght);
        float timer = 0;
        while (timer < m_speedBoostDuration)
        {
            timer += Time.deltaTime;
            m_speedBarProgress = m_speedBoostDuration - ( timer / m_speedBoostDuration); // this is wrong <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            UpdateBars();   // make the speed bar go down fluently
            yield return new WaitForEndOfFrame();
        }
        SlowDown(m_speedBoostStrenght);
        m_UsingSpeedBoostAbility = false;

        print("ant " + m_playerNumber + ": speedBoost stopped.");

    }

    private IEnumerator ResetMoveSpeed(float _duration)
    {
        yield return new WaitForSeconds(_duration);
        m_moveSpeed = m_originalMoveSpeed;
    }

    private IEnumerator RestrictRigidBodyY(float _timeUntilREstriction)
    {
        yield return new WaitForSeconds(_timeUntilREstriction);
        m_rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    public void Hit(float _damage)
    {
        m_antHitAudio.Play();
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
        m_currentHealth += amountToAdd;
        UpdateBars();
        if(m_currentHealth > m_maxHealth) { m_currentHealth = m_maxHealth; }
        if(m_currentHealth <= 0)
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

    private void UpdateBars()
    {
        m_healthBarImage.fillAmount = (float)m_currentHealth / (float)m_maxHealth;
        m_speedBarImage.fillAmount = (float)m_speedBarProgress / (float)m_speedBarMax;
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
        if (m_speedType == AntSpeed.Normal)
        {
            m_speedType = AntSpeed.Slow;
            m_moveSpeed -= amount;
            print("slow: " + m_moveSpeed);
        }
        if (m_speedType == AntSpeed.Fast)
        {
            m_speedType = AntSpeed.Normal;
            m_moveSpeed -= amount;
            print("normal: " + m_moveSpeed);
        }
        if(m_speedType == AntSpeed.Normal)
        {
            m_moveSpeed = m_originalMoveSpeed;
        }
    }

    public void SpeedUp(int amount)
    {
        if (m_speedType == AntSpeed.Slow)
        {
            m_speedType = AntSpeed.Normal;
            m_moveSpeed += amount;
            print("normal: " + m_moveSpeed);
        }
        if (m_speedType == AntSpeed.Normal)
        {
            m_speedType = AntSpeed.Fast;
            m_moveSpeed += amount;
            print("fast: " + m_moveSpeed);
        }
        if (m_speedType == AntSpeed.Normal)
        {
            m_moveSpeed = m_originalMoveSpeed;
        }
    }

    public void PickedUpJellyBean(JellyBean bean)
    {
        switch (bean.GetBeanType())
        {
            case JellyBeanType.Health:
                ChangeHealth(1);
                UpdateBars();
                break;
            case JellyBeanType.MoveSpeed:
                if (!m_UsingSpeedBoostAbility)
                {
                    if (m_speedBarProgress < m_speedBarMax)
                    {
                        m_speedBarProgress += 1;
                    }
                    UpdateBars();
                }
                break;
        }
        m_game.SpawnBean(bean.GetComponent<PoolItem>());
    }
}