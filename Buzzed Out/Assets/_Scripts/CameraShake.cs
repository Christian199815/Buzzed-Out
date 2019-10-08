using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Entire script needs editing, this is just an old template! -----------------------------
    [SerializeField] private GameObject Player;
    [SerializeField] private int shakeCounterTarget;
    public Transform CameraTransform;
    public float ShakeDuration = 2f;

    public float ShakeAmount = 0.7f;
    public float DecreaseFactor = 1f;

    Vector3 OriginalPosition;
    private float OriginalShakeDuration;
    private int ShakeCounter;
    // Entire script needs editing, this is just an old template! -----------------------------

    void Awake()
    {
        OriginalShakeDuration = ShakeDuration;
        if (CameraTransform == null)
        {
            CameraTransform = GetComponent<Transform>();
        }
    }
    // Entire script needs editing, this is just an old template! -----------------------------

    void OnEnable()
    {
        ShakeDuration = 2f;
        OriginalPosition = CameraTransform.position;
    }
    // Entire script needs editing, this is just an old template! -----------------------------

    private void Update()
    {
        if (ShakeDuration > 0)
        {
            if (ShakeCounter >= shakeCounterTarget)
            {
                CameraTransform.position = new Vector3(Player.transform.position.x, OriginalPosition.y, Player.transform.position.z) + Random.insideUnitSphere * ShakeAmount * ShakeDuration;
                ShakeCounter = 0;
            }
            else
            {
                ShakeCounter++;
            }
            ShakeDuration -= Time.deltaTime * DecreaseFactor;
        }
        else
        {
            ShakeDuration = 0;
            enabled = false;
        }
    }
}
// Entire script needs editing, this is just an old template! -----------------------------