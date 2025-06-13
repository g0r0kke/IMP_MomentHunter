using UnityEngine;

public class FollowUIManager : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distance = 2.0f;
    [SerializeField] private float yOffset = 0.0f;
    [SerializeField] private float smoothTime = 0.2f;

    private Vector3 velocity = Vector3.zero;
    private Quaternion targetRotation;

    void LateUpdate()
    {
        
        Vector3 targetPosition = cameraTransform.position +
                               cameraTransform.forward * distance +
                               Vector3.up * yOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        
        targetRotation = Quaternion.LookRotation(
            transform.position - cameraTransform.position,
            cameraTransform.up
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            smoothTime * 2
        );
    }
}
