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

    [Header("Narration (선택)")]
    public AudioSource audioSrc;
    public AudioClip narrationClip;

    [Header("Mission 1")]
    public TutorialMission TutorialMission;           // Inspector에서 드래그

    /* ───────── 튜토리얼 시작 ───────── */
    void Start()
    {
        Debug.Log("[튜토리얼 시작]");
        StartCoroutine(RunTutorial());
    }

    IEnumerator RunTutorial()
    {
        // (선택) 내레이션
        if (narrationClip != null && audioSrc != null)
        {
            audioSrc.PlayOneShot(narrationClip);
            yield return new WaitForSeconds(narrationClip.length);
        }

        Current = Step.Teleport;
        teleportUI.SetActive(true);
        Debug.Log("[1단계] move 시작");
    }

    /* ───────── 단계별 콜백 ───────── */
    public void OnMoveDone()
    {
        if (Current != Step.Move) return;

        moveUI.SetActive(false);
        Debug.Log("조이스틱 이동 완료");

        Current = Step.Teleport;
        teleportUI.SetActive(true);
        Debug.Log("[2단계] 텔레포트 시작");
    }

    public void OnTeleportDone()
    {
        if (Current != Step.Teleport) return;

        teleportUI.SetActive(false);
        Debug.Log("텔레포트 완료");

        Current = Step.GrabLeft;
        leftGrabUI.SetActive(true);
        Debug.Log("[3단계] 왼손 그립 시작");
    }

    public void OnLeftGrabDone()
    {
        if (Current != Step.GrabLeft) return;

        leftGrabUI.SetActive(false);
        Debug.Log("왼손 그립 완료");

        Current = Step.GrabRight;
        rightGrabUI.SetActive(true);
        Debug.Log("[4단계] 오른손 그립 시작");
    }

    public void OnRightGrabDone()
    {
        if (Current != Step.GrabRight) return;

        rightGrabUI.SetActive(false);
        Debug.Log("오른손 그립 완료");

        Current = Step.TakePhoto;
        photoUI.SetActive(true);
        Debug.Log("[5단계] 사진 촬영 시작");
    }

    /* PhotoCaptureAndJudge 에서 호출 */
    public void OnTutorialPhotoTaken()
    {
        if (Current != Step.TakePhoto) return;
        photoUI.SetActive(false);

        /* ── 튜토리얼 끝 → 미션1 시작 ── */
        Debug.Log("미션시작");
        Current = Step.Mission1;
        TutorialMission.BeginMission();
    }

    /* HandsetMission 에서 호출 */
    public void OnMission1Complete()
    {
        if (Current != Step.Mission1) return;
        Current = Step.AllDone;

        Debug.Log("< 완료 >");
        // 필요하면 씬 전환 등 추가
    }
}
