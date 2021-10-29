
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minHeight = 5;
    public float maxHeight = 20;

    public float horizontalSpeed = 10;
    public float verticalSpeed = 10;


    private void Update()
    {
        Vector3 position = transform.position;
        
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = transform.right.normalized;
        

        float mult = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
        
        // Right/Left
        Vector3 rightSpeed =right * transform.position.y * mult * Input.GetAxis("Horizontal") * horizontalSpeed;
        Vector3 forwardSpeed = forward * transform.position.y * mult * Input.GetAxis("Vertical") * horizontalSpeed;
        Vector3 upSpeed = Vector3.up * Mathf.Log(transform.position.y) * -mult * Input.GetAxis("Mouse ScrollWheel") * verticalSpeed;
        
        
        Vector3 delta = Vector3.zero;
        delta += rightSpeed;
        delta += forwardSpeed;
        delta += upSpeed;

        position += delta * Time.deltaTime;


        position.y = Mathf.Clamp(position.y, minHeight, maxHeight);

        transform.position = position;

    }
}
