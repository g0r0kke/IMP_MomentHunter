using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class PhotoCaptureAndJudge : MonoBehaviour
{
    public TutorialMission tutorialMission;
    public bool useTutorial = true;
    public CanvasGroup flashCanvasGroup;
    public float flashDuration = 0.2f;

    [Header("Input")]
    public InputActionProperty triggerButton;   // 오른손 트리거

    [Header("Camera / Distance")]
    public Camera captureCam;                   // 카메라(대개 MainCamera)
    public float maxJudgeDistance = 5f;
    public LayerMask TargetLayer;          // PhotoTarget 레이어만 체크
    public int requiredTargets = 2;

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

    void PlayShutterEffect()
    {
        // 사운드
        //if (shutterAudioSrc != null && shutterClip != null)
        //{
        //    shutterAudioSrc.PlayOneShot(shutterClip);
        //}

        // 플래시 코루틴 실행
        if (flashCanvasGroup != null)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    IEnumerator FlashRoutine()
    {
        float fadeInDuration = flashDuration * 0.3f;   
        float holdDuration   = flashDuration * 0.2f;   
        float fadeOutDuration = flashDuration * 0.5f;  

        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            flashCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }
        flashCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(holdDuration);

        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            flashCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }
        flashCanvasGroup.alpha = 0f;
    }

    void OnEnable()  => triggerButton.action.Enable();
    void OnDisable() => triggerButton.action.Disable();

    void Update()
    {
        if (triggerButton.action.WasPressedThisFrame())
        {
            PlayShutterEffect();
            CaptureScreenshot();       // 1) 사진 저장
            JudgeMultipleTargets();  // 2) 즉시 판정

            if (useTutorial &&
               TutorialManager.Instance != null &&
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

        /* ---------------- 다중 타겟 판정 ---------------- */
        void JudgeMultipleTargets()
        {
            // 1) 카메라 주변 OverlapSphere 로 후보 수집
            Vector3 camPos = captureCam.transform.position;
            Collider[] hits = Physics.OverlapSphere(camPos, maxJudgeDistance, TargetLayer);

            int visibleCount = 0;
            foreach (Collider col in hits)
            {
                if (!col.CompareTag("MissionTarget")) continue; // 태그 필터

                if (IsInView(col.transform))
                {
                    visibleCount++;
    #if UNITY_EDITOR
                    Debug.Log($"뷰포트 + 거리 통과: {col.name}");
    #endif
                }
            }

            if (visibleCount >= requiredTargets)
            {
                Debug.Log($"미션 성공! ({visibleCount}/{requiredTargets})");
                tutorialMission?.OnHandsetPhotoSuccess();
            }
            else
            {
                Debug.Log($"미션 실패: 조건 통과 {visibleCount}/{requiredTargets}");
            }
        }

        // 카메라 뷰 안(0~1) + 정면(Z>0)인지 검사
        bool IsInView(Transform target)
        {
            Vector3 vPos = captureCam.WorldToViewportPoint(target.position);
            return vPos.z > 0f && vPos.x is >= 0f and <= 1f && vPos.y is >= 0f and <= 1f;
        }

        //카메라 → 타겟 라인에 가로막는 물체가 없는지 검사
        bool IsVisible(Vector3 camPos, Collider targetCol)
        {
            Vector3 targetCenter = targetCol.bounds.center;
            Vector3 dir          = targetCenter - camPos;

            // Linecast로 가장 먼저 맞는 콜라이더가 본인인가?
            return !Physics.Linecast(camPos, targetCenter, out RaycastHit hit, ~0)
                || hit.collider == targetCol;
        }

}
