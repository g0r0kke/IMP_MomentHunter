using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class CameraTrigger : MonoBehaviour
{
    public InputActionProperty triggerButton;
    public bool savePhotoToFile = true;
    public string saveFolder = "Photos";

    private void Start()
    {
        if (savePhotoToFile && !Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }
    }

    private void Update()
    {
        if (triggerButton.action.WasPressedThisFrame())
        {
            CaptureScreenshot();
        }
    }

    private void CaptureScreenshot()
    {
        string filename = Path.Combine(saveFolder, $"Photo_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        ScreenCapture.CaptureScreenshot(filename);
        Debug.Log("사진 저장됨: " + filename);
    }
}
