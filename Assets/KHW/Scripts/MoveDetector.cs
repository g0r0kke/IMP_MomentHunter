using UnityEngine;

public class MoveTriggerZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // XR Origin에 "Player" 태그 붙이기
        {
            Debug.Log("[2단계] 조이스틱 이동 성공");
            TutorialManager.Instance?.OnMoveDone(); // 튜토리얼 다음 단계로
        }
    }
}
