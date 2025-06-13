using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    // Tutorial steps
    public enum Step { None, Teleport, Move, GrabLeft, GrabRight, TakePhoto, AllDone }
    public Step Current { get; private set; }   // Current tutorial step
    
    public static TutorialManager Instance;   // Singleton instance
    void Awake() => Instance = this;
    


    [Header("Step UI")]
    public GameObject teleportUI;
    public GameObject moveUI;
    public GameObject leftGrabUI;
    public GameObject rightGrabUI;
    public GameObject photoUI;
    public GameObject tutorialCanvas;

    [Header("Trigger Zones")]
    public GameObject moveTriggerZoneObject;
    public GameObject teleportTriggerZoneObject;
    public GameObject leftGrabDetectorObject;

    [Header("Success Sound")]
    public AudioSource audioSrc;        
    public AudioClip successClip;
  

    void Start()
    {
        StartCoroutine(RunTutorial());
    }

    // Start tutorial flow
    IEnumerator RunTutorial()
    {
        // Step 1: Move
        Current = Step.Move;
        moveTriggerZoneObject.SetActive(true);
        moveUI.SetActive(true);

        yield return null;
    }

    // Called when Move is completed
    public void OnMoveDone()
    {
        if (Current != Step.Move) return;

        // Hide move step UI
        moveTriggerZoneObject.SetActive(false);
        moveUI.SetActive(false);

        audioSrc.PlayOneShot(successClip);  // Play success sound

        // Step 2: Teleport
        Current = Step.Teleport;
        teleportTriggerZoneObject.SetActive(true);
        teleportUI.SetActive(true);
    }

    // Called when Teleport is completed
    public void OnTeleportDone()
    {
        if (Current != Step.Teleport) return;

        // Hide teleport step UI
        teleportTriggerZoneObject.SetActive(false);
        teleportUI.SetActive(false);

        audioSrc.PlayOneShot(successClip);  // Play success sound

        // Step 3: Left grab
        Current = Step.GrabLeft;
        leftGrabDetectorObject.SetActive(true);
        leftGrabUI.SetActive(true);
    }

    // Called when Left grab is completed
    public void OnLeftGrabDone()
    {
        if (Current != Step.GrabLeft) return;

        // Hide left grab step UI
        leftGrabDetectorObject.SetActive(false);
        leftGrabUI.SetActive(false);

        audioSrc.PlayOneShot(successClip);   // Play success sound

        // Step 4: Right grab
        Current = Step.GrabRight;
        rightGrabUI.SetActive(true);
    }

    // Called when Right grab is completed
    public void OnRightGrabDone()
    {
        if (Current != Step.GrabRight) return;

        // Hide right grab step UI
        rightGrabUI.SetActive(false);

        audioSrc.PlayOneShot(successClip);   // Play success sound

        // Step 5: Take photo
        Current = Step.TakePhoto;
        photoUI.SetActive(true);
    }

    // Called by PhotoCaptureAndJudge
    public void OnTutorialPhotoTaken()
    {
        if (Current != Step.TakePhoto) return;

        photoUI.SetActive(false);   // Hide photo step UI
        audioSrc.PlayOneShot(successClip);   // Play success sound

        // All done
        Current = Step.AllDone;
        tutorialCanvas.SetActive(false);   // Hide tutorial canvas
        GameManager.Instance.SetNextMissionState();   // Notify GameManager
    }
}
