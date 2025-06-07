using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Data { get; private set; }
    
    [Header("Player Information")]
    private const int MAX_HEALTH = 8;
    public int MaxHealth => MAX_HEALTH;
    [SerializeField] private int _currentHealth = MAX_HEALTH;
    public int CurrentHealth => _currentHealth;

    
    [Header("Game Settings")]
    private float _masterVolume = 1f;
    public float MasterVolume => _masterVolume;

    [Header("UI Components")]
    [SerializeField] private MissionText _missionText;

    private void Awake()
    {
        if (!Data)
        {
            Data = this;
            DontDestroyOnLoad(gameObject);
            
            // 씬 로드 이벤트에 구독
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Data != this) return;
        
        FindMissionText();
        UpdateHealthUI();
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindMissionText();
        UpdateHealthUI();
    }
    
    private void FindMissionText()
    {
        _missionText = FindFirstObjectByType<MissionText>();
        
        if (!_missionText)
        {
            Debug.LogWarning($"DataManager: 씬 '{SceneManager.GetActiveScene().name}'에서 MissionText 컴포넌트를 찾을 수 없습니다.");
        }
    }

    // 에디터에서 값 변경 시 호출
    private void OnValidate()
    {
        // 체력 범위 제한
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MAX_HEALTH);

        // 런타임 중일 때만 UI 업데이트
        if (Application.isPlaying)
        {
            UpdateHealthUI();
        }
   
        // UI 업데이트
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // MissionText 컴포넌트 찾아서 업데이트
        if (_missionText)
        {
            _missionText.UpdateHealthText(_currentHealth);
        }
    }
    
    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (Data == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void UseHealth()
    {
        if (_currentHealth > 0)
        {
            --_currentHealth;
            UpdateHealthUI();
        }
        else
        {
            GameManager.Instance.SetGameState(GameState.Defeat);
        }
    }
}
