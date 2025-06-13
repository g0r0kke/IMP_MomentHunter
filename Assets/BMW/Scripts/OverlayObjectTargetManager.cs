using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlayObjectTargetManager : MonoBehaviour
{
    // Layer CONFIGURATION
    [Header("Layer")]
    [SerializeField] private LayerMask targetLayerMask; // Only objects on this layer will trigger logic

    // Tag CONFIGURATION
    [Header("Tag")]
    [SerializeField] private string UnActiveTag = "Untagged";      // Tag to set when object is inactive
    [SerializeField] private string ActiveTag = "MissionTarget";   // Tag to set when object is active

    // Debug
    [Header("Debug Log")]
    [SerializeField] private bool IsDebug = false;                 // Enable/disable debug logging

    // Object Checker CONFIGURATION
    private int _OverlayObjectCount = 0;                           // Tracks number of objects currently inside trigger
    private List<string> _enteredObjects = new List<string>();     // List of object names currently inside trigger
    private Dictionary<string, string> _previoustags = new Dictionary<string, string>(); // Stores previous tags for each object

    // Called when another collider enters this object's trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object is on the target layer
        if ((targetLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            string objName = other.gameObject.name;
            transform.tag = UnActiveTag; // Set this object's tag to inactive

            // If this object hasn't already been registered
            if (!_enteredObjects.Contains(objName))
            {
                _previoustags[objName] = other.gameObject.tag; // Store the object's previous tag

                _enteredObjects.Add(objName); // Add to entered list
                _OverlayObjectCount++;        // Increment count

                if (IsDebug)
                    Debug.LogWarning($"[Enter] Added: {objName} | Total: {_OverlayObjectCount}");

                // If this is the first object in the trigger, log the tag change
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

    // Called once per frame for every collider that stays inside this trigger
    private void OnTriggerStay(Collider other)
    {
        // Only process if the object is on the target layer and this object is tagged as active
        if ((targetLayerMask.value & (1 << other.gameObject.layer)) != 0 && transform.tag == ActiveTag)
        {
            string objName = other.gameObject.name;
            transform.tag = UnActiveTag; // Set to inactive when a valid object is present

            // If this object hasn't already been registered
            if (!_enteredObjects.Contains(objName))
            {
                _previoustags[objName] = other.gameObject.tag; // Store previous tag

                _enteredObjects.Add(objName); // Add to entered list
                _OverlayObjectCount++;        // Increment count

                if (IsDebug)
                    Debug.LogWarning($"[Enter] Added: {objName} | Total: {_OverlayObjectCount}");

                // If this is the first object in the trigger, log the tag change
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

    // Called when another collider exits this object's trigger collider
    private void OnTriggerExit(Collider other)
    {
        // Check if the other object is on the target layer
        if ((targetLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            string objName = other.gameObject.name;

            // If this object was registered as entered
            if (_enteredObjects.Contains(objName))
            {
                string prevtag = transform.tag;

                _enteredObjects.Remove(objName);                 // Remove from entered list
                _OverlayObjectCount = Mathf.Max(0, _OverlayObjectCount - 1); // Decrement count, clamp to zero

                if (IsDebug)
                    Debug.LogWarning($"[Exit] Removed: {objName} | Remaining: {_OverlayObjectCount}");

                // If no objects remain in the trigger, set tag to active
                if (_OverlayObjectCount <= 0)
                {
                    _OverlayObjectCount = 0;
                    transform.gameObject.tag = ActiveTag;

                    // Log the tag change if it occurred
                    if (transform.tag != prevtag && IsDebug)
                    {
                        Debug.LogWarning($"[Layer Changed] {objName}: " +
                            $"{prevtag} ¡æ " +
                            $"{transform.gameObject.tag}");
                    }
                }

                _previoustags.Remove(objName); // Remove previous tag record
            }
            else if (IsDebug)
            {
                Debug.LogWarning($"[Exit Ignored] Not in list: {objName}");
            }
        }
    }
}