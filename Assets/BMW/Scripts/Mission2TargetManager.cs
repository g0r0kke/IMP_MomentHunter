using UnityEngine;

public class Mission2TargetManager : MonoBehaviour
{
    [Header("CustomLayer")]
    public LayerMask customLayerMask;

    [Header("DefaultLayer")]
    public LayerMask defaultLayerMask;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cutlery"))
        {
            other.gameObject.layer = Mathf.RoundToInt(Mathf.Log(defaultLayerMask.value, 2));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cutlery"))
        {
            other.gameObject.layer = Mathf.RoundToInt(Mathf.Log(customLayerMask.value, 2));
        }
    }
}
