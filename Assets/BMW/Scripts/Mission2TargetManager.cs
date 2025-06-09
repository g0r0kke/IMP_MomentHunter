using UnityEngine;
using System.Collections.Generic;

public class Mission2TargetManager : MonoBehaviour
{
    [Header("Layer")]
    [SerializeField] private LayerMask customLayerMask;
    [SerializeField] private LayerMask defaultLayerMask;
    [Header("DeBug Log")]
    [SerializeField] private bool IsDebug = false;

    private int _cutleryCount = 0;
    private List<string> _enteredObjects = new List<string>();
    private Dictionary<string, int> _previousLayers = new Dictionary<string, int>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cutlery"))
        {
            string objName = other.gameObject.name;

            if (!_enteredObjects.Contains(objName))
            {
                _previousLayers[objName] = other.gameObject.layer;

                _enteredObjects.Add(objName);
                _cutleryCount++;

                int defaultLayerIndex = Mathf.RoundToInt(Mathf.Log(defaultLayerMask.value, 2));
                other.gameObject.layer = defaultLayerIndex;

                if (IsDebug)
                    Debug.LogWarning($"[Enter] Added: {objName} | Total: {_cutleryCount}");

                if (other.gameObject.layer != _previousLayers[objName] && IsDebug)
                {
                    Debug.LogWarning($"[Layer Changed] {objName}: " +
                        $"{LayerMask.LayerToName(_previousLayers[objName])} ¡æ " +
                        $"{LayerMask.LayerToName(other.gameObject.layer)}");
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
        if (other.CompareTag("Cutlery"))
        {
            string objName = other.gameObject.name;

            if (_enteredObjects.Contains(objName))
            {
                int prevLayer = other.gameObject.layer;

                _enteredObjects.Remove(objName);
                _cutleryCount = Mathf.Max(0, _cutleryCount - 1);

                if (IsDebug)
                    Debug.LogWarning($"[Exit] Removed: {objName} | Remaining: {_cutleryCount}");

                if (_cutleryCount <= 0)
                {
                    int customLayerIndex = Mathf.RoundToInt(Mathf.Log(customLayerMask.value, 2));
                    other.gameObject.layer = customLayerIndex;
                    _cutleryCount = 0;

                    if (other.gameObject.layer != prevLayer && IsDebug)
                    {
                        Debug.LogWarning($"[Layer Changed] {objName}: " +
                            $"{LayerMask.LayerToName(prevLayer)} ¡æ " +
                            $"{LayerMask.LayerToName(other.gameObject.layer)}");
                    }
                }

                _previousLayers.Remove(objName);
            }
            else if (IsDebug)
            {
                Debug.LogWarning($"[Exit Ignored] Not in list: {objName}");
            }
        }
    }
}
