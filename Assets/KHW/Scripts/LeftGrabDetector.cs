using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LeftGrabDetector : MonoBehaviour
{
    [Tooltip("이 오브젝트가 트리거 안에 들어오면 성공")]
    public GameObject targetObject;        // 검사할 물건 (예: 카메라 모형)

    public bool oneShot = true;            // true면 최초 1회만 발동

    Collider _zoneCollider;

    void Awake()
    {
        _zoneCollider = GetComponent<Collider>();
        _zoneCollider.isTrigger = true;   
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != targetObject) return;

        Debug.Log("[LeftGrabDetector] 구역 안에 대상 물건 확인 → 튜토리얼 진행");
        TutorialManager.Instance?.OnLeftGrabDone();

        if (oneShot) enabled = false;     
    }
}
