using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendelum : MonoBehaviour
{
    public Transform pivot;             // The pivot of the pendulum (moving with crane)
    public float length = 5f;           // Length of the pendulum
    public float gravity = 9.81f;       // Gravitational constant
    public float damping = 0.995f;      // Damping factor to reduce motion over time
    public float initialAngleX = 45f;   // Initial angle in degrees along X-axis
    public float initialAngleZ = 45f;   // Initial angle in degrees along Z-axis

    private float angleInRadiansX;      // Angle in radians along X-axis
    private float angleInRadiansZ;      // Angle in radians along Z-axis
    private float angularVelocityX = 0f; // Initial angular velocity along X-axis
    private float angularVelocityZ = 0f; // Initial angular velocity along Z-axis
    private float angularAccelerationX; // Angular acceleration along X-axis
    private float angularAccelerationZ; // Angular acceleration along Z-axis
    private Vector3 previousPivotPosition; // To store the previous position of the pivot
    private Vector3 pivotVelocity;         // Current velocity of the pivot
    private Vector3 previousPivotVelocity;  // Previous frame's pivot velocity
    private Vector3 pivotAcceleration;      // Acceleration of the pivot

    void Start()
    {
        // Convert the initial angles from degrees to radians
        angleInRadiansX = initialAngleX * Mathf.Deg2Rad;
        angleInRadiansZ = initialAngleZ * Mathf.Deg2Rad;

        // Initialize previousPivotPosition
        previousPivotPosition = pivot.position;
        previousPivotVelocity = Vector3.zero;
    }

    void Update()
    {
        // Calculate pivot velocity and acceleration
        pivotVelocity = (pivot.position - previousPivotPosition) / Time.deltaTime;
        pivotAcceleration = (pivotVelocity - previousPivotVelocity) / Time.deltaTime;

        // Update pendulum motion based on current pivot acceleration
        UpdatePendulumMotion(pivotAcceleration);

        // Update pendulum position relative to the moving pivot
        UpdatePendulumPosition();

        // Store current pivot position and velocity for the next frame
        previousPivotPosition = pivot.position;
        previousPivotVelocity = pivotVelocity;
    }

    void UpdatePendulumMotion(Vector3 pivotAcceleration)
    {
        // Calculate gravitational angular acceleration
        angularAccelerationX = -(gravity / length) * Mathf.Sin(angleInRadiansX);
        angularAccelerationZ = -(gravity / length) * Mathf.Sin(angleInRadiansZ);

        // Apply pivot acceleration effect on angular acceleration, opposing pendulum movement
        angularAccelerationX += pivotAcceleration.z / length;
        angularAccelerationZ += pivotAcceleration.x / length;

        // Update angular velocities with the calculated angular acceleration
        angularVelocityX += angularAccelerationX * Time.deltaTime;
        angularVelocityZ += angularAccelerationZ * Time.deltaTime;

        // Update angles based on angular velocities
        angleInRadiansX += angularVelocityX * Time.deltaTime;
        angleInRadiansZ += angularVelocityZ * Time.deltaTime;

        // Apply damping to reduce motion over time
        angularVelocityX *= damping;
        angularVelocityZ *= damping;
    }

    void UpdatePendulumPosition()
    {
        if (pivot != null)
        {
            // Calculate the pendulum's new position based on the pivot and angle
            Vector3 pendulumOffset = new Vector3(
                Mathf.Sin(angleInRadiansX) * length,
                -Mathf.Cos(angleInRadiansX) * length,
                Mathf.Sin(angleInRadiansZ) * length
            );

            // Update pendulum position relative to the pivot
            transform.position = pivot.position + pendulumOffset;
        }
    }
}
