using System.Collections.Generic;
using UnityEngine;

public class TagDebugManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    private List<MissionTargetTag> allTargets = new List<MissionTargetTag>();
    
    private void Start()
    {
        // 씬의 모든 MissionTargetTag 수집 (비활성화된 오브젝트도 포함)
        allTargets.AddRange(FindObjectsByType<MissionTargetTag>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        
        if (showDebugInfo)
        {
            Debug.Log($"씬에서 발견된 MissionTarget 대상 오브젝트 개수: {allTargets.Count}");
            // foreach (var target in allTargets)
            // {
            //     if (target != null)
            //     {
            //         Debug.Log($"- {target.gameObject.name}: {target.GetTargetMissionType()} (활성: {target.gameObject.activeInHierarchy})");
            //     }
            // }
        }
    }
    
    private void OnEnable()
    {
        GameManager.OnMissionStateChanged += OnMissionStateChanged;
    }
    
    private void OnDisable()
    {
        GameManager.OnMissionStateChanged -= OnMissionStateChanged;
    }
    
    private void OnMissionStateChanged(MissionState newState)
    {
        if (showDebugInfo)
        {
            Debug.Log($"=== 미션 상태 변경: {newState} ===");
            
            List<string> missionTargetNames = new List<string>();
            List<string> untaggedNames = new List<string>();
            
            foreach (var target in allTargets)
            {
                if (target != null && target.gameObject.activeInHierarchy)
                {
                    if (target.GetTargetMissionType() == newState)
                    {
                        // 현재 미션에 해당하는 타겟들 (MissionTarget 태그가 될 예정)
                        missionTargetNames.Add(target.gameObject.name);
                    }
                    else
                    {
                        // 현재 미션에 해당하지 않는 타겟들 (Untagged가 될 예정)
                        untaggedNames.Add(target.gameObject.name);
                    }
                }
            }
            
            if (missionTargetNames.Count > 0)
            {
                Debug.Log($"MissionTarget 태그 부착: {string.Join(", ", missionTargetNames)}");
            }
            
            if (untaggedNames.Count > 0)
            {
                Debug.Log($"Untagged로 변경: {string.Join(", ", untaggedNames)}");
            }
            
            Debug.Log($"현재 활성화된 MissionTarget 개수: {missionTargetNames.Count}");
        }
    }
}
