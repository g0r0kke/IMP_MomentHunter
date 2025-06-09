using UnityEngine;

public class TrayController : MonoBehaviour
{
    [Header("Init Parent Object")]
    [SerializeField] private Transform defaultParent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cutlery"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cutlery"))
        {
            if (defaultParent != null)
                other.transform.SetParent(defaultParent);
            else
                other.transform.SetParent(null);
        }
    }
}
