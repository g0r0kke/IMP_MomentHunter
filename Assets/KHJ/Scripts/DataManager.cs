using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages persistent game data including player health, volume settings, and UI updates.
/// Implements singleton pattern to maintain data across scene transitions.
/// </summary>
public class DataManager : MonoBehaviour
{
    /// <summary>
    /// Static instance for accessing DataManager from anywhere in the game
    /// </summary>
    public static DataManager Data { get; private set; }
    
    [Header("Player Information")]
    private const int MAX_HEALTH = 8;
    /// <summary>
    /// Maximum health value for the player
    /// </summary>
    public int MaxHealth => MAX_HEALTH;
    [SerializeField] private int _currentHealth = MAX_HEALTH;
    /// <summary>
    /// Current player health value
    /// </summary>
    public int CurrentHealth => _currentHealth;

    [Header("Game Settings")]
    [SerializeField] [Range(0, 100)] private int _masterVolumeLevel = 80; // Volume range changed to 0~100
    /// <summary>
    /// Master volume as float (0-1) for audio systems
    /// </summary>
    private float _masterVolume => _masterVolumeLevel / 100f; // Convert to 0~1 range

    [Header("UI Components")]
    [SerializeField] private MissionText _missionText;

    /// <summary>
    /// Event triggered when master volume changes, passes the new volume as float (0-1)
    /// </summary>
    public static event Action<float> OnMasterVolumeChanged;
    
    /// <summary>
    /// Initialize singleton instance and set up scene loading events
    /// </summary>
    private void Awake()
    {
        // Implement singleton pattern
        if (!Data)
        {
            Data = this;
            DontDestroyOnLoad(gameObject);
            
            // Subscribe to scene load events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
    }
 
    /// <summary>
    /// Initialize UI components and update displays
    /// </summary>
    private void Start()
    {
        // Only execute if this is the active Data instance
        if (Data != this) return;
        
        FindMissionText();
        UpdateHealthUI();
    }
    
    /// <summary>
    /// Called when a new scene is loaded - refreshes UI references and broadcasts current volume
    /// </summary>
    /// <param name="scene">The loaded scene</param>
    /// <param name="mode">The scene load mode</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Refresh UI component references for the new scene
        FindMissionText();
        UpdateHealthUI();
        
        // Broadcast current volume to all AudioManagers in the new scene
        OnMasterVolumeChanged?.Invoke(_masterVolume);
    }
    
    /// <summary>
    /// Finds and caches the MissionText component in the current scene
    /// </summary>
    private void FindMissionText()
    {
        _missionText = FindFirstObjectByType<MissionText>();
        
        if (!_missionText)
        {
            Debug.LogWarning($"DataManager: MissionText component not found in scene '{SceneManager.GetActiveScene().name}'.");
        }
    }

    /// <summary>
    /// Called when values are changed in the editor - validates ranges and updates UI in real-time
    /// </summary>
    private void OnValidate()
    {
        int previousVolumeLevel = _masterVolumeLevel;
        
        // Clamp health to valid range
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MAX_HEALTH);

        // Clamp volume to valid range (0~100)
        _masterVolumeLevel = Mathf.Clamp(_masterVolumeLevel, 0, 100);
        
        // Only update UI during runtime
        if (Application.isPlaying)
        {
            UpdateHealthUI();
            
            // Trigger volume change event if volume was modified
            if (previousVolumeLevel != _masterVolumeLevel)
            {
                OnMasterVolumeChanged?.Invoke(_masterVolume);
                Debug.Log($"Master volume set to {_masterVolumeLevel}%. (Float: {_masterVolume:F2})");
            }
        }
    }

    /// <summary>
    /// Updates the health display in the UI
    /// </summary>
    private void UpdateHealthUI()
    {
        // Find and update MissionText component with current health
        if (_missionText)
        {
            _missionText.UpdateHealthText(_currentHealth);
        }
    }
    
    /// <summary>
    /// Clean up event subscriptions when the object is destroyed
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (Data == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            OnMasterVolumeChanged = null; // Clear all event subscribers
        }
    }

    /// <summary>
    /// Decreases player health by 1. Triggers game over if health reaches 0.
    /// </summary>
    public void UseHealth()
    {
        if (_currentHealth > 0)
        {
            --_currentHealth;
            UpdateHealthUI();
        }
        else
        {
            // Trigger defeat state when health is depleted
            if (GameManager.Instance) GameManager.Instance.SetGameState(GameState.Defeat);
        }
    }
    
    /// <summary>
    /// Sets the master volume level and notifies all audio systems
    /// </summary>
    /// <param name="volumeLevel">Volume level as integer (0-100)</param>
    public void SetMasterVolume(int volumeLevel)
    {
        int newVolumeLevel = Mathf.Clamp(volumeLevel, 0, 100);

        // Only trigger event if volume actually changed
        if (_masterVolumeLevel != newVolumeLevel)
        {
            _masterVolumeLevel = newVolumeLevel;

            // Notify all AudioManagers of volume change (convert to 0~1 range)
            OnMasterVolumeChanged?.Invoke(_masterVolume);

            Debug.Log($"Master volume set to {_masterVolumeLevel}%. (Float: {_masterVolume:F2})");
        }
    }

    /// <summary>
    /// Gets the current master volume as float (0-1)
    /// </summary>
    /// <returns>Master volume as float between 0 and 1</returns>
    public float GetMasterVolume()
    {
        return _masterVolume;
    }
    
    /// <summary>
    /// Gets the current master volume level as integer (0-100)
    /// </summary>
    /// <returns>Master volume level as integer between 0 and 100</returns>
    public int GetMasterVolumeLevel()
    {
        return _masterVolumeLevel;
    }

    /// <summary>
    /// Resets player health to maximum value
    /// </summary>
    public void InitializeHealth()
    {
        _currentHealth = MAX_HEALTH;
        UpdateHealthUI(); // Update UI after resetting health
    }
}