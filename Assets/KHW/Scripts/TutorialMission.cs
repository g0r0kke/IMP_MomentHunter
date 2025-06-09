using UnityEngine;
using System.Collections;

public class TutorialMission : MonoBehaviour
{
    [Header("Target")]
    public Collider cupCollider;
    [Header("UI & 오디오")]
    public GameObject guideUI;               
    public AudioSource audioSrc;
    public AudioClip momVoiceClip;


    /* 튜토리얼 매니저가 호출 */
    public void BeginMission()
    {
        // guideUI.SetActive(true);
        
        if (cupCollider) cupCollider.gameObject.tag = "MissionTarget";
    }

    public void CompleteMission()
    {
        StartCoroutine(FinishRoutine());
    }

    IEnumerator FinishRoutine()
    {
        // guideUI.SetActive(false);
        if (cupCollider) cupCollider.gameObject.tag = "Untagged";

        if (momVoiceClip && audioSrc)
        {
            audioSrc.PlayOneShot(momVoiceClip);
            yield return new WaitForSeconds(momVoiceClip.length);
        }
    }
}

