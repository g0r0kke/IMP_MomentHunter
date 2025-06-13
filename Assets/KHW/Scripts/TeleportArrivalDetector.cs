using UnityEngine;
public class TeleportArrivalDetector : MonoBehaviour
{
    public Transform xrOrigin;    // XR Origin (VR player)
    public float detectRadius = 0.5f;   // Detection radius
    bool triggered = false;  // Has teleport detected

    void Update()
    {
        // Calculate distance to XR Origin
        float dist = Vector3.Distance(transform.position, xrOrigin.position);
        // Debug.Log($"distance : {dist}");

        // If already triggered or too far, skip
        if (triggered || dist > detectRadius) return;

        // First time entering detection zone
        triggered = true;
        TutorialManager.Instance?.OnTeleportDone();   // Notify TutorialManager
    }
}

