using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public enum Step { None, Teleport, Move, Grab, TakePhoto, Mission1, AllDone }
    public Step Current { get; private set; }
    
    public static TutorialManager Instance;
    void Awake() => Instance = this;

    [Header("Step UI")]
    public GameObject teleportUI;
    public GameObject moveUI;
    public GameObject grabUI;
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
        Debug.Log("[1단계] 텔레포트 시작");
    }

    /* ───────── 단계별 콜백 ───────── */
    public void OnTeleportDone()

    {
        Debug.Log("OnTeleportDone() 호출됨");
        if (Current != Step.Teleport) return;
        teleportUI.SetActive(false);
        Debug.Log("텔레포트 완료");

        Current = Step.Move;
        moveUI.SetActive(true);
        Debug.Log("[2단계] 조이스틱 이동 시작");
    }

    public void OnMoveDone()
    {
        if (Current != Step.Move) return;
        moveUI.SetActive(false);
        Debug.Log(" 조이스틱 이동 완료");

        Current = Step.Grab;
        grabUI.SetActive(true);
        Debug.Log("[3단계] 카메라 잡기 시작");
    }

    public void OnGrabDone()
    {
        if (Current != Step.Grab) return;
        grabUI.SetActive(false);

        Current = Step.TakePhoto;
        photoUI.SetActive(true);
        Debug.Log("[4단계] 시작");
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
