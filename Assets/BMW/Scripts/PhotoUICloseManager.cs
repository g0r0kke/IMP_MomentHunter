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

    [Header("UIComponents")]
    [SerializeField] private GameObject MainUI;
    [SerializeField] private string MainUIName;
    [SerializeField] private GameObject PhotoUI;
    [SerializeField] private string PhotoUIName;
    [SerializeField] private GameObject CAMUI;
    [SerializeField] private string CAMUIName;

    [Header("Check Debug:")]
    [SerializeField] bool isDebug = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        FindUIComponents();

    }


    public bool GetActPhotoUICanvus()
    {
        if (PhotoUI != null && PhotoUI.activeInHierarchy) { return true; }
        else { return false; }
    }

    public void GetOnYButtonPressed()
    {
        PhotoUI.SetActive(false);
        MainUI.SetActive(true);
        if (isDebug) Debug.Log($"{PhotoUIName} closed & {MainUIName} opend");

    }

    private void FindUIComponents()
    {

        if (MainUI == null) MainUI = transform.Find(MainUIName).gameObject;
        if (MainUI != null)
        {
            if (isDebug) Debug.Log($"{MainUIName} found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning($"{MainUIName} not found!");
        }

        if (PhotoUI == null) PhotoUI = transform.Find(PhotoUIName).gameObject;
        if (PhotoUI != null)
        {
            if (isDebug) Debug.Log($"{PhotoUIName} found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning($"{PhotoUIName} not found!");
        }

        if (CAMUI == null) CAMUI = transform.Find(CAMUIName).gameObject;
        if (CAMUI != null)
        {
            if (isDebug) Debug.Log($"{CAMUIName} found!");
        }
        else
        {
            if (isDebug) Debug.LogWarning($"{CAMUIName} not found!");
        }

    }
}
