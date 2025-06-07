using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            // 미션 6 종료 후엔 이거 말고 SetGameState() 호출 필요
            GameManager.Instance.SetNextMissionState();
        }
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            
        }
    }
}
