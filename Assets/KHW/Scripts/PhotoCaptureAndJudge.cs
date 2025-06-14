using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Collections;
using UnityEngine.UI;

public class PhotoCaptureAndJudge : MonoBehaviour
{
    [Header("Tutorial Settings")]
    public bool useTutorial = true;   // Use tutorial mode

    [Header("Flash Effect")]
    public CanvasGroup flashCanvasGroup;   // Flash effect
    public float flashDuration = 0.2f;

    [Header("Audio")]
    public AudioClip shutterClip;   // Shutter sound
    public float startTimeInSeconds = 0f;   // Shutter start time
    public float playDuration = 2f;    // Shutter play duration
    private AudioSource audioSource;

    [Header("UI References")]
    public GameObject DisplayCanvas;   // Photo display canvas
    public float photoDisplayDuration = 5f;
    public GameObject CameraFrame;
    public GameObject tutorialCanvas;
    public GameObject cameraFrameControl;

    [Header("Render Target")]
    public RenderTexture captureRT;

    [Header("Input")]
    public InputActionProperty triggerButton;   // Right hand trigger

    [Header("Camera Settings")]
    public Camera captureCam;   // Capture camera
    public float maxJudgeDistance = 5f;   // Max judge distance
    public LayerMask TargetLayer;   // Layer to detect

    [Header("Save Settings")]
    public bool savePhotoToFile = true;
    public string saveFolder = "Photos";   // Folder path

    void Start()
    {
        if (savePhotoToFile)
        {
            string path = Path.Combine(Application.persistentDataPath, saveFolder);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            saveFolder = path;   // For Quest build
        }

        audioSource = GetComponent<AudioSource>();   // Get audio source
    }

    // Play shutter sound and flash effect
    void PlayShutterEffect()
    {
        // Play sound
        if (shutterClip != null && audioSource != null)
        {
            audioSource.clip = shutterClip;
            audioSource.time = startTimeInSeconds; 
            audioSource.Play();

            // Stop sound after delay
            StartCoroutine(StopAudioAfterSeconds(playDuration));
        }

        // Play flash effect
        if (flashCanvasGroup != null)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    // Stop audio after duration
    IEnumerator StopAudioAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        audioSource.Stop();
    }

    // Flash UI fade in/out
    IEnumerator FlashRoutine()
    {
        float fadeInDuration = flashDuration * 0.3f;
        float holdDuration = flashDuration * 0.2f;
        float fadeOutDuration = flashDuration * 0.5f;

        // Fade in
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            flashCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }
        flashCanvasGroup.alpha = 1f;

        // Hold
        yield return new WaitForSeconds(holdDuration);

        // Fade out
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            flashCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }
        flashCanvasGroup.alpha = 0f;
    }

    void OnEnable() => triggerButton.action.Enable();   // Enable input
    void OnDisable() => triggerButton.action.Disable();   // Disable input

    void Update()
    {
        // Only if camera mode active
        if (CameraModeController.Instance != null && CameraModeController.Instance.IsActive)
        {
            // On trigger press
            if (triggerButton.action.WasPressedThisFrame())
            {
                if (DataManager.Data.CurrentHealth > 0)
                {
                    PlayShutterEffect();   // Play sound & flash
                    StartCoroutine(CaptureAndShowPhoto());   // Capture photo

                    // Judge targets
                    int count = JudgeMultipleTargets();
                    // Debug.Log($"pass target count : {count}");
                    
                    // KHJ: Send count to GameManager
                    if (GameManager.Instance)
                    {
                        GameManager.Instance.SetMissionObjectCount(count);
                    }
                }
                else
                {
                    if (DataManager.Data)
                    {
                        DataManager.Data.UseHealth();
                    }
                }
                

                // Tutorial check
                if (useTutorial &&
                   TutorialManager.Instance != null &&
                   TutorialManager.Instance.Current == TutorialManager.Step.TakePhoto)
                {
                    TutorialManager.Instance.OnTutorialPhotoTaken();
                }
            }
        }
    }

    // Show captured photo on screen
    IEnumerator CaptureAndShowPhoto()
    {
        // Enable capture cam
        captureCam.enabled = true;
        captureCam.targetTexture = captureRT;
        captureCam.Render();

        // Read pixels to Texture2D
        RenderTexture.active = captureRT;
        Texture2D tex = new Texture2D(captureRT.width, captureRT.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, captureRT.width, captureRT.height), 0, 0);
        tex.Apply();

        // Disable capture cam
        captureCam.enabled = false;
        RenderTexture.active = null;

        // Save to file
        if (savePhotoToFile)
        {
            string file = Path.Combine(saveFolder,
                $"Photo_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
            File.WriteAllBytes(file, tex.EncodeToPNG());
        }

        // Show captured photo in UI
        if (DisplayCanvas != null)
        {
            var photoRawImage = DisplayCanvas.transform.Find("CapturedPhoto").GetComponent<RawImage>();
            if (photoRawImage != null)
            {
                photoRawImage.texture = tex;
            }

            // Hide other UI
            GameManager.Instance.SetMainCanvasActive(false);
            tutorialCanvas.gameObject.SetActive(false);
            cameraFrameControl.gameObject.SetActive(false);

            // Show display
            DisplayCanvas.gameObject.SetActive(true);  

            // Wait display time
            yield return new WaitForSeconds(photoDisplayDuration);

            // Restore UI
            GameManager.Instance.SetMainCanvasActive(true);
            DisplayCanvas.gameObject.SetActive(false);  
            tutorialCanvas.gameObject.SetActive(true);
            cameraFrameControl.gameObject.SetActive(true);

        }
    }

    // Judge multiple targets
    int JudgeMultipleTargets()
    {
        Vector3 camPos = captureCam.transform.position;

        // Find nearby colliders
        Collider[] hits = Physics.OverlapSphere(camPos, maxJudgeDistance, TargetLayer);

        int visibleCount = 0;
        
        // Check each collider
        foreach (Collider col in hits)
        {
            if (!col.CompareTag("MissionTarget")) continue;   // Check tag

            // Check if in camera view
            if (IsInView(col.transform))
            {
                visibleCount++;
            // #if UNITY_EDITOR
            //     Debug.Log($"pass target : {col.name}");
            // #endif
            }
        }

        return visibleCount; // Return visible target count
    }

    // Check if target is in camera view and in front
    bool IsInView(Transform target)
    {
        Vector3 vPos = captureCam.WorldToViewportPoint(target.position);
        // Check Z>0 and viewport inside (0~1)
        return vPos.z > 0f && vPos.x is >= 0f and <= 1f && vPos.y is >= 0f and <= 1f;
    }

}