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

    [Header("UIComponents")]
    [SerializeField] private GameObject WristUI;
    [SerializeField] private GameObject TutorialUI;
                     private GameObject Tutorial1Page;
                     private GameObject Tutorial2Page;
    [SerializeField] private TextMeshProUGUI TutorialPageText;
    [SerializeField] private GameObject AudioUI;
    [SerializeField] private TextMeshProUGUI BGMText;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private TextMeshProUGUI SFXText;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private GameObject MainBackUI;
    
    private string SelectedMenu;

    [Header("InteractorComponents")]
    [SerializeField] private GameObject rightController;
                     private GameObject R_directInteractor;
                     private GameObject R_rayInteractor;
    [SerializeField] private GameObject leftController;
    [SerializeField] private GameObject uiRayInteractor;
                     private GameObject L_directInteractor;

    [Header("CameraComponents")]
    [SerializeField] private GameObject Camera;

    [Header("inputActions")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Check Debug:")]
    [SerializeField] bool isDebug = true;

    private InputAction yButton;
    private InputAction BButton;
    private XRGrabInteractable grabInteractable;

    private bool isWristUI;
    private bool isTutorialUI;
    private bool isAudioUI;
    private bool isMainBackUI;
    private int pagesNum;
    private int BGMValue;
    private int SFXValue;

    private GameManager gameManager;
    private DataManager dataManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        dataManager = FindAnyObjectByType<DataManager>();


        FindInteractorComponents();
        FindUIComponents();
        FindCameraComponents();

        isWristUI = false;
        isTutorialUI = false;
        isAudioUI = false;
        isMainBackUI = false;

        if (WristUI != null) WristUI.SetActive(isWristUI);
        if (TutorialUI != null) TutorialUI.SetActive(isTutorialUI);
        if (AudioUI != null) AudioUI.SetActive(isAudioUI);
        if (MainBackUI != null) MainBackUI.SetActive(isMainBackUI);
        if (uiRayInteractor != null) uiRayInteractor.SetActive(isWristUI);

        BGMSlider.minValue = 0;
        BGMSlider.maxValue = 100;
        BGMSlider.wholeNumbers = true;
        if(dataManager != null) BGMValue = Mathf.RoundToInt(dataManager.GetMasterVolume() * 100f);
        else BGMValue = 100;
        BGMSlider.value = BGMValue;
        BGMSlider.onValueChanged.AddListener(OnBGMSliderValueChanged);
        OnBGMSliderValueChanged(BGMValue);
        
        SFXSlider.minValue = 0;
        SFXSlider.maxValue = 100;
        SFXSlider.wholeNumbers = true;
        SFXValue = 100;
        SFXSlider.value = SFXValue;
        SFXSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);
        OnSFXSliderValueChanged(SFXValue);

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

        ResetAction();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnYButtonPressed(InputAction.CallbackContext context)
    {
        if (isWristUI || isTutorialUI || isAudioUI || isMainBackUI) { CloseAction(); }
        else { OpenAction(); }
    }
    private void OnBButtonPressed(InputAction.CallbackContext context)
    {
        if (isTutorialUI || isAudioUI || isMainBackUI) { BackAction(); }
    }

    private void OnDestroy()
    {
        if (yButton != null) yButton.performed -= OnYButtonPressed;
        if (BButton != null) BButton.performed -= OnBButtonPressed;
    }

    public void OnClickMenu()
    {
        GameObject clickedObj = EventSystem.current.currentSelectedGameObject;

        if (clickedObj != null)
        {
            SelectedMenu = clickedObj.name;

            CheckMenu();
            UpdateText();
        }
    }

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

    private void UpdateText()
    {

        if (isDebug) Debug.Log($"Clicked Menu {SelectedMenu}");
    }

    private void ResetAction()
    {
        SelectedMenu = "Not Selected";
        UpdateText();
    }

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

    private void AudioUIAction()
    {
        SelectedMenu = "AudioUIButton";
        if (isDebug) Debug.Log("The Audio Menu has been activated.");

        isWristUI = false;
        WristUI.SetActive(isWristUI);
        isAudioUI = true;
        AudioUI.SetActive(isAudioUI);

    }

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

    void OnSFXSliderValueChanged(float value)
    {
        SFXValue = Mathf.RoundToInt(value);
        if (SFXText != null)
            SFXText.text = SFXValue.ToString();
    }

    private void MainBackUIAction()
    {
        SelectedMenu = "MainBackUIButton";
        if (isDebug) Debug.Log("The MainBack Menu has been activated.");

        isWristUI = false;
        WristUI.SetActive(isWristUI);
        isMainBackUI = true;
        MainBackUI.SetActive(isMainBackUI);

    }
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


        if (WristUI == null) WristUI = transform.Find("WristUI").gameObject;
        if (WristUI != null)
        {
            if (isDebug) Debug.Log("WristUI found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning("WristUI not found!");
        }

        if (TutorialUI == null) TutorialUI = transform.Find("TutorialUI").gameObject;
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

        if (AudioUI == null) AudioUI = transform.Find("AudioUI").gameObject;
        if (AudioUI != null)
        {
            if (isDebug) Debug.Log("AudioUI found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning("AudioUI not found!");
        }

        if (MainBackUI == null) MainBackUI = transform.Find("MainBackUI").gameObject;
        if (MainBackUI != null)
        {
            if (isDebug) Debug.Log("MainBackUI found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning("MainBackUI not found!");
        }

    }

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

    private void ToggleInteractor()
    {
        if (R_directInteractor != null && R_rayInteractor != null && L_directInteractor != null)
        {

            R_directInteractor.SetActive(!isWristUI);
            R_rayInteractor.SetActive(!isWristUI);
            L_directInteractor.SetActive(!isWristUI);
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
                if (isDebug) Debug.Log("Interactor found and modified.");
            }
            else
            {
                if (isDebug) Debug.LogError("Cannot find Interactor to modify!");
            }
        }
    }

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
