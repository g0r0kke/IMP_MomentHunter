using UnityEngine;

public class MoveTriggerZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // XR Origin에 "Player" 태그 붙이기
        {
            TutorialManager.Instance?.OnMoveDone(); // 튜토리얼 다음 단계로
            Debug.Log("다음단계로로");
        }
    }
}
