using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class CameraShotAndJudge : MonoBehaviour
{
    [Header("Trigger Action")]
    public InputActionProperty triggerButton;

    [Header("카메라 위치 정보")]
    public Transform shootOrigin; // 카메라에서 Ray 쏘는 위치 (카메라 렌즈 기준)

    [Header("스크린샷 저장")]
    public bool savePhotoToFile = true;
    public string saveFolder = "Photos";

    [Header("판정 조건")]
    public float maxJudgeDistance = 5f;
    public LayerMask photoTargetLayer;

    [Header("디버그")]
    public bool showDebugRay = true;

    private void Start()
    {
        if (savePhotoToFile && !Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);
    }

    void Update()
    {
        if (triggerButton.action.WasPressedThisFrame())
        {
            TakePhoto();
            JudgePhoto();
        }
    }

    void TakePhoto()
    {
        if (!savePhotoToFile) return;

        string filename = Path.Combine(saveFolder, $"Photo_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        ScreenCapture.CaptureScreenshot(filename);

        Debug.Log("사진 저장 완료: " + filename);
    }

    void JudgePhoto()
    {
        Ray ray = new Ray(shootOrigin.position, shootOrigin.forward);
        RaycastHit hit;

        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * maxJudgeDistance, Color.red, 1f);
        }

        if (Physics.Raycast(ray, out hit, maxJudgeDistance, photoTargetLayer))
        {
            GameObject target = hit.collider.gameObject;

            if (target.CompareTag("MissionTarget"))
            {
                Debug.Log("✅ 미션 성공! 대상 감지됨: " + target.name);
                // TODO: 미션 성공 처리 (GameManager 통보 등)
            }
            else
            {
                Debug.Log("감지된 오브젝트는 있지만 MissionTarget 태그 아님: " + target.name);
            }
        }
        else
        {
            Debug.Log("미션 대상 없음 (Ray가 아무것도 안 맞음)");
        }
    }
}
