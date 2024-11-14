using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendelum : MonoBehaviour
{
    public Transform pivot;           // The pivot of the pendulum (should be moving with crane)
    public float length = 5f;         // Length of the pendulum
    public float gravity = 9.81f;     // Gravitational constant
    public float damping = 0.995f;    // Damping factor to reduce motion over time
    public float initialAngle = 45f;  // Initial angle in degrees
    public float angularVelocity = 0f; // Initial angular velocity

    private float angleInRadians;     // Angle in radians
    private float angularAcceleration; // Angular acceleration
    private Vector3 previousPivotPosition; // To store the previous position of the pivot

    void Start()
    {
        // Convert the initial angle from degrees to radians
        angleInRadians = initialAngle * Mathf.Deg2Rad;
        
        // Initialize previousPivotPosition
        previousPivotPosition = pivot.position;
    }

    void Update()
    {
        // Calculate movement of the pivot and adjust pendulum motion accordingly
        Vector3 pivotDelta = pivot.position - previousPivotPosition;
        
        // Update the pendulum's motion based on the current pivot position and movement
        UpdatePendulumMotion(pivotDelta);

        // Update the pendulum's position relative to the moving pivot
        UpdatePendulumPosition();

        // Store the current pivot position for the next frame
        previousPivotPosition = pivot.position;
    }

    void UpdatePendulumMotion(Vector3 pivotDelta)
    {
        // Calculate angular acceleration (force due to gravity)
        angularAcceleration = -(gravity / length) * Mathf.Sin(angleInRadians);

        // Update the angular velocity based on the angular acceleration
        angularVelocity += angularAcceleration * Time.deltaTime;

        // Adjust angular velocity based on the pivot movement
        float pivotAngleEffect = Mathf.Atan2(pivotDelta.y, pivotDelta.x);
        angularVelocity += pivotAngleEffect / length;

        // Update the angle based on the angular velocity
        angleInRadians += angularVelocity * Time.deltaTime;

        // Apply damping to reduce motion over time
        angularVelocity *= damping;
    }

    void UpdatePendulumPosition()
    {
        if (pivot != null)
        {
            // Calculate the new position of the pendulum based on the angle and pivot position
            Vector3 pendulumPosition = pivot.position + new Vector3(Mathf.Sin(angleInRadians) * length,
                                                                    -Mathf.Cos(angleInRadians) * length,
                                                                    0f);

            // Update the pendulum's position in the world
            transform.position = pendulumPosition;
        }
    }
}
