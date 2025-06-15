using TMPro;
using UnityEngine;

/// <summary>
/// Manages mission-related UI displays including mission text, health display, and feedback objects.
/// Updates UI elements based on current mission state and player health.
/// </summary>
public class MissionText : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text _healthText;
    
    [Header("Mission UI Objects")]
    [SerializeField] private GameObject _missionBG;          // MissionBG object
    [SerializeField] private GameObject _pin;                // Pin object
    [SerializeField] private GameObject _missionTitle;       // MissionTitle object
    [SerializeField] private GameObject _missionContents;    // MissionContents object
    
    [Header("Mission GameObject Arrays")]
    [SerializeField] private GameObject[] _missionTitleObjects;    // Mission title game objects
    [SerializeField] private GameObject[] _missionContentObjects; // Mission content game objects
    
    [Header("Feedback GameObject Array")]
    [SerializeField] private GameObject[] _feedbackObjects;        // Feedback game objects
    
    /// <summary>
    /// Initialize mission UI and health display on start
    /// </summary>
    private void Start()
    {
        UpdateMissionText();
        
        if (!DataManager.Data) return;
        UpdateHealthText(DataManager.Data.CurrentHealth);
    }
    
    /// <summary>
    /// Activates/deactivates mission objects by index
    /// </summary>
    /// <param name="index">Index of the mission objects to activate</param>
    public void ActivateMissionObjects(int index)
    {
        // Deactivate all mission title objects
        if (_missionTitleObjects != null)
        {
            for (int i = 0; i < _missionTitleObjects.Length; i++)
            {
                if (_missionTitleObjects[i])
                {
                    _missionTitleObjects[i].SetActive(i == index);
                }
            }
        }
        
        // Deactivate all mission content objects
        if (_missionContentObjects != null)
        {
            for (int i = 0; i < _missionContentObjects.Length; i++)
            {
                if (_missionContentObjects[i])
                {
                    _missionContentObjects[i].SetActive(i == index);
                }
            }
        }
    }
    
    /// <summary>
    /// Activates/deactivates feedback objects by index
    /// </summary>
    /// <param name="index">Index of the feedback object to activate</param>
    public void ActivateFeedbackObject(int index)
    {
        if (_feedbackObjects == null) return;
        
        // Deactivate all feedback objects then activate only the specified index
        for (int i = 0; i < _feedbackObjects.Length; i++)
        {
            if (_feedbackObjects[i])
            {
                _feedbackObjects[i].SetActive(i == index);
            }
        }
    }
    
    /// <summary>
    /// Updates mission text display based on current mission state
    /// </summary>
    public void UpdateMissionText()
    {
        if (!GameManager.Instance) return;
        
        MissionState currentState = GameManager.Instance.MissionState;
        
        // Hide mission UI during None and Ending states
        if (currentState == MissionState.None || currentState == MissionState.Ending)
        {
            SetMissionUIActive(false);
        }
        else
        {
            SetMissionUIActive(true);
            int objectIndex = GetObjectIndex(currentState);
            ActivateMissionObjects(objectIndex);
        }
    }

    /// <summary>
    /// Sets the entire mission UI active or inactive
    /// </summary>
    /// <param name="isActive">Whether to activate or deactivate mission UI</param>
    private void SetMissionUIActive(bool isActive)
    {
        if (_missionBG) _missionBG.SetActive(isActive);
        if (_pin) _pin.SetActive(isActive);
        if (_missionTitle) _missionTitle.SetActive(isActive);
        if (_missionContents) _missionContents.SetActive(isActive);
    }
    
    /// <summary>
    /// Updates the health text display
    /// </summary>
    /// <param name="health">Current health value (parameter is not used, gets value from DataManager)</param>
    public void UpdateHealthText(int health)
    {
        if (!DataManager.Data) return;

        _healthText.text = DataManager.Data.CurrentHealth + " / "  + DataManager.Data.MaxHealth;
    }
    
    /// <summary>
    /// Converts mission state to corresponding object index
    /// </summary>
    /// <param name="missionState">The mission state to convert</param>
    /// <returns>Index corresponding to the mission state</returns>
    private int GetObjectIndex(MissionState missionState)
    {
        return missionState switch
        {
            MissionState.Mission1 => 0,    // First game object
            MissionState.Mission2 => 1,
            MissionState.Mission3 => 2,
            MissionState.Mission4 => 3,
            MissionState.Mission5 => 4,
            MissionState.Mission6 => 5,    // Sixth game object
            _ => 0 // Default value
        };
    }
}