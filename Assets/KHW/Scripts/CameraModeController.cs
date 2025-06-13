using UnityEngine;
using UnityEngine.InputSystem;
public class CameraModeController : MonoBehaviour
{
    public static CameraModeController Instance;
    public GameObject cameraUI;   // Camera UI object
    public GameObject cameraModel;  // Camera 3D model

    public InputActionProperty gripAction;    // Grip button input

    public bool IsActive { get; private set; }   // Current camera mode state

    void Awake()
    {
        Instance = this;    // Singleton instance
    }
    void Start()
    {
        cameraUI.SetActive(false);   // Hide UI at start
    }

    // Input binding
    void OnEnable()
    {
        gripAction.action.performed += OnGripPressed;   // On grip press
        gripAction.action.canceled  += OnGripReleased;  // On grip release
        gripAction.action.Enable();
    }
    void OnDisable()
    {
        gripAction.action.performed -= OnGripPressed;
        gripAction.action.canceled  -= OnGripReleased;
        gripAction.action.Disable();
    }

    // Event handlers
    void OnGripPressed(InputAction.CallbackContext _)
    {
        if (IsActive) return;
        IsActive = true;

        cameraUI.SetActive(true);   // Show camera UI
        cameraModel.SetActive(false);   // Hide camera model
        TutorialManager.Instance?.OnRightGrabDone();   // Notify tutorial progress
    }

    void OnGripReleased(InputAction.CallbackContext _)
    {
        if (!IsActive) return;
        IsActive = false;

        cameraUI.SetActive(false);   // Hide camera UI
        cameraModel.SetActive(true);   // Show camera model
    }
}
