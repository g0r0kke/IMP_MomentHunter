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

    [Header("WristUI")]
    [SerializeField] private GameObject WristUI;

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
        if(WristUI == null) WristUI = transform.Find("WristUI").gameObject;
        WristUI.SetActive(false);
        isWristUI = false;

        yButton = inputActions.FindActionMap("XRI Left").FindAction("YButton");
        yButton.Enable();
        yButton.performed += OnYButtonPressed;

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
        yButton.performed -= OnYButtonPressed;
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
        WristUI.SetActive(true);
        isWristUI = true;
        ResetAction();
        if (isDebug) Debug.Log("The WristUI has been activated.");
    }

    private void CloseAction()
    {
        SelectedMenu = "CloseButton";
        WristUI.SetActive(false);
        isWristUI = false;
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
}
