using TMPro;
using UnityEngine;

public class MissionText : MonoBehaviour
{
    [Header("Text Components")]
    [SerializeField] private TMP_Text _firstText;
    [SerializeField] private TMP_Text _secondText;
    
    [Header("Text Arrays")]
    [SerializeField] private string[] _firstTextArray;
    [SerializeField] private string[] _secondTextArray;
    
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
}
