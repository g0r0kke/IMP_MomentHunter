using TMPro;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PhotoUICloseManager : MonoBehaviour
{
    // === UI COMPONENT REFERENCES ===
    [Header("UIComponents")]
    [SerializeField] private GameObject MainUI;      // Primary interface panel
    [SerializeField] private string MainUIName;      // Name for locating MainUI in hierarchy
    [SerializeField] private GameObject PhotoUI;     // Photo-related UI panel
    [SerializeField] private string PhotoUIName;     // Name for locating PhotoUI in hierarchy
    [SerializeField] private GameObject CAMUI;       // Camera control interface
    [SerializeField] private string CAMUIName;       // Name for locating CAMUI in hierarchy

    // Debug
    [Header("Debug Log")]
    [SerializeField] private bool isDebug = true;            // Enable/disable diagnostic logging

    void Start()
    {
        FindUIComponents();  // Initialize UI references on startup
    }

    // Checks if photo UI is currently active in hierarchy
    public bool GetActPhotoUICanvus()
    {
        return PhotoUI != null && PhotoUI.activeInHierarchy;
    }

    /* Handles Y button press event for UI state management
    * - Closes photo UI
    * - Re-opens main UI
    * - Maintains camera UI state
    */
    public void GetOnYButtonPressed()
    {
        PhotoUI.SetActive(false);
        MainUI.SetActive(true);
        if (isDebug) Debug.Log($"{PhotoUIName} closed & {MainUIName} opened");
    }

    /* Locates and caches UI components in hierarchy using serialized names
    *  Provides fallback for manual reference assignment
    */
    private void FindUIComponents()
    {
        // Main UI panel initialization
        if (MainUI == null) MainUI = transform.Find(MainUIName)?.gameObject;
        LogComponentStatus(MainUI, MainUIName);

        // Photo UI panel initialization
        if (PhotoUI == null) PhotoUI = transform.Find(PhotoUIName)?.gameObject;
        LogComponentStatus(PhotoUI, PhotoUIName);

        // Camera UI element initialization
        if (CAMUI == null) CAMUI = transform.Find(CAMUIName)?.gameObject;
        LogComponentStatus(CAMUI, CAMUIName);
    }

    // Handles debug logging for component initialization status
    private void LogComponentStatus(GameObject component, string componentName)
    {
        if (component != null && isDebug)
        {
            Debug.Log($"{componentName} found!");
        }
        else if (isDebug)
        {
            Debug.LogWarning($"{componentName} not found!");
        }
    }
}
