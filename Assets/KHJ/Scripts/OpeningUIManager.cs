using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class OpeningUIManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private Text _debugText; // VR에서 디버그 정보 표시용

    [Header("UI Components")]
    [SerializeField] private GameObject _playUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _quitUI;
    [SerializeField] private Canvas _openingCanvas;
    [SerializeField] private Canvas _prologueCanvas;
    
    [SerializeField] private TrackedDeviceGraphicRaycaster _trackedRaycaster;
    [SerializeField] private EventSystem _eventSystem;

    [Header("VR Components")]
    [SerializeField] private XRRayInteractor _leftRayInteractor;  // 왼쪽 컨트롤러 레이 인터랙터
    [SerializeField] private XRRayInteractor _rightRayInteractor; // 오른쪽 컨트롤러 레이 인터랙터
    [SerializeField] private InputActionAsset _inputActions;      // VR Input Actions

    // 캔버스 상태 관리 변수 (true: 프롤로그, false: 오프닝)
    private bool _isPrologueActive = false;
    
    // VR Input Actions
    private InputAction _aButton;
    private InputAction _leftTrigger;
    private InputAction _rightTrigger;
    
    // 현재 레이가 가리키고 있는 UI
    private GameObject _currentHoveredUI = null;
    
    private void Start()
    {
        SetPrologueActive(_isPrologueActive);
        SetupVRInputActions();
    }
    
    private void SetupVRInputActions()
    {
        if (_inputActions == null)
        {
            Debug.LogError("InputActionAsset not found!");
            return;
        }

        // 오른쪽 컨트롤러 A버튼 설정 (XRI Right Action Map에서)
        var rightActionMap = _inputActions?.FindActionMap("XRI Right");
        if (rightActionMap != null)
        {
            _aButton = rightActionMap.FindAction("AButton");
            if (_aButton != null)
            {
                _aButton.Enable();
                _aButton.performed += OnAButtonPressed;
            }
            else
            {
                Debug.LogError("AButton action not found!");
            }
        }
        else
        {
            Debug.LogError("XRI Right action map not found!");
        }

        // 오른쪽 트리거 설정 (XRI Right Interaction Action Map에서)
        var rightInteractionMap = _inputActions?.FindActionMap("XRI Right Interaction");
        if (rightInteractionMap != null)
        {
            _rightTrigger = rightInteractionMap.FindAction("Activate");
            if (_rightTrigger != null)
            {
                _rightTrigger.Enable();
                _rightTrigger.performed += OnRightTriggerPressed;
            }
            else
            {
                Debug.LogError("Right Activate action not found in XRI Right Interaction!");
            }
        }
        else
        {
            Debug.LogError("XRI Right Interaction action map not found!");
        }

        // 왼쪽 트리거 설정 (XRI Left Interaction Action Map에서)
        var leftInteractionMap = _inputActions?.FindActionMap("XRI Left Interaction");
        if (leftInteractionMap != null)
        {
            _leftTrigger = leftInteractionMap.FindAction("Activate");
            if (_leftTrigger != null)
            {
                _leftTrigger.Enable();
                _leftTrigger.performed += OnLeftTriggerPressed;
            }
            else
            {
                Debug.LogError("Left Activate action not found in XRI Left Interaction!");
            }
        }
        else
        {
            Debug.LogError("XRI Left Interaction action map not found!");
        }
    }
    
    private void Update()
    {
        // VR 환경에서는 레이캐스트로 UI 검사
        if (!_isPrologueActive)
        {
            CheckVRUIHover();
        }
    }
    
    void CheckVRUIHover()
    {
        if (_isPrologueActive) 
        {
            UpdateDebugText("Prologue is active");
            return;
        }
        
        GameObject hitUI = null;
        string raySource = "";
        
        // 왼쪽 레이 먼저 확인
        if (_leftRayInteractor != null && _leftRayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult leftRaycastResult))
        {
            hitUI = leftRaycastResult.gameObject;
            raySource = "Left Ray";
        }
        // 왼쪽에 없으면 오른쪽 레이 확인
        else if (_rightRayInteractor != null && _rightRayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult rightRaycastResult))
        {
            hitUI = rightRaycastResult.gameObject;
            raySource = "Right Ray";
        }
        
        if (hitUI != null)
        {
            UpdateDebugText($"{raySource} hit: {hitUI.name}");
            
            // 현재 가리키고 있는 UI 업데이트
            if (IsTargetUI(hitUI))
            {
                _currentHoveredUI = hitUI;
                UpdateDebugText($"{raySource} - Target UI: {hitUI.name}");
            }
            else
            {
                _currentHoveredUI = null;
                UpdateDebugText($"{raySource} - Non-target: {hitUI.name}");
            }
        }
        else
        {
            _currentHoveredUI = null;
            UpdateDebugText("No UI hit");
        }
    }
    
    private void UpdateDebugText(string message)
    {
        if (_debugText != null)
        {
            _debugText.text = message;
        }
    }
    
    private bool IsTargetUI(GameObject hitObject)
    {
        if (hitObject == null) return false;
        
        return (hitObject == _playUI || hitObject.transform.IsChildOf(_playUI.transform)) ||
               (hitObject == _settingsUI || hitObject.transform.IsChildOf(_settingsUI.transform)) ||
               (hitObject == _quitUI || hitObject.transform.IsChildOf(_quitUI.transform));
    }
    
    private void OnAButtonPressed(InputAction.CallbackContext context)
    {
        if (_isPrologueActive)
        {
            // 프롤로그에서 A버튼 누르면 다음 씬으로 (A버튼만 프롤로그에서 작동)
            GameManager.Instance.TransitionToScene(1);
        }
        else if (_currentHoveredUI != null)
        {
            // 오프닝에서 레이가 UI에 닿은 상태에서 A버튼 누르면 해당 UI 클릭
            HandleUIClick(_currentHoveredUI);
        }
    }
    
    private void OnLeftTriggerPressed(InputAction.CallbackContext context)
    {
        // 왼쪽 트리거는 UI 선택에만 사용 (프롤로그에서는 작동 안함)
        if (!_isPrologueActive && _currentHoveredUI != null)
        {
            HandleUIClick(_currentHoveredUI);
        }
    }
    
    private void OnRightTriggerPressed(InputAction.CallbackContext context)
    {
        // 오른쪽 트리거는 UI 선택에만 사용 (프롤로그에서는 작동 안함)
        if (!_isPrologueActive && _currentHoveredUI != null)
        {
            HandleUIClick(_currentHoveredUI);
        }
    }
    
    private void HandleUIClick(GameObject clickedUI)
    {
        if (clickedUI == _playUI || clickedUI.transform.IsChildOf(_playUI.transform))
        {
            OnPlayUIClicked();
        }
        else if (clickedUI == _settingsUI || clickedUI.transform.IsChildOf(_settingsUI.transform))
        {
            OnSettingsUIClicked();
        }
        else if (clickedUI == _quitUI || clickedUI.transform.IsChildOf(_quitUI.transform))
        {
            OnQuitUIClicked();
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
    
    private void OnDestroy()
    {
        if (_aButton != null) 
        {
            _aButton.performed -= OnAButtonPressed;
        }
        if (_leftTrigger != null)
        {
            _leftTrigger.performed -= OnLeftTriggerPressed;
        }
        if (_rightTrigger != null)
        {
            _rightTrigger.performed -= OnRightTriggerPressed;
        }
    }
}