using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ControllerButtonManager : MonoBehaviour
{

    [Header("inputActions")]
    [SerializeField] private InputActionAsset inputActions;


    [Header("Check Debug:")]
    [SerializeField] bool isDebug = true;

    private InputAction yButton;
    private InputAction BButton;
    private XRGrabInteractable grabInteractable;

    private WristUIManager wristUIManager;
    private PhotoUICloseManager photoUICloseManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            wristUIManager = FindAnyObjectByType<WristUIManager>();
            photoUICloseManager = FindAnyObjectByType<PhotoUICloseManager>();

            if (wristUIManager == null)
            {
                if (isDebug) Debug.LogError("WristUIManager not found!");
            }

            if (photoUICloseManager == null)
            {
                if (isDebug) Debug.LogError("PhotoUICloseManager not found!");
            }

            SetupInputActions();
        }
        catch (System.Exception e)
        {
            if (isDebug) Debug.LogError($"Start Exception: {e.Message}");
        }

    }

    private void SetupInputActions()
    {
        if (inputActions == null)
        {
            if (isDebug) Debug.LogError("InputActionAsset not found!");
            return;
        }

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

    private void OnYButtonPressed(InputAction.CallbackContext context)
    {
        if (isDebug) Debug.Log("Y Button Pressed");
        
        if (photoUICloseManager != null && photoUICloseManager.GetActPhotoUICanvus())
        {
            photoUICloseManager.GetOnYButtonPressed();
        }
        else if (wristUIManager != null)
        {
            
        wristUIManager.GetOnYButtonPressed();
            
        }
        
    }

    private void OnBButtonPressed(InputAction.CallbackContext context)
    {
        if (isDebug) Debug.Log("B  Button Pressed");
        if (wristUIManager != null && wristUIManager.GetActWristUI())
        {
            wristUIManager.GetOnBButtonPressed();
        }
    }

    private void OnDestroy()
    {
        if (yButton != null) yButton.performed -= OnYButtonPressed;
        if (BButton != null) BButton.performed -= OnBButtonPressed;
    }
}
