using UnityEngine;

/// <summary>
/// Manages mission target tagging based on current mission state.
/// Automatically applies or removes "MissionTarget" tag when the mission state changes.
/// </summary>
public class MissionTargetTag : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private MissionState targetMissionType = MissionState.None;
    
    /// <summary>
    /// Constant string for the mission target tag
    /// </summary>
    private const string MISSION_TARGET_TAG = "MissionTarget";
    
    /// <summary>
    /// Initialize the tag based on current mission state
    /// </summary>
    void Start()
    {
        // Check current mission state and set initial tag
        if (GameManager.Instance)
        {
            OnMissionStateChanged(GameManager.Instance.MissionState);
        }
    }
    
    /// <summary>
    /// Subscribe to mission state change events when enabled
    /// </summary>
    private void OnEnable()
    {
        // Subscribe to events
        GameManager.OnMissionStateChanged += OnMissionStateChanged;
    }
    
    /// <summary>
    /// Unsubscribe from mission state change events when disabled
    /// </summary>
    private void OnDisable()
    {
        // Unsubscribe from events
        GameManager.OnMissionStateChanged -= OnMissionStateChanged;
    }

    /// <summary>
    /// Handles mission state change events and updates tag accordingly
    /// </summary>
    /// <param name="newMissionState">The new mission state</param>
    private void OnMissionStateChanged(MissionState newMissionState)
    {
        if (targetMissionType == newMissionState)
        {
            // Apply MissionTarget tag if this target belongs to current mission
            SetMissionTargetTag(true);
        }
        else
        {
            // Change to Untagged if this target doesn't belong to current mission
            SetMissionTargetTag(false);
        }
    }
    
    /// <summary>
    /// Sets or removes the mission target tag
    /// </summary>
    /// <param name="isMissionTarget">True to apply MissionTarget tag, false to set as Untagged</param>
    private void SetMissionTargetTag(bool isMissionTarget)
    {
        if (isMissionTarget)
        {
            gameObject.tag = MISSION_TARGET_TAG;
            // Debug.Log($"{gameObject.name}: MissionTarget tag applied (Type: {targetMissionType})");
        }
        else
        {
            gameObject.tag = "Untagged";
            // Debug.Log($"{gameObject.name}: Changed to Untagged");
        }
    }
    
    /// <summary>
    /// Gets the configured mission type for this target
    /// </summary>
    /// <returns>The mission state this target belongs to</returns>
    public MissionState GetTargetMissionType()
    {
        return targetMissionType;
    }
}
