using UnityEngine;

/// <summary>
/// Manages audio playback including background music and sound effects.
/// Supports singleton pattern for persistent background music across scenes.
/// </summary>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Static instance for persistent background music manager across scenes
    /// </summary>
    public static AudioManager BGMInstance { get; private set; }
    
    [Header("Destroy Settings")]
    [SerializeField] private bool _dontDestroyOnLoad = false;
    
    [Header("Audio Components")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips;
    
    [Header("Auto Play Settings")]
    [SerializeField] private bool _autoPlayOnStart = false;
    [SerializeField] private int _backgroundMusicClipId = 0;
    [SerializeField] private bool _loopBackgroundMusic = true;
    
    /// <summary>
    /// Initialize the AudioManager instance and set up DontDestroyOnLoad if enabled
    /// </summary>
    void Awake()
    {
        if (_dontDestroyOnLoad)
        {
            // Check if an instance already exists
            if (BGMInstance)
            {
                // Destroy the new duplicate instance if one already exists
                Debug.Log("DontDestroyOnLoad AudioManager already exists. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            
            // Set as the first and only instance
            BGMInstance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AudioManager set to DontDestroyOnLoad and assigned as BGMInstance");
        }
        else
        {
            // Don't set BGMInstance if DontDestroyOnLoad is disabled
            Debug.Log("AudioManager created without DontDestroyOnLoad");
        }
    }
    
    /// <summary>
    /// Initialize audio components, subscribe to volume events, and start auto-play if enabled
    /// </summary>
    void Start()
    {
        // Get AudioSource component if not assigned in inspector
        if (!_audioSource) _audioSource = GetComponent<AudioSource>();
        
        // Subscribe to DataManager's volume change events
        DataManager.OnMasterVolumeChanged += OnMasterVolumeChanged;
        
        // Apply current master volume immediately
        ApplyMasterVolume();
        
        // Only play background music if this is the BGM instance and auto-play is enabled
        if (_autoPlayOnStart && BGMInstance == this)
        {
            PlayBackgroundMusic();
        }
    }
    
    /// <summary>
    /// Clean up event subscriptions and reset BGMInstance reference when destroyed
    /// </summary>
    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        DataManager.OnMasterVolumeChanged -= OnMasterVolumeChanged;
        
        // Reset BGMInstance to null if this was the active instance
        if (BGMInstance == this)
        {
            BGMInstance = null;
        }
    }
    
    /// <summary>
    /// Called when values are changed in the editor - validates settings and applies changes in real-time
    /// </summary>
    private void OnValidate()
    {
        // Clamp background music clip ID to valid range
        if (_audioClips != null && _audioClips.Length > 0)
        {
            _backgroundMusicClipId = Mathf.Clamp(_backgroundMusicClipId, 0, _audioClips.Length - 1);
        }
        
        // Only update during runtime
        if (Application.isPlaying)
        {
            // Apply master volume immediately
            ApplyMasterVolume();
            
            // Handle auto-play setting changes
            if (_autoPlayOnStart && !_audioSource.isPlaying)
            {
                PlayBackgroundMusic();
            }
            else if (!_autoPlayOnStart && _audioSource.isPlaying)
            {
                StopAudio();
            }
        }
    }
    
    /// <summary>
    /// Event handler for master volume changes from DataManager
    /// </summary>
    /// <param name="newVolume">The new volume value</param>
    private void OnMasterVolumeChanged(float newVolume)
    {
        ApplyMasterVolume();
    }
    
    /// <summary>
    /// Plays the designated background music clip with loop settings
    /// </summary>
    public void PlayBackgroundMusic()
    {
        // Validate background music clip ID
        if (!IsValidClipId(_backgroundMusicClipId))
        {
            Debug.LogWarning($"Invalid background music clip ID: {_backgroundMusicClipId}");
            return;
        }
        
        // Set up audio source with background music settings
        _audioSource.clip = _audioClips[_backgroundMusicClipId];
        _audioSource.loop = _loopBackgroundMusic;
        
        // Apply DataManager's master volume
        ApplyMasterVolume();
        
        _audioSource.Play();
        
        Debug.Log($"Background music started: {_audioClips[_backgroundMusicClipId].name}");
    }

    /// <summary>
    /// Plays a specific audio clip by ID (non-looping)
    /// </summary>
    /// <param name="clipId">Index of the audio clip to play</param>
    public void PlayAudio(int clipId)
    {
        // Safety check for valid clip ID
        if (!IsValidClipId(clipId))
        {
            Debug.LogWarning($"Invalid clip ID: {clipId}");
            return;
        }

        // Set up audio source for one-time playback
        _audioSource.clip = _audioClips[clipId];
        _audioSource.loop = false; // Regular audio clips don't loop
        
        // Apply DataManager's master volume
        ApplyMasterVolume();
        
        _audioSource.Play();
    }
    
    /// <summary>
    /// Plays an audio clip without changing the AudioSource's main clip
    /// Allows multiple sounds to play simultaneously without interrupting each other
    /// </summary>
    /// <param name="clipId">Index of the audio clip to play as one-shot</param>
    public void PlayOneShot(int clipId)
    {
        // Validate clip ID
        if (!IsValidClipId(clipId))
        {
            Debug.LogWarning($"Invalid clip ID: {clipId}");
            return;
        }
        
        // Get current master volume and play one-shot
        float volumeScale = GetMasterVolume();
        _audioSource.PlayOneShot(_audioClips[clipId], volumeScale);
    }
    
    /// <summary>
    /// Stops the currently playing audio
    /// </summary>
    public void StopAudio()
    {
        _audioSource.Stop();
        Debug.Log("Audio stopped");
    }
    
    /// <summary>
    /// Validates if the given clip ID is within bounds and references a valid AudioClip
    /// </summary>
    /// <param name="clipId">The clip ID to validate</param>
    /// <returns>True if the clip ID is valid, false otherwise</returns>
    private bool IsValidClipId(int clipId)
    {
        return _audioClips != null && clipId >= 0 && clipId < _audioClips.Length && _audioClips[clipId];
    }
    
    /// <summary>
    /// Retrieves the master volume from DataManager
    /// </summary>
    /// <returns>Master volume value, or 1.0 if DataManager is unavailable</returns>
    private float GetMasterVolume()
    {
        if (DataManager.Data)
        {
            return DataManager.Data.GetMasterVolume();
        }
        
        return 1f; // Return default value 1.0 if DataManager is not available
    }
    
    /// <summary>
    /// Applies the current master volume to the AudioSource
    /// </summary>
    private void ApplyMasterVolume()
    {
        if (_audioSource)
        {
            _audioSource.volume = GetMasterVolume();
        }
    }
}