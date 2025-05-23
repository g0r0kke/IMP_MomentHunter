using UnityEngine;
using UnityEngine.InputSystem;

public class CameraModeController : MonoBehaviour
{
    public Transform cameraIdlePos;
    public Transform cameraActivePos;
    public GameObject cameraModel;
    public GameObject cameraUIRoot;
    public InputActionProperty gripButton;

    bool isActive = false;

    void Update()
    {
        if (gripButton.action.ReadValue<float>() > 0.5f && !isActive)
        {
            isActive = true;
            cameraModel.transform.SetPositionAndRotation(cameraActivePos.position, cameraActivePos.rotation);
            cameraUIRoot.SetActive(true);
        }
        else if (gripButton.action.ReadValue<float>() <= 0.5f && isActive)
        {
            isActive = false;
            cameraModel.transform.SetPositionAndRotation(cameraIdlePos.position, cameraIdlePos.rotation);
            cameraUIRoot.SetActive(false);
        }
    }
}
