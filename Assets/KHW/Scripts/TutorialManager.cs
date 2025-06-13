using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public enum Step { None, Teleport, Move, GrabLeft, GrabRight, TakePhoto, Mission1, AllDone }
    public Step Current { get; private set; }
    
    public static TutorialManager Instance;
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
        Debug.Log("[튜토리얼 시작]");
        StartCoroutine(RunTutorial());
    }

    IEnumerator RunTutorial()
    {
        Current = Step.Move;
        moveTriggerZoneObject.SetActive(true);
        moveUI.SetActive(true);
        Debug.Log("[1단계] move 시작");

        yield return null;
    }

    public void OnMoveDone()
    {
        if (Current != Step.Move) return;

        moveTriggerZoneObject.SetActive(false);
        moveUI.SetActive(false);
        Debug.Log("조이스틱 이동 완료");

        audioSrc.PlayOneShot(successClip);

        Current = Step.Teleport;
        teleportTriggerZoneObject.SetActive(true);
        teleportUI.SetActive(true);
        Debug.Log("[2단계] 텔레포트 시작");
    }

    public void OnTeleportDone()
    {
        if (Current != Step.Teleport) return;

        teleportTriggerZoneObject.SetActive(false);
        teleportUI.SetActive(false);

        audioSrc.PlayOneShot(successClip);

        /* 3. 왼손 그립 학습 */
        Current = Step.GrabLeft;
        leftGrabDetectorObject.SetActive(true);  
        leftGrabUI.SetActive(true);
        Debug.Log("[3단계] 왼손 그립 시작 (미션1 진행 중)");
    }

    public void OnLeftGrabDone()
    {
        if (Current != Step.GrabLeft) return;

        leftGrabDetectorObject.SetActive(false);
        leftGrabUI.SetActive(false);

        audioSrc.PlayOneShot(successClip);

        /* 4. 오른손 그립 + 카메라 모드 진입 */
        Current = Step.GrabRight;
        rightGrabUI.SetActive(true);
        Debug.Log("[4단계] 오른손 그립 + 카메라 모드 진입 시작 (미션1 진행 중)");
    }

    public void OnRightGrabDone()
    {
        if (Current != Step.GrabRight) return;

        rightGrabUI.SetActive(false);

        audioSrc.PlayOneShot(successClip);

        /* 5. 사진 촬영 */
        Current = Step.TakePhoto;
        photoUI.SetActive(true);
        Debug.Log("[5단계] 사진 촬영 시작 (미션1 진행 중)");
    }

    /* PhotoCaptureAndJudge 에서 호출 */
    public void OnTutorialPhotoTaken()
    {
        if (Current != Step.TakePhoto) return;

        photoUI.SetActive(false);

        audioSrc.PlayOneShot(successClip);

        Current = Step.AllDone;
        Debug.Log("< 튜토리얼 + 미션1 완료 >");
        tutorialCanvas.SetActive(false);
        GameManager.Instance.SetNextMissionState();
    }
}
