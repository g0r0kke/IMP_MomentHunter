using UnityEngine;

// Makes a UI element smoothly follow and face the camera while maintaining a specified offset
public class FollowUIManager : MonoBehaviour
{
    // Camera References
    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;  // Target camera to follow (usually main camera)

    // Position References
    [Header("Position Settings")]
    [SerializeField] private float distance = 2.0f;     // Horizontal distance from camera
    [SerializeField] private float yOffset = 0.0f;      // Vertical offset from camera center

    // Motion References
    [Header("Motion Smoothing")]
    [SerializeField] private float smoothTime = 0.2f;   // Position smoothing duration

    private Vector3 velocity = Vector3.zero;            // Velocity reference for SmoothDamp
    private Quaternion targetRotation;                  // Target rotation for smooth facing

    // LateUpdate ensures camera movement completes before UI updates

    void LateUpdate()
    {
        // Calculate target position in camera's forward direction with offset
        Vector3 targetPosition = cameraTransform.position +
                               cameraTransform.forward * distance +
                               Vector3.up * yOffset;

        // Smoothly transition to target position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        // Calculate rotation to face camera while maintaining upright orientation
        targetRotation = Quaternion.LookRotation(
            transform.position - cameraTransform.position,  // Direction from camera to UI
            cameraTransform.up                               // Maintain world up axis
        );

        // Smoothly rotate towards target orientation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            smoothTime * 2  // Faster rotation than position movement
        );
    }
}
