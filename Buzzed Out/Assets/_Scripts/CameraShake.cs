using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float amount;
    [SerializeField] private float decrease;

    private Vector3 OriginalPosition; // the position of the camera before the shake starts
    private float m_shakeDuration = 2f;
    private float m_shakeAmount = 0.7f;
    private float m_decreaseFactor = 1f;
    private bool shaking = false;

    public void Shake(float _shakeDuration, float _shakeAmount, float _shakeDecreaseFactor)
    {
        if (!shaking)
        {
            OriginalPosition = transform.position; // original position of the camera before the shake starts
            m_shakeDuration = _shakeDuration; // factors for the shake
            m_shakeAmount = _shakeAmount;
            m_decreaseFactor = _shakeDecreaseFactor;

            if (m_shakeDuration <= 0) { m_shakeDuration = 1; }
            if (m_shakeAmount <= 0) { m_shakeAmount = 1; }
            if (m_decreaseFactor <= 0) { m_decreaseFactor = 1; }

            shaking = true; // shake the camera
        }
    }

    // Entire script needs editing, this is just an old template! -----------------------------
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Shake(duration, amount, decrease);
        //}

        if (shaking)
        {
            if (m_shakeDuration > 0)
            {
                transform.position = OriginalPosition + (Random.insideUnitSphere * m_shakeAmount * m_shakeDuration);
                m_shakeDuration -= Time.deltaTime * m_decreaseFactor;
            }
            else
            {
                transform.position = OriginalPosition;
                m_shakeDuration = 0;
                shaking = false;
            }
        }
    }
}