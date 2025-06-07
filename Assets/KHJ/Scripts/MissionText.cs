using System.Collections;
using TMPro;
using UnityEngine;

public class MissionText : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text _firstText;
    [SerializeField] private TMP_Text _secondText;
    
    [Header("Text Arrays")]
    [SerializeField] private string[] _firstTextArray;
    [SerializeField] private string[] _secondTextArray;
    
    private void Start()
    {
        UpdateMissionText();
    }
    
    // 인덱스로 텍스트 변경
    public void ChangeTexts(int index)
    {
        if (_firstTextArray != null && index >= 0 && index < _firstTextArray.Length)
        {
            _firstText.text = _firstTextArray[index];
        }
        
        if (_secondTextArray != null && index >= 0 && index < _secondTextArray.Length)
        {
            _secondText.text = _secondTextArray[index];
        }
    }
    
    public void UpdateMissionText()
    {
        if (!GameManager.Instance) return;
        
        int textIndex = GetTextIndex(GameManager.Instance.MissionState);
        ChangeTexts(textIndex);
    }
    
    private int GetTextIndex(MissionState missionState)
    {
        return missionState switch
        {
            MissionState.None => 7,
            MissionState.Tutorial => 0,
            MissionState.Mission1 => 1,
            MissionState.Mission2 => 2,
            MissionState.Mission3 => 3,
            MissionState.Mission4 => 4,
            MissionState.Mission5 => 5,
            MissionState.Mission6 => 6,
            MissionState.Ending => 7,
            _ => 7 // 기본값
        };
    }
}
