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

    // 볼륨 변경 이벤트
    public static event Action<float> OnMasterVolumeChanged;
    
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
        
        // 새 씬이 로드될 때 현재 볼륨을 모든 AudioManager에 브로드캐스트
        OnMasterVolumeChanged?.Invoke(_masterVolume);
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
        float previousVolume = _masterVolume;
        
        // 체력 범위 제한
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MAX_HEALTH);

        // 볼륨 범위 제한
        _masterVolume = Mathf.Clamp01(_masterVolume);
        
        // 런타임 중일 때만 UI 업데이트
        if (Application.isPlaying)
        {
            UpdateHealthUI();
            
            // 볼륨이 변경되었다면 이벤트 발생
            if (!Mathf.Approximately(previousVolume, _masterVolume))
            {
                OnMasterVolumeChanged?.Invoke(_masterVolume);
            }
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
            OnMasterVolumeChanged = null; // 이벤트 구독자 모두 해제
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
    
    // 볼륨 변경 함수 (int 값 0~100으로 받아서 0~1로 변환)
    public void SetMasterVolume(int volumeLevel)
    {
        float newVolume = Mathf.Clamp(volumeLevel / 100f, 0f, 1f);

        // 볼륨이 실제로 변경된 경우에만 이벤트 발생
        if (!Mathf.Approximately(_masterVolume, newVolume))
        {
            _masterVolume = newVolume;

            // 모든 AudioManager에게 볼륨 변경 알림
            OnMasterVolumeChanged?.Invoke(_masterVolume);

            Debug.Log($"마스터 볼륨이 {_masterVolume:F1}로 설정되었습니다.");
        }
    }

    public float GetMasterVolume()
    {
        return _currentHealth;
    }

    public void InitializeHealth()
    {
        _currentHealth = MAX_HEALTH;
    }
}
