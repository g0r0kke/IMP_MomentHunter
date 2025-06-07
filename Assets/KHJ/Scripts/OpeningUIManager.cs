using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OpeningUIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject _playUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _quitUI;
    [SerializeField] private Canvas _openingCanvas;
    [SerializeField] private Canvas _prologueCanvas;
    
    [SerializeField] private GraphicRaycaster _graphicRaycaster;
    [SerializeField] private EventSystem _eventSystem;

    // 캔버스 상태 관리 변수 (true: 프롤로그, false: 오프닝)
    private bool _isPrologueActive = false;
    
    private void Start()
    {
        SetPrologueActive(_isPrologueActive);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isPrologueActive)
        {
            CheckUIClick();
        }

        if (Keyboard.current.aKey.wasPressedThisFrame && _isPrologueActive)
        {
            GameManager.Instance.TransitionToScene(1);
        }
    }
    
    void CheckUIClick()
    {
        if (_isPrologueActive) return;
        
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
        SetPrologueActive(true); // 프롤로그 캔버스로 전환
    }
    
    private void OnSettingsUIClicked()
    {
        Debug.Log("Settings 클릭");
    }
    
    private void OnQuitUIClicked()
    {
        Debug.Log("Quit 클릭");
    }
    
    // 캔버스 상태를 설정하는 메서드
    public void SetPrologueActive(bool isPrologueActive)
    {
        _isPrologueActive = isPrologueActive;
        _openingCanvas.enabled = !_isPrologueActive;
        _prologueCanvas.enabled = _isPrologueActive;
    }
}
