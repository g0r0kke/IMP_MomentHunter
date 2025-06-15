using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Debug utility for monitoring and logging mission target tag changes.
/// Tracks all MissionTargetTag components in the scene and logs their state changes.
/// </summary>
public class TagDebugManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    /// <summary>
    /// List of all MissionTargetTag components found in the scene
    /// </summary>
    private List<MissionTargetTag> allTargets = new List<MissionTargetTag>();
    
    /// <summary>
    /// Collect all MissionTargetTag components in the scene on start
    /// </summary>
    private void Start()
    {
        // Collect all MissionTargetTag components in scene (including inactive objects)
        allTargets.AddRange(FindObjectsByType<MissionTargetTag>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        
        if (showDebugInfo)
        {
            Debug.Log($"Found {allTargets.Count} MissionTarget objects in scene");
            // foreach (var target in allTargets)
            // {
            //     if (target)
            //     {
            //         Debug.Log($"- {target.gameObject.name}: {target.GetTargetMissionType()} (활성: {target.gameObject.activeInHierarchy})");
            //     }
            // }
        }
    }
    
    /// <summary>
    /// Subscribe to mission state change events when enabled
    /// </summary>
    private void OnEnable()
    {
        GameManager.OnMissionStateChanged += OnMissionStateChanged;
    }
    
    /// <summary>
    /// Unsubscribe from mission state change events when disabled
    /// </summary>
    private void OnDisable()
    {
        GameManager.OnMissionStateChanged -= OnMissionStateChanged;
    }
    
    /// <summary>
    /// Handles mission state change events and logs tag changes for debugging
    /// </summary>
    /// <param name="newState">The new mission state</param>
    private void OnMissionStateChanged(MissionState newState)
    {
        if (showDebugInfo)
        {
            Debug.Log($"=== Mission State Changed: {newState} ===");
            
            List<string> missionTargetNames = new List<string>();
            List<string> untaggedNames = new List<string>();
            
            foreach (var target in allTargets)
            {
                if (target && target.gameObject.activeInHierarchy)
                {
                    if (target.GetTargetMissionType() == newState)
                    {
                        // Targets that belong to current mission (will get MissionTarget tag)
                        missionTargetNames.Add(target.gameObject.name);
                    }
                    else
                    {
                        // Targets that don't belong to current mission (will become Untagged)
                        untaggedNames.Add(target.gameObject.name);
                    }
                }
            }
            
            // Log tag assignments
            if (missionTargetNames.Count > 0)
            {
                Debug.Log($"MissionTarget tag applied to: {string.Join(", ", missionTargetNames)}");
            }
            
            if (untaggedNames.Count > 0)
            {
                Debug.Log($"Changed to Untagged: {string.Join(", ", untaggedNames)}");
            }
            
            Debug.Log($"Current active MissionTarget count: {missionTargetNames.Count}");
        }
    }
}
