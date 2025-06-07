using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WristUIManager : MonoBehaviour
{

    [Header("Clicks Count")]
    private int ClicksCount;
    [SerializeField] private TextMeshProUGUI ClicksCountUIText;

    [Header("Clicked Menu:")]
    private string SelectedMenu;
    [SerializeField] private TextMeshProUGUI SelectedMenuUIText;

    [Header("UIComponents")]
    [SerializeField] private GameObject WristUI;
    [SerializeField] private GameObject rightController;
    [SerializeField] private GameObject uiRayInteractor;

    [Header("inputActions")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Check Debug:")]
    [SerializeField] bool isDebug = true;

    private InputAction menu;
    private InputAction yButton;
    private XRGrabInteractable grabInteractable;

    private bool isWristUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindUIComponents();

        isWristUI = false;
        if (WristUI != null) WristUI.SetActive(isWristUI);
        if (uiRayInteractor != null) uiRayInteractor.SetActive(isWristUI);

        var actionMap = inputActions?.FindActionMap("XRI Left");
        if (actionMap != null)
        {
            yButton = actionMap.FindAction("YButton");
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

        ResetAction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnYButtonPressed(InputAction.CallbackContext context)
    {
        if (isWristUI) { CloseAction(); }
        else { OpenAction(); }
    }

    private void OnDestroy()
    {
        if (yButton != null) yButton.performed -= OnYButtonPressed;
    }

    public void OnClickMenu()
    {
        GameObject clickedObj = EventSystem.current.currentSelectedGameObject;

        if (clickedObj != null)
        {
            SelectedMenu = clickedObj.name;
            ClicksCount++;

            CheckMenu();
            UpdateText();
        }
    }

    private void CheckMenu()
    {
        switch (SelectedMenu) {

            case "CloseButton":
                CloseAction();
                break;

            case "TutorialButton":
                TutorialAction();
                break;

            case "AudioButton":
                AudioAction();
                break;

            case "MainBackButton":
                MainBackAction();
                break;

            default:
                if(isDebug) Debug.LogError("Unknown menu: " + SelectedMenu);
                break;

        }
    }

    private void UpdateText()
    {
        if (ClicksCountUIText.text != ClicksCount.ToString())
            ClicksCountUIText.text = ClicksCount.ToString();

        if (SelectedMenuUIText.text != SelectedMenu)
            SelectedMenuUIText.text = SelectedMenu;

        if (isDebug) Debug.Log($"Clicked Menu {SelectedMenu}\n" +
                               $"Click Count {ClicksCount}");
    }

    private void ResetAction()
    {
        ClicksCount = 0;
        SelectedMenu = "Not Selected";
        UpdateText();
    }

    private void OpenAction()
    {
        SelectedMenu = "OpenButton";
        isWristUI = true;
        WristUI.SetActive(isWristUI);
        ToggleUIRayInteractor();

        ResetAction();
        if (isDebug) Debug.Log("The WristUI has been activated.");
    }

    private void CloseAction()
    {
        SelectedMenu = "CloseButton";
        isWristUI = false;
        WristUI.SetActive(isWristUI);
        ToggleUIRayInteractor();

        if (isDebug) Debug.Log("The WristUI has been disabled.");
    }

    private void TutorialAction()
    {
        SelectedMenu = "TutorialButton";
        if (isDebug) Debug.Log("The Tutorial Menu has been activated.");
    }

    private void AudioAction()
    {
        SelectedMenu = "AudioButton";
        if (isDebug) Debug.Log("The Audio Menu has been activated.");
    }

    private void MainBackAction()
    {
        SelectedMenu = "MainBackButton";
        if (isDebug) Debug.Log("The MainBack Menu has been activated.");
    }

    private void FindUIComponents()
    {
        if (rightController == null) rightController = GameObject.Find("Right Controller");

        if (rightController != null)
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
        }
        else
        {
            if (isDebug) Debug.LogWarning("Right Controller not found!");
        }


        if(WristUI != null) WristUI = transform.Find("WristUI").gameObject;
        else WristUI = GameObject.Find("WristUI");

        if (WristUI != null)
        {
            if (isDebug) Debug.LogWarning("WristUI found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning("Right Controller not found!");
        }
    }

    private void ToggleUIRayInteractor()
    {
        if (uiRayInteractor != null)
        {
            uiRayInteractor.SetActive(isWristUI);
            if (isDebug) Debug.Log("UI Ray Interactor activated.");
        }
        else
        {
            FindUIComponents();
            if (uiRayInteractor != null)
            {
                uiRayInteractor.SetActive(isWristUI);
                if (isDebug) Debug.Log("UI Ray Interactor found and activated.");
            }
            else
            {
                if (isDebug) Debug.LogError("Cannot find UI Ray Interactor to activate!");
            }
        }
    }
}
