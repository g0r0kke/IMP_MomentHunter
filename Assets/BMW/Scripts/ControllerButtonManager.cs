using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ControllerButtonManager : MonoBehaviour
{
    // === inputActions REFERENCES ===
    [Header("inputActions")]
    [SerializeField] private InputActionAsset inputActions; // Reference to the Input Action Asset for controller mapping

    // Debug
    [Header("Check Debug:")]
    [SerializeField] bool isDebug = true; // Toggle for debug logging

    // Controller CONFIGURATION
    private InputAction yButton;    // Action reference for the Y button (typically left controller)
    private InputAction BButton;    // Action reference for the B button (typically right controller)

    // External Script References
    private WristUIManager wristUIManager;               // Reference to Wrist UI manager script
    private PhotoUICloseManager photoUICloseManager;     // Reference to Photo UI close manager script

    void Start()
    {
        try
        {
            // Find and cache references to UI management scripts in the scene
            wristUIManager = FindAnyObjectByType<WristUIManager>();
            photoUICloseManager = FindAnyObjectByType<PhotoUICloseManager>();

            // Error logging if managers are not found
            if (wristUIManager == null)
            {
                if (isDebug) Debug.LogError("WristUIManager not found!");
            }

            if (photoUICloseManager == null)
            {
                if (isDebug) Debug.LogError("PhotoUICloseManager not found!");
            }

            // Set up input action bindings
            SetupInputActions();
        }
        catch (System.Exception e)
        {
            if (isDebug) Debug.LogError($"Start Exception: {e.Message}");
        }
    }

    // Configures input action bindings for Y and B buttons.
    // Subscribes event handlers to their performed events.
    private void SetupInputActions()
    {
        if (inputActions == null)
        {
            if (isDebug) Debug.LogError("InputActionAsset not found!");
            return;
        }

        // Locate left controller action map and bind Y button
        var LeftActionMap = inputActions?.FindActionMap("XRI Left");
        if (LeftActionMap != null)
        {
            yButton = LeftActionMap.FindAction("YButton");
            if (yButton != null)
            {
                yButton.Enable();
                yButton.performed += OnYButtonPressed;
            }
            else
            {
                if (isDebug) Debug.LogError("YButton action not found!");
            }
        }
        else
        {
            if (isDebug) Debug.LogError("XRI Left action map not found!");
        }

        // Locate right controller action map and bind B button
        var RightActionMap = inputActions?.FindActionMap("XRI Right");
        if (RightActionMap != null)
        {
            BButton = RightActionMap.FindAction("BButton");
            if (BButton != null)
            {
                BButton.Enable();
                BButton.performed += OnBButtonPressed;
            }
            else
            {
                if (isDebug) Debug.LogError("BButton action not found!");
            }
        }
        else
        {
            if (isDebug) Debug.LogError("XRI Right action map not found!");
        }
    }

    /*
    * Called when the Y button is pressed.
    * - If Photo UI is open, closes it and opens Main UI.
    * - Otherwise, toggles the Wrist UI.
    */
    private void OnYButtonPressed(InputAction.CallbackContext context)
    {
        if (isDebug) Debug.Log("Y Button Pressed");

        // Give priority to closing the Photo UI if it's active
        if (photoUICloseManager != null && photoUICloseManager.GetActPhotoUICanvus())
        {
            photoUICloseManager.GetOnYButtonPressed();
        }
        // Otherwise, toggle Wrist UI
        else if (wristUIManager != null)
        {
            wristUIManager.GetOnYButtonPressed();
        }
    }

    // Called when the B button is pressed.
    // - If Wrist UI is active, triggers its back action.
    private void OnBButtonPressed(InputAction.CallbackContext context)
    {
        if (isDebug) Debug.Log("B  Button Pressed");
        if (wristUIManager != null && wristUIManager.GetActWristUI())
        {
            wristUIManager.GetOnBButtonPressed();
        }
    }

    // Unsubscribes from input action events to prevent memory leaks.
    private void OnDestroy()
    {
        if (yButton != null) yButton.performed -= OnYButtonPressed;
        if (BButton != null) BButton.performed -= OnBButtonPressed;
    }
}
