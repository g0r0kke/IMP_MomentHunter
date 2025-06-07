using UnityEngine;

public class FollowUIManager : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // XR Origin의 Camera Transform
    [SerializeField] private float distance = 2.0f;     // 카메라 앞 거리
    [SerializeField] private float yOffset = 0.0f;      // 높이 오프셋
    [SerializeField] private float smoothTime = 0.2f;   // 부드러운 이동을 위한 시간

    private Vector3 velocity = Vector3.zero;
    private Quaternion targetRotation;

    void LateUpdate()
    {
        // 1. 위치 업데이트
        Vector3 targetPosition = cameraTransform.position +
                               cameraTransform.forward * distance +
                               Vector3.up * yOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        // 2. 회전 업데이트
        targetRotation = Quaternion.LookRotation(
            transform.position - cameraTransform.position, // 카메라 → UI 방향
            cameraTransform.up
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            smoothTime * 2
        );
    }
}
