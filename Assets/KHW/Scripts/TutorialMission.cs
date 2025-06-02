using UnityEngine;
using System.Collections;

public class TutorialMission : MonoBehaviour
{
    public TutorialManager tutorialManager;
    [Header("Target")]
    public Collider handsetCollider;          // 수화기 오브젝트의 Collider

    [Header("UI & 오디오")]
    public GameObject guideUI;                // “수화기를 찍으세요” 패널
    public AudioSource audioSrc;
    public AudioClip momVoiceClip;

    bool waiting = false;

    /* 튜토리얼 매니저가 호출 */
    public void BeginMission()
    {
        guideUI.SetActive(true);
        handsetCollider.gameObject.tag = "MissionTarget";   // 판정용
        waiting = true;
    }

    /* PhotoCaptureAndJudge의 Raycast 성공 시 호출 */
    public void OnHandsetPhotoSuccess()
    {
        if (!waiting) return;
        waiting = false;
        StartCoroutine(VoiceAndFinish());
    }

    IEnumerator VoiceAndFinish()
    {
        guideUI.SetActive(false);
        handsetCollider.gameObject.tag = "Untagged";

        if (momVoiceClip != null && audioSrc != null)
        {
            audioSrc.PlayOneShot(momVoiceClip);
            yield return new WaitForSeconds(momVoiceClip.length);
        }

        tutorialManager?.OnMission1Complete();
    }
}

