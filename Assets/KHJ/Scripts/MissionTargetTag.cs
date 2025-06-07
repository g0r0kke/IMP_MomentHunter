using UnityEngine;

public class MissionTargetTag : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private MissionState targetMissionType = MissionState.None;
    
    private const string MISSION_TARGET_TAG = "MissionTarget";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 현재 미션 상태 확인해 초기 태그 설정
        if (GameManager.Instance != null)
        {
            OnMissionStateChanged(GameManager.Instance.MissionState);
        }
    }
    
    private void OnEnable()
    {
        // 이벤트 구독
        GameManager.OnMissionStateChanged += OnMissionStateChanged;
    }
    
    private void OnDisable()
    {
        // 이벤트 구독 해제
        GameManager.OnMissionStateChanged -= OnMissionStateChanged;
    }

    // 미션 상태 변경 이벤트 처리
    private void OnMissionStateChanged(MissionState newMissionState)
    {
        if (targetMissionType == newMissionState)
        {
            // 현재 미션에 해당하는 타겟이라면 MissionTarget 태그 부착
            SetMissionTargetTag(true);
        }
        else
        {
            // 현재 미션에 해당하지 않는다면 Untagged로 변경
            SetMissionTargetTag(false);
        }
    }
    
    private void SetMissionTargetTag(bool isMissionTarget)
    {
        if (isMissionTarget)
        {
            gameObject.tag = MISSION_TARGET_TAG;
            // Debug.Log($"{gameObject.name}: MissionTarget 태그 부착 (Type: {targetMissionType})");
        }
        else
        {
            gameObject.tag = "Untagged";
            // Debug.Log($"{gameObject.name}: Untagged로 변경");
        }
    }
    
    // 현재 설정된 미션 타입 확인
    public MissionState GetTargetMissionType()
    {
        return targetMissionType;
    }
}
