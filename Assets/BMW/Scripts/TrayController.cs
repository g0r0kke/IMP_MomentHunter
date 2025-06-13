using UnityEngine;

// Controls the parenting of objects tagged as "OverlayObject" when they enter or exit the tray's trigger area.
public class TrayController : MonoBehaviour
{
    [Header("Init Parent Object")]
    [SerializeField] private Transform defaultParent; // The default parent to restore when object exits the tray

    // When an object with the "OverlayObject" tag enters the tray's trigger,
    // it becomes a child of the tray (this transform).
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OverlayObject"))
        {
            other.transform.SetParent(transform);
        }
    }

    // When an object with the "OverlayObject" tag exits the tray's trigger,
    // its parent is reset to the defaultParent if specified, otherwise unparented (set to null).
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("OverlayObject"))
        {
            if (defaultParent != null)
                other.transform.SetParent(defaultParent);
            else
                other.transform.SetParent(null);
        }
    }
}
