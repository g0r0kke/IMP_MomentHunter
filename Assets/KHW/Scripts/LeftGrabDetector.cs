using UnityEngine;

public class LeftGrabDetector : MonoBehaviour
{
    public GameObject targetObject;      // Target object to detect
    public bool oneShot = true;          // Detect only once if true
    Collider _zoneCollider;

    void Awake()
    {
        _zoneCollider = GetComponent<Collider>();
        _zoneCollider.isTrigger = true;   // Set collider as trigger
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != targetObject) return;   // Only detect target object

        TutorialManager.Instance?.OnLeftGrabDone();    // Notify tutorial progress

        if (oneShot) enabled = false;     // Disable after first detection
    }
}
