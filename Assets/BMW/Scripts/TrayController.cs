using UnityEngine;

public class TrayController : MonoBehaviour
{
    [Header("Init Parent Object")]
    [SerializeField] private Transform defaultParent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OverlayObject"))
        {
            other.transform.SetParent(transform);
        }
    }

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
