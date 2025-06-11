using UnityEngine;
using System.Collections;

public class TutorialMission : MonoBehaviour
{
    [Header("Target")]
    public Collider cupCollider;
    [Header("UI & 오디오")]
    public GameObject Mission1Clear;      


    /* 튜토리얼 매니저가 호출 */
    public void BeginMission()
    {
        
        if (cupCollider) cupCollider.gameObject.tag = "MissionTarget";
    }

    public void CompleteMission()
    {
        StartCoroutine(FinishRoutine());
    }

    IEnumerator FinishRoutine()
    {
        Mission1Clear.SetActive(true);
        yield return null;
    }
}

