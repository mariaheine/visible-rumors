using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOrbitalCamera : MonoBehaviour
{
    [Header("Dependencies")]
    public Transform target;

    [Header("Settings")]
    public float offsetSpeed = 2f;
    public float distance = 10f;
    public float offsetY = 2;
    public float rotateSpeed = 20f;

    float xSpeed = 100;
    float ySpeed = 100;

    float x;
    float y;

    void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.x;
        y = angles.y;
    }

    void Update()
    {
        if (target)
        {
            float rotationDelta = ySpeed * rotateSpeed / 100 * Time.deltaTime;
            y += rotationDelta;
            Quaternion rotation = Quaternion.Euler(x, y, 0);
            transform.rotation = rotation;
            Vector3 position = -transform.forward * distance + target.position;
            position.y += offsetY;
            transform.position = position;
        }
        
        if(Input.GetKey(KeyCode.Q))
        {
            offsetY += offsetSpeed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.E))
        {
            offsetY -= offsetSpeed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.W))
        {
            distance -= offsetSpeed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.S))
        {
            distance += offsetSpeed * Time.deltaTime;
        }
    }
}
