using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class WristUIManager : MonoBehaviour
{

    [Header("Clicks Count")]
    private int ClicksCount;
    [SerializeField] private TextMeshProUGUI ClicksCountUIText;

    [Header("Clicked Menu:")]
    private string SelectedMenu;
    [SerializeField] private TextMeshProUGUI SelectedMenuUIText;

    [Header("Check Debug:")]
    [SerializeField] bool IsBebug = true;

    public InputActionAsset inputActions;
    private Canvas wristUICanvas;
    private InputAction menu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wristUICanvas = GetComponent<Canvas>();
        menu = inputActions.FindActionMap("XRI LeftHand").FindAction("Menu");
        menu.Enable();
        menu.performed += ToggleMenu;
        ResetMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateText()
    {
        ClicksCountUIText.text = ClicksCount.ToString();
        SelectedMenuUIText.text = SelectedMenu;

        if (IsBebug) Debug.Log($"Clicked Menu {SelectedMenu}" +
                               $"Click Count {ClicksCount}");
    }

    private void OnDestroy()
    {
        menu.performed -= ToggleMenu;
    }

    public void ToggleMenu(InputAction.CallbackContext context)
    {
        wristUICanvas.enabled = !wristUICanvas.enabled;
    }

    public void OnClickMenu()
    {
        GameObject clickedObj = EventSystem.current.currentSelectedGameObject;

        if (clickedObj != null)
        {
            SelectedMenu = clickedObj.name;
            ClicksCount++;

            UpdateText();
            CheckMenu();
        }
    }

    private void CheckMenu()
    {
        switch (SelectedMenu) {

            case "ResetMenu":
                ResetMenu();
                break;

            case "GalleryMenu":
                ResetMenu();
                break;

            case "TutorialMenu":
                ResetMenu();
                break;

            default:
                ResetMenu();
                break;

        }
    }

    private void ResetMenu()
    {
        ClicksCount = 0;
        SelectedMenu = "Not Selected";
    }
}
