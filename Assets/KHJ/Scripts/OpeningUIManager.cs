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
    
    // 새로 추가된 UI 팝업들
    [Header("Popup UI Components")]
    [SerializeField] private GameObject _settingsPopupUI;  // 설정 팝업 UI
    [SerializeField] private GameObject _quitPopupUI;      // 종료 팝업 UI
    
    // 설정 및 종료 UI 내부 컴포넌트들
    [Header("Settings UI Components")]
    [SerializeField] private Slider _soundSlider;         // 사운드 슬라이더
    [SerializeField] private Text _soundValueText;        // 사운드 값 표시 텍스트 (선택사항)
    
    [Header("Quit UI Components")]
    [SerializeField] private Button _mainBackButton;      // 메인으로 돌아가기 버튼
    
    [SerializeField] private TrackedDeviceGraphicRaycaster _trackedRaycaster;
    [SerializeField] private EventSystem _eventSystem;

    [Header("VR Components")]
    [SerializeField] private XRRayInteractor _leftRayInteractor;  // 왼쪽 컨트롤러 레이 인터랙터
    [SerializeField] private XRRayInteractor _rightRayInteractor; // 오른쪽 컨트롤러 레이 인터랙터
    [SerializeField] private InputActionAsset _inputActions;      // VR Input Actions

    // 캔버스 상태 관리 변수 (true: 프롤로그, false: 오프닝)
    private bool _isPrologueActive = false;
    
    // UI 팝업 상태 관리
    private bool _isSettingsPopupActive = false;
    private bool _isQuitPopupActive = false;
    
    // 입력 중복 방지
    private bool _isProcessingInput = false;
    private bool _isQuittingGame = false;  // 게임 종료 중 플래그
    private float _inputCooldown = 0.5f; // 0.5초 쿨다운
    private float _lastInputTime = 0f;
    
    // VR Input Actions
    private InputAction _aButton;
    private InputAction _yButton;
    private InputAction _leftTrigger;
    private InputAction _rightTrigger;
    
    // 현재 레이가 가리키고 있는 UI
    private GameObject _currentHoveredUI = null;
    
    // 슬라이더 이벤트 리스너가 등록되었는지 확인하는 플래그
    private bool _sliderListenerRegistered = false;
    
    private void Start()
    {
        SetPrologueActive(_isPrologueActive);
        InitializePopupUI();
        SetupVRInputActions();
        SetupSliderEvents(); // 슬라이더 이벤트 설정 추가
    }
    
    private void InitializePopupUI()
    {
        // 팝업 UI들을 초기에는 비활성화
        if (_settingsPopupUI != null)
            _settingsPopupUI.SetActive(false);
        if (_quitPopupUI != null)
            _quitPopupUI.SetActive(false);
    }
    
    private void SetupSliderEvents()
    {
        if (_soundSlider != null && !_sliderListenerRegistered)
        {
            // Unity의 기본 슬라이더 이벤트 시스템 사용
            _soundSlider.onValueChanged.AddListener(OnSoundSliderValueChanged);
            _sliderListenerRegistered = true;
            
            // 슬라이더 설정 (0~1 범위, 연속적인 값)
            _soundSlider.minValue = 0f;
            _soundSlider.maxValue = 1f;
            _soundSlider.wholeNumbers = false; // 스무스한 조작을 위해 연속값 사용
            
            Debug.Log("Sound slider events setup completed");
        }
    }
    
    private void OnSoundSliderValueChanged(float value)
    {
        // 슬라이더 값이 변경될 때마다 호출 (드래그 중에도 실시간으로 호출)
        int volumeLevel = Mathf.RoundToInt(value * 100f);
        
        // DataManager에 볼륨 설정
        if (DataManager.Data != null)
        {
            DataManager.Data.SetMasterVolume(volumeLevel);
        }
        
        // 옵션: 볼륨 값을 텍스트로 표시
        if (_soundValueText != null)
        {
            _soundValueText.text = volumeLevel.ToString() + "%";
        }
        
        Debug.Log($"Sound volume updated to: {volumeLevel}% (Slider value: {value:F2})");
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

        // 왼쪽 컨트롤러 Y버튼 설정 (XRI Left Action Map에서)
        var leftActionMap = _inputActions?.FindActionMap("XRI Left");
        if (leftActionMap != null)
        {
            _yButton = leftActionMap.FindAction("YButton");
            if (_yButton != null)
            {
                _yButton.Enable();
                _yButton.performed += OnYButtonPressed;
            }
            else
            {
                Debug.LogError("YButton action not found!");
            }
        }
        else
        {
            Debug.LogError("XRI Left action map not found!");
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
    
    // 특정 레이 인터랙터에서 히트된 UI를 가져오는 헬퍼 메서드
    private GameObject GetRayHitUI(XRRayInteractor rayInteractor)
    {
        if (rayInteractor != null && rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
        {
            return raycastResult.gameObject;
        }
        return null;
    }
    
    // A버튼/트리거로 슬라이더 값을 오른쪽 레이 위치에 따라 업데이트
    private bool UpdateSliderFromRaycast()
    {
        // 오른쪽 레이에서 히트 포인트 가져오기
        if (_rightRayInteractor != null && _rightRayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult))
        {
            if (_soundSlider == null) 
            {
                Debug.LogWarning("Sound slider is null");
                return false;
            }
            
            // 슬라이더의 RectTransform 가져오기
            RectTransform sliderRect = _soundSlider.GetComponent<RectTransform>();
            if (sliderRect == null) 
            {
                Debug.LogWarning("Slider RectTransform is null");
                return false;
            }
            
            // 월드 포인트를 슬라이더의 로컬 포인트로 변환
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                sliderRect, raycastResult.screenPosition, raycastResult.module.eventCamera, out localPoint))
            {
                // 슬라이더의 로컬 좌표에서 0~1 값으로 변환
                Rect rect = sliderRect.rect;
                float normalizedValue = Mathf.Clamp01((localPoint.x - rect.xMin) / rect.width);
                
                // 슬라이더 값 설정 (이때 OnSoundSliderValueChanged가 자동 호출됨)
                _soundSlider.value = normalizedValue;
                
                Debug.Log($"Slider updated from button press: {normalizedValue:F2} (Volume level: {Mathf.RoundToInt(normalizedValue * 100f)}%)");
                return true;
            }
            else
            {
                Debug.LogWarning("Failed to convert screen point to local point");
            }
        }
        else
        {
            Debug.LogWarning("No raycast hit from right ray interactor");
        }
        
        return false;
    }
    
    private bool IsTargetUI(GameObject hitObject)
    {
        if (hitObject == null) return false;
    
        // 팝업이 열려있을 때는 해당 팝업의 UI만 허용
        if (_isSettingsPopupActive)
        {
            // 설정 팝업에서는 슬라이더와 관련된 모든 UI 요소 허용
            bool isSettingsSlider = _soundSlider != null && 
                                    (hitObject == _soundSlider.gameObject || 
                                     hitObject.transform.IsChildOf(_soundSlider.transform) ||
                                     hitObject.GetComponent<Slider>() == _soundSlider);
        
            return isSettingsSlider;
        }
    
        if (_isQuitPopupActive)
        {
            // 종료 팝업에서는 메인 돌아가기 버튼만 허용
            bool isMainBackButton = _mainBackButton != null && 
                                    (hitObject == _mainBackButton.gameObject || 
                                     hitObject.transform.IsChildOf(_mainBackButton.transform));
        
            return isMainBackButton;
        }
    
        // 팝업이 없을 때는 기본 메뉴 UI만 허용
        bool isMainMenuUI = (hitObject == _playUI || hitObject.transform.IsChildOf(_playUI.transform)) ||
                            (hitObject == _settingsUI || hitObject.transform.IsChildOf(_settingsUI.transform)) ||
                            (hitObject == _quitUI || hitObject.transform.IsChildOf(_quitUI.transform));
    
        return isMainMenuUI;
    }
    
    private void OnAButtonPressed(InputAction.CallbackContext context)
    {
        if (_isProcessingInput || Time.time - _lastInputTime < _inputCooldown)
            return;
            
        _isProcessingInput = true;
        _lastInputTime = Time.time;
        
        if (_isPrologueActive)
        {
            // 프롤로그에서 A버튼 누르면 다음 씬으로 (A버튼만 프롤로그에서 작동)
            GameManager.Instance.TransitionToScene(1);
        }
        else
        {
            // A버튼은 오른쪽 레이만 확인
            GameObject rightHitUI = GetRayHitUI(_rightRayInteractor);
            if (rightHitUI != null && IsTargetUI(rightHitUI))
            {
                HandleUIClick(rightHitUI);
            }
        }
        
        _isProcessingInput = false;
    }
    
    private void OnYButtonPressed(InputAction.CallbackContext context)
    {
        if (_isProcessingInput || Time.time - _lastInputTime < _inputCooldown)
            return;
            
        _isProcessingInput = true;
        _lastInputTime = Time.time;
        
        // Y버튼으로 팝업 닫기
        if (_isSettingsPopupActive)
        {
            CloseSettingsPopup();
        }
        else if (_isQuitPopupActive)
        {
            CloseQuitPopup();
        }
        
        _isProcessingInput = false;
    }
    
    private void OnLeftTriggerPressed(InputAction.CallbackContext context)
    {
        if (_isProcessingInput || Time.time - _lastInputTime < _inputCooldown)
            return;
            
        _isProcessingInput = true;
        _lastInputTime = Time.time;
        
        // 왼쪽 트리거는 왼쪽 레이만 확인 (프롤로그에서는 작동 안함)
        if (!_isPrologueActive)
        {
            GameObject leftHitUI = GetRayHitUI(_leftRayInteractor);
            if (leftHitUI != null && IsTargetUI(leftHitUI))
            {
                HandleUIClick(leftHitUI);
            }
        }
        
        _isProcessingInput = false;
    }
    
    private void OnRightTriggerPressed(InputAction.CallbackContext context)
    {
        if (_isProcessingInput || Time.time - _lastInputTime < _inputCooldown)
            return;
            
        _isProcessingInput = true;
        _lastInputTime = Time.time;
        
        // 오른쪽 트리거는 오른쪽 레이만 확인 (프롤로그에서는 작동 안함)
        if (!_isPrologueActive)
        {
            GameObject rightHitUI = GetRayHitUI(_rightRayInteractor);
            if (rightHitUI != null && IsTargetUI(rightHitUI))
            {
                HandleUIClick(rightHitUI);
            }
        }
        
        _isProcessingInput = false;
    }
    
    private void HandleUIClick(GameObject clickedUI)
    {
        Debug.Log($"HandleUIClick called with: {clickedUI.name}");
        
        // 팝업이 열려있을 때는 해당 팝업의 UI만 처리
        if (_isSettingsPopupActive)
        {
            HandleSettingsPopupClick(clickedUI);
            return;
        }
        
        if (_isQuitPopupActive)
        {
            HandleQuitPopupClick(clickedUI);
            return;
        }
        
        // 메인 메뉴 UI 처리 (팝업이 없을 때만)
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
        else
        {
            Debug.Log($"No matching UI handler found for: {clickedUI.name}");
        }
    }
    
    private void HandleSettingsPopupClick(GameObject clickedUI)
    {
        // 슬라이더 클릭 시 A버튼/트리거로 조작 가능하도록
        if (_soundSlider != null && (clickedUI == _soundSlider.gameObject || clickedUI.transform.IsChildOf(_soundSlider.transform)))
        {
            Debug.Log("Sound slider button interaction");
            // A버튼/트리거로 슬라이더 값을 오른쪽 레이 위치에 따라 설정
            UpdateSliderFromRaycast();
        }
    }
    
    private void HandleQuitPopupClick(GameObject clickedUI)
    {
        // 메인으로 돌아가기 버튼 (게임 종료)만
        if (_mainBackButton != null && (clickedUI == _mainBackButton.gameObject || clickedUI.transform.IsChildOf(_mainBackButton.transform)))
        {
            Debug.Log("Main back button detected - calling OnMainBackButtonClicked");
            OnMainBackButtonClicked();
        }
    }

    private void OnPlayUIClicked()
    {
        SetPrologueActive(true); // 프롤로그 캔버스로 전환
    }
    
    private void OnSettingsUIClicked()
    {
        Debug.Log("Settings 클릭");
        OpenSettingsPopup();
    }
    
    private void OnQuitUIClicked()
    {
        Debug.Log("Quit 클릭");
        OpenQuitPopup();
    }
    
    private void OpenSettingsPopup()
    {
        if (_settingsPopupUI != null)
        {
            _settingsPopupUI.SetActive(true);
            _isSettingsPopupActive = true;
            
            // 설정 팝업 열 때 현재 볼륨을 슬라이더에 반영
            UpdateSoundSliderValue();
            
            Debug.Log("Settings popup opened");
        }
    }
    
    private void UpdateSoundSliderValue()
    {
        if (_soundSlider != null && DataManager.Data != null)
        {
            // DataManager의 마스터 볼륨 레벨(0~100)을 슬라이더(0~1)에 설정
            float sliderValue = DataManager.Data.GetMasterVolumeLevel() / 100f;
            _soundSlider.value = sliderValue;
            Debug.Log($"Sound slider value updated to: {_soundSlider.value} (Volume Level: {DataManager.Data.GetMasterVolumeLevel()}%)");
            
            // 텍스트도 업데이트
            if (_soundValueText != null)
            {
                _soundValueText.text = DataManager.Data.GetMasterVolumeLevel().ToString() + "%";
            }
        }
    }
    
    private void CloseSettingsPopup()
    {
        if (_settingsPopupUI != null)
        {
            _settingsPopupUI.SetActive(false);
            _isSettingsPopupActive = false;
            Debug.Log("Settings popup closed");
        }
    }
    
    private void OpenQuitPopup()
    {
        if (_quitPopupUI != null)
        {
            _quitPopupUI.SetActive(true);
            _isQuitPopupActive = true;
            Debug.Log("Quit popup opened");
        }
    }
    
    private void CloseQuitPopup()
    {
        if (_quitPopupUI != null)
        {
            _quitPopupUI.SetActive(false);
            _isQuitPopupActive = false;
            Debug.Log("Quit popup closed");
        }
    }
    
    private void OnMainBackButtonClicked()
    {
        Debug.Log("=== OnMainBackButtonClicked START ===");
        
        // 이미 게임 종료 중인지 확인
        if (_isQuittingGame)
        {
            Debug.Log("Already quitting game, ignoring click");
            return;
        }
            
        Debug.Log("Main back button clicked - Quitting game");
        
        // 게임 종료 플래그 설정 (한 번만 실행되도록)
        _isQuittingGame = true;
        
        // Input Actions 비활성화 (게임 종료 전에)
        DisableAllInputActions();
        
        Debug.Log("Attempting to quit game...");
        
        // 짧은 지연 후 게임 종료 (Input System이 정리될 시간을 줌)
        StartCoroutine(QuitGameCoroutine());
        
        Debug.Log("=== OnMainBackButtonClicked END ===");
    }
    
    private System.Collections.IEnumerator QuitGameCoroutine()
    {
        // 0.1초 대기 (Input System 정리 시간)
        yield return new WaitForSeconds(0.1f);
        
        // 게임 종료
        #if UNITY_EDITOR
            Debug.Log("Stopping play mode in editor");
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Debug.Log("Calling Application.Quit()");
            Application.Quit();
        #endif
    }
    
    private void DisableAllInputActions()
    {
        Debug.Log("Disabling all input actions...");
        
        if (_aButton != null)
        {
            _aButton.performed -= OnAButtonPressed;
            _aButton.Disable();
        }
        if (_yButton != null)
        {
            _yButton.performed -= OnYButtonPressed;
            _yButton.Disable();
        }
        if (_leftTrigger != null)
        {
            _leftTrigger.performed -= OnLeftTriggerPressed;
            _leftTrigger.Disable();
        }
        if (_rightTrigger != null)
        {
            _rightTrigger.performed -= OnRightTriggerPressed;
            _rightTrigger.Disable();
        }
        
        Debug.Log("All input actions disabled");
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
        // 슬라이더 이벤트 리스너 해제
        if (_soundSlider != null && _sliderListenerRegistered)
        {
            _soundSlider.onValueChanged.RemoveListener(OnSoundSliderValueChanged);
        }
        
        // OnDestroy에서는 이미 DisableAllInputActions에서 처리했으므로 
        // 중복 해제를 방지하기 위해 체크
        if (!_isQuittingGame)
        {
            if (_aButton != null) 
            {
                _aButton.performed -= OnAButtonPressed;
            }
            if (_yButton != null)
            {
                _yButton.performed -= OnYButtonPressed;
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
}