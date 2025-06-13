using TMPro;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WristUIManager : MonoBehaviour
{
    // UI Component References
    [Header("UIComponents")]
    [SerializeField] private GameObject WristCanvus; // Parent canvas for wrist UI
    [SerializeField] private GameObject WristUI;     // Main wrist UI panel
    [SerializeField] private GameObject TutorialUI;  // Tutorial UI panel
                     private GameObject Tutorial1Page; // First tutorial page
                     private GameObject Tutorial2Page; // Second tutorial page
    [SerializeField] private TextMeshProUGUI TutorialPageText; // Tutorial page indicator text
    [SerializeField] private GameObject AudioUI;     // Audio settings UI panel
    [SerializeField] private TextMeshProUGUI BGMText; // Background music volume text
    [SerializeField] private Slider BGMSlider;        // BGM volume slider
    [SerializeField] private TextMeshProUGUI SFXText; // SFX volume text
    [SerializeField] private Slider SFXSlider;        // SFX volume slider
    [SerializeField] private GameObject MainBackUI;   // Main back UI panel
    [SerializeField] private GameObject CamUI;        // Camera UI element

    private string SelectedMenu; // Current selected menu/button

    // Interactor Component References
    [Header("InteractorComponents")]
    [SerializeField] private GameObject rightController; // Right VR controller
                     private GameObject R_directInteractor; // Right direct interactor
                     private GameObject R_rayInteractor;    // Right ray interactor
    [SerializeField] private GameObject leftController;     // Left VR controller
    [SerializeField] private GameObject uiRayInteractor;    // UI ray interactor
                     private GameObject L_directInteractor; // Left direct interactor

    // Camera Reference
    [Header("CameraComponents")]
    [SerializeField] private GameObject Camera; // Main camera holder

    // Debug and State Flags
    [Header("Debug Log")]
    [SerializeField] private bool isDebug = true; // Enable debug logging
    private bool isWristUI;      // Wrist UI active state
    private bool isTutorialUI;   // Tutorial UI active state
    private bool isAudioUI;      // Audio UI active state
    private bool isMainBackUI;   // Main back UI active state
    private int pagesNum;        // Current tutorial page number
    private int BGMValue;        // Current BGM value (0-100)
    private int SFXValue;        // Current SFX value (0-100)

    // External Script References
    private GameManager gameManager; // Reference to GameManager script
    private DataManager dataManager; // Reference to DataManager script

    void Awake()
    {
        // Find and cache references to GameManager and DataManager
        gameManager = FindAnyObjectByType<GameManager>();
        dataManager = FindAnyObjectByType<DataManager>();

        // Find and cache references to interactors, UI, and camera
        FindInteractorComponents();
        FindUIComponents();
        FindCameraComponents();

        // Initialize UI state flags
        isWristUI = false;
        isTutorialUI = false;
        isAudioUI = false;
        isMainBackUI = false;

        // Set initial UI active states
        if (WristUI != null) WristUI.SetActive(isWristUI);
        if (TutorialUI != null) TutorialUI.SetActive(isTutorialUI);
        if (AudioUI != null) AudioUI.SetActive(isAudioUI);
        if (MainBackUI != null) MainBackUI.SetActive(isMainBackUI);
        if (uiRayInteractor != null) uiRayInteractor.SetActive(isWristUI);

        // Initialize BGM slider
        BGMSlider.minValue = 0;
        BGMSlider.maxValue = 100;
        BGMSlider.wholeNumbers = true;
        if (dataManager != null) BGMValue = Mathf.RoundToInt(dataManager.GetMasterVolume() * 100f);
        else BGMValue = 100;
        BGMSlider.value = BGMValue;
        BGMSlider.onValueChanged.AddListener(OnBGMSliderValueChanged);
        OnBGMSliderValueChanged(BGMValue);

        // Initialize SFX slider
        SFXSlider.minValue = 0;
        SFXSlider.maxValue = 100;
        SFXSlider.wholeNumbers = true;
        SFXValue = 100;
        SFXSlider.value = SFXValue;
        SFXSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);
        OnSFXSliderValueChanged(SFXValue);

        ResetAction(); // Reset menu selection
    }

    // Returns true if any sub-UI (tutorial, audio, main back) is active
    public bool GetActWristUI()
    {
        if (isTutorialUI || isAudioUI || isMainBackUI) { return true; }
        else { return false; }
    }

    // Called when the Y button is pressed: toggles the wrist UI open/close
    public void GetOnYButtonPressed()
    {
        if (isDebug) Debug.Log("WristUI Called");
        if (isWristUI || isTutorialUI || isAudioUI || isMainBackUI) { CloseAction(); }
        else { OpenAction(); }
    }

    // Called when the B button is pressed: goes back within sub-UIs
    public void GetOnBButtonPressed()
    {
        if (isTutorialUI || isAudioUI || isMainBackUI) { BackAction(); }
    }

    // Handles menu button clicks
    public void OnClickMenu()
    {
        GameObject clickedObj = EventSystem.current.currentSelectedGameObject;

        if (clickedObj != null)
        {
            SelectedMenu = clickedObj.name;

            CheckMenu();   // Handle menu logic
            UpdateText();  // Update debug text
        }
    }

    // Switches logic based on which menu/button was selected
    private void CheckMenu()
    {
        switch (SelectedMenu)
        {
            case "CloseButton":
                CloseAction();
                break;
            case "BackButton":
                BackAction();
                break;
            case "TutorialUIButton":
                TutorialUIAction();
                break;
            case "PreButton":
                TurningPageAction();
                break;
            case "NextButton":
                TurningPageAction();
                break;
            case "AudioUIButton":
                AudioUIAction();
                break;
            case "MainBackUIButton":
                MainBackUIAction();
                break;
            case "MainBackButton":
                MainBackAction();
                break;
            default:
                if (isDebug) Debug.LogError("Unknown menu: " + SelectedMenu);
                break;
        }
    }

    // Updates debug text for menu selection
    private void UpdateText()
    {
        if (isDebug) Debug.Log($"Clicked Menu {SelectedMenu}");
    }

    // Resets menu selection to default
    private void ResetAction()
    {
        SelectedMenu = "Not Selected";
        UpdateText();
    }

    // Opens the wrist UI and sets relevant states
    private void OpenAction()
    {
        SelectedMenu = "OpenButton";
        isWristUI = true;
        WristUI.SetActive(isWristUI);
        ToggleUIRayInteractor();
        ToggleInteractor();
        ToggleCamera();

        ResetAction();
        if (isDebug) Debug.Log("The WristUI has been activated.");
    }

    // Closes all sub-UIs and wrist UI
    private void CloseAction()
    {
        SelectedMenu = "CloseButton";

        isWristUI = false;
        WristUI.SetActive(isWristUI);
        isTutorialUI = false;
        TutorialUI.SetActive(isTutorialUI);
        isAudioUI = false;
        AudioUI.SetActive(isAudioUI);
        isMainBackUI = false;
        MainBackUI.SetActive(isMainBackUI);

        ToggleUIRayInteractor();
        ToggleInteractor();
        ToggleCamera();

        if (isDebug) Debug.Log("The WristUI has been disabled.");
    }

    // Goes back to the main wrist UI from sub-UIs
    private void BackAction()
    {
        SelectedMenu = "BackButton";

        isTutorialUI = false;
        TutorialUI.SetActive(isTutorialUI);
        isAudioUI = false;
        AudioUI.SetActive(isAudioUI);
        isMainBackUI = false;
        MainBackUI.SetActive(isMainBackUI);
        isWristUI = true;
        WristUI.SetActive(isWristUI);

        if (isDebug) Debug.Log("The Back Menu has been activated.");
    }

    // Opens the tutorial UI and shows the first page
    private void TutorialUIAction()
    {
        SelectedMenu = "TutorialUIButton";
        if (isDebug) Debug.Log("The Tutorial Menu has been activated.");
        pagesNum = 1;
        isWristUI = false;
        WristUI.SetActive(isWristUI);
        isTutorialUI = true;
        TutorialUI.SetActive(isTutorialUI);

        TutorialPageText.text = pagesNum.ToString();
        Tutorial2Page.SetActive(false);
        Tutorial1Page.SetActive(true);
    }

    // Handles tutorial page turning logic
    private void TurningPageAction()
    {
        SelectedMenu = "TurningPageButton";
        if (isDebug) Debug.Log("TurningPageButton has been activated.");

        pagesNum++;
        switch (pagesNum)
        {
            case 2:
                Tutorial1Page.SetActive(false);
                Tutorial2Page.SetActive(true);
                break;
            case 1:
            default:
                pagesNum = 1;
                Tutorial2Page.SetActive(false);
                Tutorial1Page.SetActive(true);
                break;
        }
        TutorialPageText.text = pagesNum.ToString();
    }

    // Opens the audio settings UI
    private void AudioUIAction()
    {
        SelectedMenu = "AudioUIButton";
        if (isDebug) Debug.Log("The Audio Menu has been activated.");

        isWristUI = false;
        WristUI.SetActive(isWristUI);
        isAudioUI = true;
        AudioUI.SetActive(isAudioUI);
    }

    // Called when BGM slider value changes
    void OnBGMSliderValueChanged(float value)
    {
        BGMValue = Mathf.RoundToInt(value);
        if (BGMText != null)
            BGMText.text = BGMValue.ToString();
        if (dataManager != null)
        {
            if (isDebug) Debug.Log("dataManager found.");
            dataManager.SetMasterVolume(BGMValue);
        }
        else
        {
            if (isDebug) Debug.LogWarning("dataManager not found");
        }
    }

    // Called when SFX slider value changes
    void OnSFXSliderValueChanged(float value)
    {
        SFXValue = Mathf.RoundToInt(value);
        if (SFXText != null)
            SFXText.text = SFXValue.ToString();
    }

    // Opens the main back UI
    private void MainBackUIAction()
    {
        SelectedMenu = "MainBackUIButton";
        if (isDebug) Debug.Log("The MainBack Menu has been activated.");

        isWristUI = false;
        WristUI.SetActive(isWristUI);
        isMainBackUI = true;
        MainBackUI.SetActive(isMainBackUI);
    }

    // Handles main back button logic (scene transition)
    private void MainBackAction()
    {
        if (gameManager != null)
        {
            if (isDebug) Debug.Log("gameManager found.");
            gameManager.TransitionToScene(0);
        }
        else
        {
            if (isDebug) Debug.LogWarning("gameManager not found");
        }
    }

    // Finds and caches references to interactor components
    private void FindInteractorComponents()
    {
        if (rightController == null) rightController = GameObject.Find("Right Controller");
        if (rightController != null)
        {
            R_directInteractor = rightController.transform.Find("Direct Interactor").gameObject;
            R_rayInteractor = rightController.transform.Find("Ray Interactor").gameObject;

            if (R_directInteractor != null && R_rayInteractor != null)
            {
                if (isDebug) Debug.Log("Interactor found and cached.");
            }
            else
            {
                if (isDebug) Debug.LogWarning("Interactor not found under Right Controller!");
            }
        }
        else
        {
            if (isDebug) Debug.LogWarning("Right Controller not found!");
        }

        if (leftController == null) leftController = GameObject.Find("Left Controller");
        if (leftController != null)
        {
            L_directInteractor = leftController.transform.Find("Direct Interactor").gameObject;

            if (L_directInteractor != null)
            {
                if (isDebug) Debug.Log("Interactor found and cached.");
            }
            else
            {
                if (isDebug) Debug.LogWarning("Interactor not found under Left Controller!");
            }
        }
        else
        {
            if (isDebug) Debug.LogWarning("Left Controller not found!");
        }
    }

    // Finds and caches references to UI components
    private void FindUIComponents()
    {
        if (uiRayInteractor == null) uiRayInteractor = rightController.transform.Find("UI Ray Interactor").gameObject;
        if (uiRayInteractor != null)
        {
            if (isDebug) Debug.Log("UI Ray Interactor found and cached.");
        }
        else
        {
            if (isDebug) Debug.LogWarning("UI Ray Interactor not found under Right Controller!");
        }

        if (WristCanvus != null)
        {
            if (isDebug) Debug.Log("WristCanvus found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning("WristCanvus not found!");
        }
        if (WristUI == null) WristUI = WristCanvus.transform.Find("WristUI").gameObject;
        if (WristUI != null)
        {
            if (isDebug) Debug.Log("WristUI found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning("WristUI not found!");
        }

        if (TutorialUI == null) TutorialUI = WristCanvus.transform.Find("TutorialUI").gameObject;
        if (TutorialUI != null)
        {
            if (isDebug) Debug.Log("TutorialUI found!");

            Tutorial1Page = TutorialUI.transform.Find("Tutorial1Page").gameObject;
            if (Tutorial1Page != null)
            {
                if (isDebug) Debug.Log("Tutorial1Page found!");
            }
            else
            {
                if (isDebug) Debug.LogWarning("Tutorial1Page not found!");
            }

            Tutorial2Page = TutorialUI.transform.Find("Tutorial2Page").gameObject;
            if (Tutorial2Page != null)
            {
                if (isDebug) Debug.Log("Tutorial2Page found!");
            }
            else
            {
                if (isDebug) Debug.LogWarning("Tutorial2Page not found!");
            }
        }
        else
        {
            if (isDebug) Debug.LogWarning("TutorialUI not found!");
        }

        if (AudioUI == null) AudioUI = WristCanvus.transform.Find("AudioUI").gameObject;
        if (AudioUI != null)
        {
            if (isDebug) Debug.Log("AudioUI found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning("AudioUI not found!");
        }

        if (MainBackUI == null) MainBackUI = WristCanvus.transform.Find("MainBackUI").gameObject;
        if (MainBackUI != null)
        {
            if (isDebug) Debug.Log("MainBackUI found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning("MainBackUI not found!");
        }

        if (CamUI != null)
        {
            if (isDebug) Debug.Log("CamUI found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning("CamUI not found!");
        }
    }

    // Finds and caches reference to the camera
    private void FindCameraComponents()
    {
        if (Camera == null) Camera = GameObject.Find("Camera Holder");
        if (Camera != null)
        {
            if (isDebug) Debug.Log("Camera found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning("Camera not found!");
        }
    }

    // Toggles the interactors' active state based on wrist UI state
    private void ToggleInteractor()
    {
        if (R_directInteractor != null && R_rayInteractor != null && L_directInteractor != null)
        {
            R_directInteractor.SetActive(!isWristUI);
            R_rayInteractor.SetActive(!isWristUI);
            L_directInteractor.SetActive(!isWristUI);
            if (CamUI != null && isWristUI) CamUI.SetActive(!isWristUI);
            if (isDebug) Debug.Log("Interactor modified.");
        }
        else
        {
            FindInteractorComponents();
            if (uiRayInteractor != null)
            {
                R_directInteractor.SetActive(!isWristUI);
                R_rayInteractor.SetActive(!isWristUI);
                L_directInteractor.SetActive(!isWristUI);
                if (CamUI != null) CamUI.SetActive(!isWristUI);
                if (isDebug) Debug.Log("Interactor found and modified.");
            }
            else
            {
                if (isDebug) Debug.LogError("Cannot find Interactor to modify!");
            }
        }
    }

    // Toggles the UI ray interactor based on wrist UI state
    private void ToggleUIRayInteractor()
    {
        if (uiRayInteractor != null)
        {
            uiRayInteractor.SetActive(isWristUI);
            if (isDebug) Debug.Log("UI Ray Interactor modified.");
        }
        else
        {
            FindInteractorComponents();
            FindUIComponents();
            if (uiRayInteractor != null)
            {
                uiRayInteractor.SetActive(isWristUI);
                if (isDebug) Debug.Log("UI Ray Interactor found and modified.");
            }
            else
            {
                if (isDebug) Debug.LogError("Cannot find UI Ray Interactor to modified!");
            }
        }
    }

    // Toggles the camera active state based on wrist UI state
    private void ToggleCamera()
    {
        if (Camera != null)
        {
            Camera.SetActive(!isWristUI);
            if (isDebug) Debug.Log("Camera modified.");
        }
        else
        {
            FindCameraComponents();
            if (Camera != null)
            {
                Camera.SetActive(!isWristUI);
                if (isDebug) Debug.Log("Camera found and modified.");
            }
            else
            {
                if (isDebug) Debug.LogError("Cannot find Camera to modified!");
            }
        }
    }
}
