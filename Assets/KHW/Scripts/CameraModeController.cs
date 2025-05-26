using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Grip을 눌러 카메라 UI를 켜고 끄는 컨트롤러
/// </summary>
public class CameraModeController : MonoBehaviour
{
    public GameObject cameraModel;
    public GameObject cameraUI; 

    public InputActionProperty gripAction;   // <XRController>{RightHand}/grip

    public bool IsActive { get; private set; }

    /* ─────────── Input 바인딩 ─────────── */
    void OnEnable()
    {
        gripAction.action.performed += OnGripPressed;   // 눌렀을 때
        gripAction.action.canceled  += OnGripReleased;  // 뗐을 때
        gripAction.action.Enable();
    }
    void OnDisable()
    {
        gripAction.action.performed -= OnGripPressed;
        gripAction.action.canceled  -= OnGripReleased;
        gripAction.action.Disable();
    }

    /* ─────────── 이벤트 핸들러 ─────────── */
    void OnGripPressed(InputAction.CallbackContext _)
    {
        if (IsActive) return;
        IsActive = true;

        // 시각 요소 온
        cameraUI.SetActive(true);
    }

    void OnGripReleased(InputAction.CallbackContext _)
    {
        if (!IsActive) return;
        IsActive = false;

        // UI 끄기
        cameraUI.SetActive(false);
    }
}
