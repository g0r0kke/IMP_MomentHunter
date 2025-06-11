using System.Collections;
using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class MissionText : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text _healthText;
    
    [Header("Mission UI Objects")]
    [SerializeField] private GameObject _missionBG;          // MissionBG 오브젝트
    [SerializeField] private GameObject _pin;                // Pin 오브젝트
    [SerializeField] private GameObject _missionTitle;       // MissionTitle 오브젝트
    [SerializeField] private GameObject _missionContents;    // MissionContents 오브젝트
    
    [Header("Mission GameObject Arrays")]
    [SerializeField] private GameObject[] _missionTitleObjects;    // 미션 타이틀 게임오브젝트들
    [SerializeField] private GameObject[] _missionContentObjects; // 미션 콘텐츠 게임오브젝트들
    
    [Header("Feedback GameObject Array")]
    [SerializeField] private GameObject[] _feedbackObjects;        // 피드백 게임오브젝트들
    
    private void Start()
    {
        UpdateMissionText();
        
        if (!DataManager.Data) return;
        UpdateHealthText(DataManager.Data.CurrentHealth);
    }
    
    // 인덱스로 게임오브젝트 활성화/비활성화
    public void ActivateMissionObjects(int index)
    {
        // 모든 미션 타이틀 오브젝트 비활성화
        if (_missionTitleObjects != null)
        {
            for (int i = 0; i < _missionTitleObjects.Length; i++)
            {
                if (_missionTitleObjects[i])
                {
                    _missionTitleObjects[i].SetActive(i == index);
                }
            }
        }
        
        // 모든 미션 콘텐츠 오브젝트 비활성화
        if (_missionContentObjects != null)
        {
            for (int i = 0; i < _missionContentObjects.Length; i++)
            {
                if (_missionContentObjects[i])
                {
                    _missionContentObjects[i].SetActive(i == index);
                }
            }
        }
    }
    
    // 피드백 오브젝트 활성화/비활성화
    public void ActivateFeedbackObject(int index)
    {
        if (_feedbackObjects == null) return;
        
        // 모든 피드백 오브젝트 비활성화 후 지정된 인덱스만 활성화
        for (int i = 0; i < _feedbackObjects.Length; i++)
        {
            if (_feedbackObjects[i])
            {
                _feedbackObjects[i].SetActive(i == index);
            }
        }
    }
    
    public void UpdateMissionText()
    {
        if (!GameManager.Instance) return;
        
        MissionState currentState = GameManager.Instance.MissionState;
        
        // None, Tutorial, Ending 상태에서는 미션 UI 숨기기
        if (currentState == MissionState.None || currentState == MissionState.Ending)
        {
            SetMissionUIActive(false);
        }
        else
        {
            SetMissionUIActive(true);
            int objectIndex = GetObjectIndex(currentState);
            ActivateMissionObjects(objectIndex);
        }
    }

    // 미션 UI 전체 활성화/비활성화
    private void SetMissionUIActive(bool isActive)
    {
        if (_missionBG) _missionBG.SetActive(isActive);
        if (_pin) _pin.SetActive(isActive);
        if (_missionTitle) _missionTitle.SetActive(isActive);
        if (_missionContents) _missionContents.SetActive(isActive);
    }
    
    public void UpdateHealthText(int health)
    {
        if (!DataManager.Data) return;

        _healthText.text = DataManager.Data.CurrentHealth + " / "  + DataManager.Data.MaxHealth;
    }
    
    private int GetObjectIndex(MissionState missionState)
    {
        return missionState switch
        {
            MissionState.Mission1 => 0,    // 첫 번째 게임오브젝트
            MissionState.Mission2 => 1,    // 두 번째 게임오브젝트
            MissionState.Mission3 => 2,    // 세 번째 게임오브젝트
            MissionState.Mission4 => 3,    // 네 번째 게임오브젝트
            MissionState.Mission5 => 4,    // 다섯 번째 게임오브젝트
            MissionState.Mission6 => 5,    // 여섯 번째 게임오브젝트
            _ => 0 // 기본값
        };
    }
}