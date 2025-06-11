using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlayObjectTargetManager : MonoBehaviour
{
    [Header("Layer")]
    [SerializeField] private LayerMask targetLayerMask;

    [Header("Tag")]
    [SerializeField] private string UnActiveTag = "Untagged";
    [SerializeField] private string ActiveTag = "MissionTarget";

    [Header("DeBug Log")]
    [SerializeField] private bool IsDebug = false;

    private int _OverlayObjectCount = 0;
    private List<string> _enteredObjects = new List<string>();
    private Dictionary<string, string> _previoustags = new Dictionary<string, string>();

    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            string objName = other.gameObject.name;
            transform.tag = UnActiveTag;

            if (!_enteredObjects.Contains(objName))
            {
                _previoustags[objName] = other.gameObject.tag;

                _enteredObjects.Add(objName);
                _OverlayObjectCount++;

                if (IsDebug)
                    Debug.LogWarning($"[Enter] Added: {objName} | Total: {_OverlayObjectCount}");

                if (_OverlayObjectCount == 1 && IsDebug)
                {
                    Debug.LogWarning($"[Layer Changed] {objName}: " +
                        $"{_previoustags[objName]} ¡æ " +
                        $"{transform.gameObject.tag}");
                }
            }
            else if (IsDebug)
            {
                Debug.LogWarning($"[Enter Ignored] Duplicate: {objName}");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((targetLayerMask.value & (1 << other.gameObject.layer)) != 0 && transform.tag == ActiveTag)
        {
            string objName = other.gameObject.name;
            transform.tag = UnActiveTag;

            if (!_enteredObjects.Contains(objName))
            {
                _previoustags[objName] = other.gameObject.tag;

                _enteredObjects.Add(objName);
                _OverlayObjectCount++;

                if (IsDebug)
                    Debug.LogWarning($"[Enter] Added: {objName} | Total: {_OverlayObjectCount}");

                if (_OverlayObjectCount == 1 && IsDebug)
                {
                    Debug.LogWarning($"[Layer Changed] {objName}: " +
                        $"{_previoustags[objName]} ¡æ " +
                        $"{transform.gameObject.tag}");
                }
            }
            else if (IsDebug)
            {
                Debug.LogWarning($"[Enter Ignored] Duplicate: {objName}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((targetLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            string objName = other.gameObject.name;

            if (_enteredObjects.Contains(objName))
            {
                string  prevtag = transform.tag;

                _enteredObjects.Remove(objName);
                _OverlayObjectCount = Mathf.Max(0, _OverlayObjectCount - 1);

                if (IsDebug)
                    Debug.LogWarning($"[Exit] Removed: {objName} | Remaining: {_OverlayObjectCount}");

                if (_OverlayObjectCount <= 0)
                {
                    _OverlayObjectCount = 0;
                    transform.gameObject.tag = ActiveTag;

                    if (transform.tag != prevtag && IsDebug)
                    {
                        Debug.LogWarning($"[Layer Changed] {objName}: " +
                            $"{prevtag} ¡æ " +
                            $"{transform.gameObject.tag}");
                    }
                }

                _previoustags.Remove(objName);
            }
            else if (IsDebug)
            {
                Debug.LogWarning($"[Exit Ignored] Not in list: {objName}");
            }
        }
    }
}
