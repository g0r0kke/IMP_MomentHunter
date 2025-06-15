using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

/// <summary>
/// Manages VR UI interactions for the opening scene including main menu, settings, and quit functionality.
/// Handles VR input through XR controllers and manages popup states.
/// </summary>
public class OpeningUIManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private Text _debugText; // Debug information display for VR

    [Header("UI Components")]
    [SerializeField] private GameObject _playUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _quitUI;
    [SerializeField] private Canvas _openingCanvas;
    [SerializeField] private Canvas _prologueCanvas;
    
    [Header("Popup UI Components")]
    [SerializeField] private GameObject _settingsPopupUI;  // Settings popup UI
    [SerializeField] private GameObject _quitPopupUI;      // Quit popup UI
    
    // Internal components for settings and quit UI
    [Header("Settings UI Components")]
    [SerializeField] private Slider _soundSlider;         // Sound volume slider
    [SerializeField] private Text _soundValueText;       // Sound value display text (optional)
    
    [Header("Quit UI Components")]
    [SerializeField] private Button _mainBackButton;      // Return to main button
    
    [SerializeField] private TrackedDeviceGraphicRaycaster _trackedRaycaster;
    [SerializeField] private EventSystem _eventSystem;

    [Header("VR Components")]
    [SerializeField] private XRRayInteractor _leftRayInteractor;  // Left controller ray interactor
    [SerializeField] private XRRayInteractor _rightRayInteractor; // Right controller ray interactor
    [SerializeField] private InputActionAsset _inputActions;      // VR Input Actions

    /// <summary>
    /// Canvas state management (true: prologue, false: opening)
    /// </summary>
    private bool _isPrologueActive = false;
    
    /// <summary>
    /// UI popup state management
    /// </summary>
    private bool _isSettingsPopupActive = false;
    private bool _isQuitPopupActive = false;
    
    /// <summary>
    /// Input prevention variables
    /// </summary>
    private bool _isProcessingInput = false;
    private bool _isQuittingGame = false;  // Game quit flag
    private float _inputCooldown = 0.5f; // 0.5 second cooldown
    private float _lastInputTime = 0f;
    
    /// <summary>
    /// VR Input Actions
    /// </summary>
    private InputAction _aButton;
    private InputAction _yButton;
    private InputAction _leftTrigger;
    private InputAction _rightTrigger;
    
    /// <summary>
    /// Currently hovered UI element by ray
    /// </summary>
    private GameObject _currentHoveredUI = null;
    
    /// <summary>
    /// Flag to check if slider event listener is registered
    /// </summary>
    private bool _sliderListenerRegistered = false;
    
    /// <summary>
    /// Initialize UI state and VR input systems
    /// </summary>
    private void Start()
    {
        SetPrologueActive(_isPrologueActive);
        InitializePopupUI();
        SetupVRInputActions();
        SetupSliderEvents(); // Add slider event setup
    }
    
    /// <summary>
    /// Initialize popup UIs to inactive state
    /// </summary>
    private void InitializePopupUI()
    {
        // Initially deactivate popup UIs
        if (_settingsPopupUI)
            _settingsPopupUI.SetActive(false);
        if (_quitPopupUI)
            _quitPopupUI.SetActive(false);
    }
    
    /// <summary>
    /// Set up slider event listeners and configuration
    /// </summary>
    private void SetupSliderEvents()
    {
        if (_soundSlider && !_sliderListenerRegistered)
        {
            // Use Unity's default slider event system
            _soundSlider.onValueChanged.AddListener(OnSoundSliderValueChanged);
            _sliderListenerRegistered = true;
            
            // Configure slider (0~1 range, continuous values)
            _soundSlider.minValue = 0f;
            _soundSlider.maxValue = 1f;
            _soundSlider.wholeNumbers = false; // Use continuous values for smooth operation
            
            Debug.Log("Sound slider events setup completed");
        }
    }
    
    /// <summary>
    /// Called whenever slider value changes (including during drag)
    /// </summary>
    /// <param name="value">New slider value (0-1)</param>
    private void OnSoundSliderValueChanged(float value)
    {
        // Called whenever slider value changes (real-time during drag)
        int volumeLevel = Mathf.RoundToInt(value * 100f);
        
        // Set volume in DataManager
        if (DataManager.Data)
        {
            DataManager.Data.SetMasterVolume(volumeLevel);
        }
        
        // Optional: Display volume value as text
        if (_soundValueText)
        {
            _soundValueText.text = volumeLevel.ToString() + "%";
        }
        
        Debug.Log($"Sound volume updated to: {volumeLevel}% (Slider value: {value:F2})");
    }
    
    /// <summary>
    /// Set up VR input action bindings for controllers
    /// </summary>
    private void SetupVRInputActions()
    {
        if (!_inputActions)
        {
            Debug.LogError("InputActionAsset not found!");
            return;
        }

        // Right controller A button setup (from XRI Right Action Map)
        var rightActionMap = _inputActions?.FindActionMap("XRI Right");
        if (rightActionMap != null)
        {
            _aButton = rightActionMap.FindAction("AButton");
            if (_aButton != null)
            {
                _aButton.Enable();
                _aButton.performed += OnAButtonPressed;
            }
            else
            {
                Debug.LogError("AButton action not found!");
            }
        }
        else
        {
            Debug.LogError("XRI Right action map not found!");
        }

        // Left controller Y button setup (from XRI Left Action Map)
        var leftActionMap = _inputActions?.FindActionMap("XRI Left");
        if (leftActionMap != null)
        {
            _yButton = leftActionMap.FindAction("YButton");
            if (_yButton != null)
            {
                _yButton.Enable();
                _yButton.performed += OnYButtonPressed;
            }
            else
            {
                Debug.LogError("YButton action not found!");
            }
        }
        else
        {
            Debug.LogError("XRI Left action map not found!");
        }

        // Right trigger setup (from XRI Right Interaction Action Map)
        var rightInteractionMap = _inputActions?.FindActionMap("XRI Right Interaction");
        if (rightInteractionMap != null)
        {
            _rightTrigger = rightInteractionMap.FindAction("Activate");
            if (_rightTrigger != null)
            {
                _rightTrigger.Enable();
                _rightTrigger.performed += OnRightTriggerPressed;
            }
            else
            {
                Debug.LogError("Right Activate action not found in XRI Right Interaction!");
            }
        }
        else
        {
            Debug.LogError("XRI Right Interaction action map not found!");
        }

        // Left trigger setup (from XRI Left Interaction Action Map)
        var leftInteractionMap = _inputActions?.FindActionMap("XRI Left Interaction");
        if (leftInteractionMap != null)
        {
            _leftTrigger = leftInteractionMap.FindAction("Activate");
            if (_leftTrigger != null)
            {
                _leftTrigger.Enable();
                _leftTrigger.performed += OnLeftTriggerPressed;
            }
            else
            {
                Debug.LogError("Left Activate action not found in XRI Left Interaction!");
            }
        }
        else
        {
            Debug.LogError("XRI Left Interaction action map not found!");
        }
    }
    
    /// <summary>
    /// Update loop for VR UI hover detection
    /// </summary>
    private void Update()
    {
        // Check UI hover with raycast in VR environment
        if (!_isPrologueActive)
        {
            CheckVRUIHover();
        }
    }
    
    /// <summary>
    /// Check which UI element is currently being hovered by VR rays
    /// </summary>
    void CheckVRUIHover()
    {
        if (_isPrologueActive) 
        {
            UpdateDebugText("Prologue is active");
            return;
        }
        
        GameObject hitUI = null;
        string raySource = "";
        
        // Check left ray first
        if (_leftRayInteractor && _leftRayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult leftRaycastResult))
        {
            hitUI = leftRaycastResult.gameObject;
            raySource = "Left Ray";
        }
        // If not on left, check right ray
        else if (_rightRayInteractor && _rightRayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult rightRaycastResult))
        {
            hitUI = rightRaycastResult.gameObject;
            raySource = "Right Ray";
        }
        
        if (hitUI)
        {
            UpdateDebugText($"{raySource} hit: {hitUI.name}");
            
            // Update currently hovered UI
            if (IsTargetUI(hitUI))
            {
                _currentHoveredUI = hitUI;
                UpdateDebugText($"{raySource} - Target UI: {hitUI.name}");
            }
            else
            {
                _currentHoveredUI = null;
                UpdateDebugText($"{raySource} - Non-target: {hitUI.name}");
            }
        }
        else
        {
            _currentHoveredUI = null;
            UpdateDebugText("No UI hit");
        }
    }
    
    /// <summary>
    /// Update debug text display
    /// </summary>
    /// <param name="message">Debug message to display</param>
    private void UpdateDebugText(string message)
    {
        if (_debugText)
        {
            _debugText.text = message;
        }
    }
    
    /// <summary>
    /// Helper method to get hit UI from specific ray interactor
    /// </summary>
    /// <param name="rayInteractor">Ray interactor to check</param>
    /// <returns>Hit UI GameObject or null</returns>
    private GameObject GetRayHitUI(XRRayInteractor rayInteractor)
    {
        if (rayInteractor && rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
        {
            return raycastResult.gameObject;
        }
        return null;
    }
    
    /// <summary>
    /// Update slider value based on right ray position when A button/trigger is pressed
    /// </summary>
    /// <returns>True if slider was successfully updated</returns>
    private bool UpdateSliderFromRaycast()
    {
        // Get hit point from right ray
        if (_rightRayInteractor && _rightRayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
        {
            if (!_soundSlider) 
            {
                Debug.LogWarning("Sound slider is null");
                return false;
            }
            
            // Get slider's RectTransform
            RectTransform sliderRect = _soundSlider.GetComponent<RectTransform>();
            if (!sliderRect) 
            {
                Debug.LogWarning("Slider RectTransform is null");
                return false;
            }
            
            // Convert world point to slider's local point
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                sliderRect, raycastResult.screenPosition, raycastResult.module.eventCamera, out localPoint))
            {
                // Convert from slider's local coordinates to 0~1 value
                Rect rect = sliderRect.rect;
                float normalizedValue = Mathf.Clamp01((localPoint.x - rect.xMin) / rect.width);
                
                // Set slider value (OnSoundSliderValueChanged will be called automatically)
                _soundSlider.value = normalizedValue;
                
                Debug.Log($"Slider updated from button press: {normalizedValue:F2} (Volume level: {Mathf.RoundToInt(normalizedValue * 100f)}%)");
                return true;
            }
            else
            {
                Debug.LogWarning("Failed to convert screen point to local point");
            }
        }
        else
        {
            Debug.LogWarning("No raycast hit from right ray interactor");
        }
        
        return false;
    }
    
    /// <summary>
    /// Check if the hit object is a valid target UI based on current state
    /// </summary>
    /// <param name="hitObject">GameObject to check</param>
    /// <returns>True if object is a valid target UI</returns>
    private bool IsTargetUI(GameObject hitObject)
    {
        if (!hitObject) return false;
    
        // When popup is open, only allow that popup's UI
        if (_isSettingsPopupActive)
        {
            // In settings popup, allow all slider-related UI elements
            bool isSettingsSlider = _soundSlider && 
                                    (hitObject == _soundSlider.gameObject || 
                                     hitObject.transform.IsChildOf(_soundSlider.transform) ||
                                     hitObject.GetComponent<Slider>() == _soundSlider);
        
            return isSettingsSlider;
        }
    
        if (_isQuitPopupActive)
        {
            // In quit popup, only allow main back button
            bool isMainBackButton = _mainBackButton && 
                                    (hitObject == _mainBackButton.gameObject || 
                                     hitObject.transform.IsChildOf(_mainBackButton.transform));
        
            return isMainBackButton;
        }
    
        // When no popup is open, only allow main menu UI
        bool isMainMenuUI = (hitObject == _playUI || hitObject.transform.IsChildOf(_playUI.transform)) ||
                            (hitObject == _settingsUI || hitObject.transform.IsChildOf(_settingsUI.transform)) ||
                            (hitObject == _quitUI || hitObject.transform.IsChildOf(_quitUI.transform));
    
        return isMainMenuUI;
    }
    
    /// <summary>
    /// Handle A button press events
    /// </summary>
    /// <param name="context">Input action callback context</param>
    private void OnAButtonPressed(InputAction.CallbackContext context)
    {
        if (_isProcessingInput || Time.time - _lastInputTime < _inputCooldown)
            return;
            
        _isProcessingInput = true;
        _lastInputTime = Time.time;
        
        if (_isPrologueActive)
        {
            // In prologue, A button advances to next scene (only A button works in prologue)
            GameManager.Instance.TransitionToScene(1);
        }
        else
        {
            // A button only checks right ray
            GameObject rightHitUI = GetRayHitUI(_rightRayInteractor);
            if (rightHitUI && IsTargetUI(rightHitUI))
            {
                HandleUIClick(rightHitUI);
            }
        }
        
        _isProcessingInput = false;
    }
    
    /// <summary>
    /// Handle Y button press events (popup closing)
    /// </summary>
    /// <param name="context">Input action callback context</param>
    private void OnYButtonPressed(InputAction.CallbackContext context)
    {
        if (_isProcessingInput || Time.time - _lastInputTime < _inputCooldown)
            return;
            
        _isProcessingInput = true;
        _lastInputTime = Time.time;
        
        // Y button closes popups
        if (_isSettingsPopupActive)
        {
            CloseSettingsPopup();
        }
        else if (_isQuitPopupActive)
        {
            CloseQuitPopup();
        }
        
        _isProcessingInput = false;
    }
    
    /// <summary>
    /// Handle left trigger press events
    /// </summary>
    /// <param name="context">Input action callback context</param>
    private void OnLeftTriggerPressed(InputAction.CallbackContext context)
    {
        if (_isProcessingInput || Time.time - _lastInputTime < _inputCooldown)
            return;
            
        _isProcessingInput = true;
        _lastInputTime = Time.time;
        
        // Left trigger only checks left ray (doesn't work in prologue)
        if (!_isPrologueActive)
        {
            GameObject leftHitUI = GetRayHitUI(_leftRayInteractor);
            if (leftHitUI && IsTargetUI(leftHitUI))
            {
                HandleUIClick(leftHitUI);
            }
        }
        
        _isProcessingInput = false;
    }
    
    /// <summary>
    /// Handle right trigger press events
    /// </summary>
    /// <param name="context">Input action callback context</param>
    private void OnRightTriggerPressed(InputAction.CallbackContext context)
    {
        if (_isProcessingInput || Time.time - _lastInputTime < _inputCooldown)
            return;
            
        _isProcessingInput = true;
        _lastInputTime = Time.time;
        
        // Right trigger only checks right ray (doesn't work in prologue)
        if (!_isPrologueActive)
        {
            GameObject rightHitUI = GetRayHitUI(_rightRayInteractor);
            if (rightHitUI && IsTargetUI(rightHitUI))
            {
                HandleUIClick(rightHitUI);
            }
        }
        
        _isProcessingInput = false;
    }
    
    /// <summary>
    /// Route UI click events to appropriate handlers based on current state
    /// </summary>
    /// <param name="clickedUI">The clicked UI GameObject</param>
    private void HandleUIClick(GameObject clickedUI)
    {
        Debug.Log($"HandleUIClick called with: {clickedUI.name}");
        
        // When popup is open, only handle that popup's UI
        if (_isSettingsPopupActive)
        {
            HandleSettingsPopupClick(clickedUI);
            return;
        }
        
        if (_isQuitPopupActive)
        {
            HandleQuitPopupClick(clickedUI);
            return;
        }
        
        // Handle main menu UI (only when no popup is open)
        if (clickedUI == _playUI || clickedUI.transform.IsChildOf(_playUI.transform))
        {
            OnPlayUIClicked();
        }
        else if (clickedUI == _settingsUI || clickedUI.transform.IsChildOf(_settingsUI.transform))
        {
            OnSettingsUIClicked();
        }
        else if (clickedUI == _quitUI || clickedUI.transform.IsChildOf(_quitUI.transform))
        {
            OnQuitUIClicked();
        }
        else
        {
            Debug.Log($"No matching UI handler found for: {clickedUI.name}");
        }
    }
    
    /// <summary>
    /// Handle clicks within the settings popup
    /// </summary>
    /// <param name="clickedUI">The clicked UI GameObject</param>
    private void HandleSettingsPopupClick(GameObject clickedUI)
    {
        // Allow A button/trigger interaction with slider
        if (_soundSlider && (clickedUI == _soundSlider.gameObject || clickedUI.transform.IsChildOf(_soundSlider.transform)))
        {
            Debug.Log("Sound slider button interaction");
            // Set slider value based on right ray position with A button/trigger
            UpdateSliderFromRaycast();
        }
    }
    
    /// <summary>
    /// Handle clicks within the quit popup
    /// </summary>
    /// <param name="clickedUI">The clicked UI GameObject</param>
    private void HandleQuitPopupClick(GameObject clickedUI)
    {
        // Only main back button (game quit)
        if (_mainBackButton && (clickedUI == _mainBackButton.gameObject || clickedUI.transform.IsChildOf(_mainBackButton.transform)))
        {
            Debug.Log("Main back button detected - calling OnMainBackButtonClicked");
            OnMainBackButtonClicked();
        }
    }

    /// <summary>
    /// Handle play UI button click - switches to prologue canvas
    /// </summary>
    private void OnPlayUIClicked()
    {
        SetPrologueActive(true); // Switch to prologue canvas
    }
    
    /// <summary>
    /// Handle settings UI button click - opens settings popup
    /// </summary>
    private void OnSettingsUIClicked()
    {
        Debug.Log("Settings clicked");
        OpenSettingsPopup();
    }
    
    /// <summary>
    /// Handle quit UI button click - opens quit popup
    /// </summary>
    private void OnQuitUIClicked()
    {
        Debug.Log("Quit clicked");
        OpenQuitPopup();
    }
    
    /// <summary>
    /// Open the settings popup and initialize slider value
    /// </summary>
    private void OpenSettingsPopup()
    {
        if (_settingsPopupUI)
        {
            _settingsPopupUI.SetActive(true);
            _isSettingsPopupActive = true;
            
            // Reflect current volume in slider when opening settings popup
            UpdateSoundSliderValue();
            
            Debug.Log("Settings popup opened");
        }
    }
    
    /// <summary>
    /// Update slider value to match current DataManager volume setting
    /// </summary>
    private void UpdateSoundSliderValue()
    {
        if (_soundSlider && DataManager.Data)
        {
            // Set DataManager's master volume level (0~100) to slider (0~1)
            float sliderValue = DataManager.Data.GetMasterVolumeLevel() / 100f;
            _soundSlider.value = sliderValue;
            Debug.Log($"Sound slider value updated to: {_soundSlider.value} (Volume Level: {DataManager.Data.GetMasterVolumeLevel()}%)");
            
            // Update text as well
            if (_soundValueText)
            {
                _soundValueText.text = DataManager.Data.GetMasterVolumeLevel().ToString() + "%";
            }
        }
    }
    
    /// <summary>
    /// Close the settings popup
    /// </summary>
    private void CloseSettingsPopup()
    {
        if (_settingsPopupUI)
        {
            _settingsPopupUI.SetActive(false);
            _isSettingsPopupActive = false;
            Debug.Log("Settings popup closed");
        }
    }
    
    /// <summary>
    /// Open the quit confirmation popup
    /// </summary>
    private void OpenQuitPopup()
    {
        if (_quitPopupUI)
        {
            _quitPopupUI.SetActive(true);
            _isQuitPopupActive = true;
            Debug.Log("Quit popup opened");
        }
    }
    
    /// <summary>
    /// Close the quit confirmation popup
    /// </summary>
    private void CloseQuitPopup()
    {
        if (_quitPopupUI)
        {
            _quitPopupUI.SetActive(false);
            _isQuitPopupActive = false;
            Debug.Log("Quit popup closed");
        }
    }
    
    /// <summary>
    /// Handle main back button click - initiates game quit sequence
    /// </summary>
    private void OnMainBackButtonClicked()
    {
        Debug.Log("=== OnMainBackButtonClicked START ===");
        
        // Check if already quitting game
        if (_isQuittingGame)
        {
            Debug.Log("Already quitting game, ignoring click");
            return;
        }
            
        Debug.Log("Main back button clicked - Quitting game");
        
        // Set game quit flag (ensure it only executes once)
        _isQuittingGame = true;
        
        // Disable Input Actions (before game quit)
        DisableAllInputActions();
        
        Debug.Log("Attempting to quit game...");
        
        // Quit game after short delay (give Input System time to clean up)
        StartCoroutine(QuitGameCoroutine());
        
        Debug.Log("=== OnMainBackButtonClicked END ===");
    }
    
    /// <summary>
    /// Coroutine to handle game quit with delay
    /// </summary>
    /// <returns>Coroutine enumerator</returns>
    private System.Collections.IEnumerator QuitGameCoroutine()
    {
        // Wait 0.1 seconds (Input System cleanup time)
        yield return new WaitForSeconds(0.1f);
        
        // Quit game
        #if UNITY_EDITOR
            Debug.Log("Stopping play mode in editor");
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Debug.Log("Calling Application.Quit()");
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Disable all input actions and unsubscribe from events
    /// </summary>
    private void DisableAllInputActions()
    {
        Debug.Log("Disabling all input actions...");
        
        if (_aButton != null)
        {
            _aButton.performed -= OnAButtonPressed;
            _aButton.Disable();
        }
        if (_yButton != null)
        {
            _yButton.performed -= OnYButtonPressed;
            _yButton.Disable();
        }
        if (_leftTrigger != null)
        {
            _leftTrigger.performed -= OnLeftTriggerPressed;
            _leftTrigger.Disable();
        }
        if (_rightTrigger != null)
        {
            _rightTrigger.performed -= OnRightTriggerPressed;
            _rightTrigger.Disable();
        }
        
        Debug.Log("All input actions disabled");
    }
    
    /// <summary>
    /// Set canvas state between opening and prologue
    /// </summary>
    /// <param name="isPrologueActive">True for prologue canvas, false for opening canvas</param>
    public void SetPrologueActive(bool isPrologueActive)
    {
        _isPrologueActive = isPrologueActive;
        _openingCanvas.enabled = !_isPrologueActive;
        _prologueCanvas.enabled = _isPrologueActive;
    }
    
    /// <summary>
    /// Clean up event listeners and input actions when destroyed
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe slider event listener
        if (_soundSlider && _sliderListenerRegistered)
        {
            _soundSlider.onValueChanged.RemoveListener(OnSoundSliderValueChanged);
        }
        
        // Prevent duplicate unsubscription since DisableAllInputActions already handled it
        if (!_isQuittingGame)
        {
            if (_aButton != null) 
            {
                _aButton.performed -= OnAButtonPressed;
            }
            if (_yButton != null)
            {
                _yButton.performed -= OnYButtonPressed;
            }
            if (_leftTrigger != null)
            {
                _leftTrigger.performed -= OnLeftTriggerPressed;
            }
            if (_rightTrigger != null)
            {
                _rightTrigger.performed -= OnRightTriggerPressed;
            }
        }
    }
}