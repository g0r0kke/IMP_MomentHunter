using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportArrivalDetector : MonoBehaviour
{
    public Transform xrOrigin;          // XR Origin (VR) 오브젝트
    public float detectRadius = 0.5f;   // 도착 인식 반경

    bool triggered = false;

    void Update()
{
    float dist = Vector3.Distance(transform.position, xrOrigin.position);
    // Debug.Log($"[거리 측정] XR Origin까지 거리: {dist}");

    if (triggered || dist > detectRadius) return;

    triggered = true;
    Debug.Log("텔레포트 위치 도착 감지됨");
    TutorialManager.Instance?.OnTeleportDone();
}

}

