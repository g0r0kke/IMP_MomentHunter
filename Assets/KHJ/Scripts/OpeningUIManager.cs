using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpeningUIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject _playUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _quitUI;
    
    [SerializeField] private GraphicRaycaster _graphicRaycaster;
    [SerializeField] private EventSystem _eventSystem;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckUIClick();
        }
    }
    
    void CheckUIClick()
    {
        PointerEventData pointerData = new PointerEventData(_eventSystem);
        pointerData.position = Input.mousePosition;
   
        List<RaycastResult> results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(pointerData, results);
        
        LayerMask uiLayerMask = LayerMask.GetMask("UI");
       
        // 히트된 UI 확인
        foreach (RaycastResult result in results)
        {
            if ((uiLayerMask.value & (1 << result.gameObject.layer)) == 0) continue;
        
            if (result.gameObject == _playUI || result.gameObject.transform.IsChildOf(_playUI.transform))
            {
                OnPlayUIClicked();
                return;
            }
            else if (result.gameObject == _settingsUI || result.gameObject.transform.IsChildOf(_settingsUI.transform))
            {
                OnSettingsUIClicked();
                return;
            }
            else if (result.gameObject == _quitUI || result.gameObject.transform.IsChildOf(_quitUI.transform))
            {
                OnQuitUIClicked();
                return;
            }
        }
    }

    private void OnPlayUIClicked()
    {
        Debug.Log("Play 클릭");
    }
    
    private void OnSettingsUIClicked()
    {
        Debug.Log("Settings 클릭");
    }
    
    private void OnQuitUIClicked()
    {
        Debug.Log("Quit 클릭");
    }
}
