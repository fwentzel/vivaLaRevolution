
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float minHeight = 5;
    public float maxHeight = 20;

    public float horizontalSpeed = 10;
    public float verticalSpeed = 10;

    private InputActions.CameraActions cameraInput;

    private void Start()
    {
        cameraInput = InputActionsManager.instance.inputActions.Camera;
        cameraInput.Enable();
    }

    private void Update()
    {
        Vector3 position = transform.position;

        Vector3 forward = transform.forward;
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = transform.right.normalized;

        Vector2 movementInput = cameraInput.Move.ReadValue<Vector2>();
        Vector2 scrollInput = cameraInput.ScrollWheel.ReadValue<Vector2>();
        // Right/Left
        Vector3 rightSpeed = right * transform.position.y * movementInput.x * horizontalSpeed;
        Vector3 forwardSpeed = forward * transform.position.y * movementInput.y * horizontalSpeed;
        //scroll input either 120, -120 or 0. So scale it down
        Vector3 upSpeed = Vector3.up * Mathf.Log(transform.position.y) * -scrollInput.y * .1f * verticalSpeed;


        Vector3 delta = Vector3.zero;
        delta += rightSpeed;
        delta += forwardSpeed;
        delta += upSpeed;

        position += delta * Time.deltaTime;


        position.y = Mathf.Clamp(position.y, minHeight, maxHeight);

        transform.position = position;

    }
}
