using UnityEngine;

public class FollowUIManager : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // XR Origin�� Camera Transform
    [SerializeField] private float distance = 2.0f;     // ī�޶� �� �Ÿ�
    [SerializeField] private float yOffset = 0.0f;      // ���� ������
    [SerializeField] private float smoothTime = 0.2f;   // �ε巯�� �̵��� ���� �ð�

    private Vector3 velocity = Vector3.zero;
    private Quaternion targetRotation;

    void LateUpdate()
    {
        // 1. ��ġ ������Ʈ
        Vector3 targetPosition = cameraTransform.position +
                               cameraTransform.forward * distance +
                               Vector3.up * yOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        // 2. ȸ�� ������Ʈ
        targetRotation = Quaternion.LookRotation(
            transform.position - cameraTransform.position, // ī�޶� �� UI ����
            cameraTransform.up
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            smoothTime * 2
        );
    }
}
