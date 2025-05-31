using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class PhotoCaptureAndJudge : MonoBehaviour
{
    public TutorialMission tutorialMission;

    [Header("Input")]
    public InputActionProperty triggerButton;   // 오른손 트리거

    [Header("Ray / Viewport")]
    public Camera captureCam;                   // 카메라(대개 MainCamera)
    public float maxJudgeDistance = 5f;
    public LayerMask TargetLayer;          // PhotoTarget 레이어만 체크

    [Header("Save")]
    public bool savePhotoToFile = true;
    public string saveFolder = "Photos";

    void Start()
    {
        if (savePhotoToFile)
        {
            string path = Path.Combine(Application.persistentDataPath, saveFolder);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            saveFolder = path;   // 퀘스트 빌드 대비
        }
    }

    void OnEnable()  => triggerButton.action.Enable();
    void OnDisable() => triggerButton.action.Disable();

    void Update()
    {
        if (triggerButton.action.WasPressedThisFrame())
        {
            CaptureScreenshot();       // 1) 사진 저장
            JudgeByViewportRaycast();  // 2) 즉시 판정

            // 튜토리얼 상태가 TakePhoto일 경우 → 다음 단계로 넘김
            if (TutorialManager.Instance != null &&
                TutorialManager.Instance.Current == TutorialManager.Step.TakePhoto)
            {
                TutorialManager.Instance.OnTutorialPhotoTaken();
            }
        }
    }

    /* ----------------------- 사진 저장 ----------------------- */
    void CaptureScreenshot()
    {
        if (!savePhotoToFile) return;
        string file = Path.Combine(saveFolder,
                      $"Photo_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        ScreenCapture.CaptureScreenshot(file);
        Debug.Log($"사진 저장: {file}");
    }

    /* ---------------------- 뷰포트 판정 ---------------------- */
    void JudgeByViewportRaycast()
{
    Ray ray = captureCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

    if (Physics.Raycast(ray, out RaycastHit hit, maxJudgeDistance, TargetLayer))
    {
        Debug.DrawRay(ray.origin, ray.direction * maxJudgeDistance, Color.red, 2f);

        if (hit.collider.CompareTag("MissionTarget"))
            {
                Debug.Log($"미션 성공: {hit.collider.name}");
                tutorialMission?.OnHandsetPhotoSuccess();   // ← 직접 호출
            }
            else
            {
                Debug.Log($"MissionTarget 태그 아님: {hit.collider.name}");
            }
    }
    else
    {
        Debug.Log("판정 실패: 히트 없음");
    }
}
}
