using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CraneController : MonoBehaviour
{
    public float moveSpeed = 5f;        // Speed of crane movement along the X and Z axes
    public float rotateSpeed = 50f;     // Speed of crane rotation

    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get horizontal input (left/right arrow keys or A/D keys)
        float moveHorizontal = Input.GetAxis("Horizontal"); // Left/Right Arrow or A/D keys
        
        // Get vertical input (up/down arrow keys or W/S keys)
        float moveVertical = Input.GetAxis("Vertical"); // Up/Down Arrow or W/S keys

        // Move the crane forward/backward (along the Z-axis)
        Vector3 moveDirection = transform.forward * moveVertical * moveSpeed * Time.deltaTime;

        // Move the crane left/right (along the X-axis)
        Vector3 strafeDirection = transform.right * moveHorizontal * moveSpeed * Time.deltaTime;

        // Apply horizontal movement (forward/backward and left/right)
        rb.MovePosition(rb.position + moveDirection + strafeDirection);

        // Rotate the crane based on left/right arrow key (or A/D)
        if (moveHorizontal != 0)
        {
            float rotationAmount = moveHorizontal * rotateSpeed * Time.deltaTime;
            Quaternion turnOffset = Quaternion.Euler(0, rotationAmount, 0);
            rb.MoveRotation(rb.rotation * turnOffset);
        }
    }
}
