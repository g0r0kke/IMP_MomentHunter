using UnityEngine;
using UnityEngine.InputSystem;

public class CameraModeController : MonoBehaviour
{
    public Transform cameraIdlePosition; // Grip 안 누른 상태 위치
    public Transform cameraActivePosition; // Grip 눌렀을 때 위치

    public GameObject cameraModel; // 카메라 오브젝트
    public InputActionProperty gripButton;

    public GameObject cameraUIRoot;

    private bool isCameraActive = false;

    void Update()
    {
        float gripValue = gripButton.action.ReadValue<float>();

        if (gripValue > 0.5f && !isCameraActive)
        {
            EnterCameraMode();
        }
        else if (gripValue <= 0.5f && isCameraActive)
        {
            ExitCameraMode();
        }
    }

    void EnterCameraMode()
    {
        isCameraActive = true;
        cameraModel.transform.position = cameraActivePosition.position;
        cameraModel.transform.rotation = cameraActivePosition.rotation;
        // TODO: 애니메이션이나 사운드 추가 가능

        if (cameraUIRoot != null)
            cameraUIRoot.SetActive(true);
    }

    void ExitCameraMode()
    {
        isCameraActive = false;
        cameraModel.transform.position = cameraIdlePosition.position;
        cameraModel.transform.rotation = cameraIdlePosition.rotation;

        if (cameraUIRoot != null)
            cameraUIRoot.SetActive(false);
    }
}
