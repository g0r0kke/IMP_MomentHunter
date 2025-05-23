using UnityEngine;
using UnityEngine.InputSystem;

public class PhotoJudge : MonoBehaviour
{
    public InputActionProperty judgeButton;
    public Transform shootOrigin;
    public float maxJudgeDistance = 5f;
    public LayerMask photoTargetLayer;

    private void Update()
    {
        if (judgeButton.action.WasPressedThisFrame())
        {
            PerformRaycastJudge();
        }
    }

    private void PerformRaycastJudge()
    {
        Ray ray = new Ray(shootOrigin.position, shootOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxJudgeDistance, photoTargetLayer))
        {
            if (hit.collider.CompareTag("MissionTarget"))
            {
                Debug.Log("미션 성공: " + hit.collider.name);
            }
            else
            {
                Debug.Log("감지된 오브젝트가 MissionTarget 태그가 아님: " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("감지 실패: 아무 오브젝트도 맞지 않음");
        }
    }
}
