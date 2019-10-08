using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swatter : MonoBehaviour
{
    [SerializeField] private KeyCode forward;
    [SerializeField] private KeyCode backward;
    [SerializeField] private KeyCode left;
    [SerializeField] private KeyCode right;

    [SerializeField] private int Speed;
    void Start()
    {
        
    }

    void Update()
    {
        SwatterMovement();
    }


    void SwatterMovement()
    {
        if (Input.GetKey(forward))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (Speed * Time.deltaTime));
        }
        if (Input.GetKey(backward))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + -(Speed * Time.deltaTime));
        }
        if (Input.GetKey(left))
        {
            transform.position = new Vector3(transform.position.x + -(Speed * Time.deltaTime),transform.position.y , transform.position.z);
        }
        if (Input.GetKey(right))
        {
            transform.position = new Vector3(transform.position.x + (Speed * Time.deltaTime), transform.position.y, transform.position.z);
        }
    }
}