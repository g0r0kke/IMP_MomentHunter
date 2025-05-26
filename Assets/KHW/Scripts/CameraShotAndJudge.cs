using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class CameraShotAndJudge : MonoBehaviour
{
    public InputActionProperty triggerButton;

    public Transform shootOrigin;
    public float maxJudgeDistance = 5f;
    public LayerMask photoTargetLayer;

    [Header("Save")]
    public bool savePhotoToFile = true;
    public string saveFolder = "Photos";

    void Start()
    {
        if (savePhotoToFile)
        {
            string path = Path.Combine(Application.persistentDataPath, saveFolder);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            saveFolder = path;                       // 퀘스트 빌드 대비
        }
    }

    void OnEnable()  => triggerButton.action.Enable();
    void OnDisable() => triggerButton.action.Disable();

    void Update()
    {
        if (triggerButton.action.WasPressedThisFrame())
        {
            CaptureScreenshot();
            PerformRaycastJudge();
        }
    }

    void CaptureScreenshot()
    {
        if (!savePhotoToFile) return;
        string file = Path.Combine(saveFolder, $"Photo_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        ScreenCapture.CaptureScreenshot(file);
        Debug.Log($"저장됨 → {file}");
    }

    void PerformRaycastJudge()
    {
        Ray ray = new Ray(shootOrigin.position, shootOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxJudgeDistance, photoTargetLayer))
        {
            bool success = hit.collider.CompareTag("MissionTarget");
            Debug.Log(success ? $"미션 성공: {hit.collider.name}"
                              : $"MissionTarget 아님: {hit.collider.name}");
            // TODO : 성공 시 점수·UI·사운드 트리거
        }
        else
        {
            Debug.Log("감지 실패 (히트 없음)");
        }
    }
}
