using System;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Intro,
    Room1,
    Room2,
    Victory,
    Defeat
}

public enum MissionState
{
    None,
    Mission1,
    Mission2,
    Mission3,
    Mission4,
    Mission5,
    Mission6,
    Ending
}

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    public static event Action<MissionState> OnMissionStateChanged;
    
    [SerializeField] private GameState _gameState = GameState.Intro;
    public GameState GameState => _gameState;
    
    [SerializeField] private MissionState _missionState = MissionState.None;
    public MissionState MissionState => _missionState;

    [Header("UI Components")]
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private MissionText _missionText;
    [SerializeField] private OpeningUIManager _openingUIManager;
    [SerializeField] private GameObject _failUI;
    
    [Header("UI Settings")]
    [SerializeField] private bool _isMainCanvasActive = true;
    
    [Header("Scene Configuration")]
#if UNITY_EDITOR
    [SerializeField] private List<SceneAsset> _sceneAssets = new List<SceneAsset>(); // 씬 에셋 리스트
#endif
    [SerializeField] private List<string> _sceneNames = new List<string>(); // 씬 이름 리스트 (빌드용)
    
    [Header("Mission Object Settings")]
    [SerializeField, HideInInspector] private List<int> _missionObjectCounts = new List<int>(); // 숨김
    private readonly int[] missionObjectCountsReadOnly = { 0, 2, 6, 2, 2, 1, 1, 0 }; // 읽기 전용 배열

    // 인스펙터에서 확인용 (수정 불가)
    [Space]
    [Header("Mission Object Counts (Read Only)")]
    [SerializeField] private string[] _missionObjectDisplay = new string[0];
    private int _currentMissionObjectCount = 0; // 현재 촬영한 미션 오브젝트 개수
    
    // 씬 로드 후 Initialize 호출을 위한 플래그
    private bool _shouldInitializeScene0Load = false;
    private bool _isDead = false;
    
    void Awake()
    {
        // Singleton implementation
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // 읽기 전용 배열을 리스트로 복사
        _missionObjectCounts = new List<int>(missionObjectCountsReadOnly);
    }

    void Start()
    {
        SetMainCanvasActive(true);
        if (_failUI) _failUI.SetActive(false);
    }
    
    void OnDestroy()
    {
        // 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    // 씬 로드 완료 시 호출되는 메서드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_shouldInitializeScene0Load && scene.name == _sceneNames[0])
        {
            _shouldInitializeScene0Load = false;
            Initialize();
        }
    }

    public void SetGameState(GameState newGameState)
    {
        if (_gameState == newGameState)
            return;
        
        this._gameState = newGameState;

        switch (_gameState)
        {
            case GameState.Victory:
                Invoke("WaitAndEnding", 3f);
                break;
            case GameState.Defeat:
                _isDead = true;
                Debug.Log("Defeat");
                if (_failUI) _failUI.SetActive(true);
                SetMissionState(MissionState.Ending);
                break;
            default:
                break;
        }
    }
    
    void WaitAndEnding()
    {
        TransitionToScene(3);
    }

    public void SetMissionState(MissionState newMissionState)
    {
        if (_missionState == newMissionState)
            return;
        
        this._missionState = newMissionState;
        
        // 미션 상태가 변경될 때 미션 오브젝트 개수 초기화
        ResetMissionObjectCount();

        OnMissionStateChanged?.Invoke(_missionState);
        
        if (_missionText)
        {
            _missionText.UpdateMissionText();
        }
        
        switch (_missionState)
        {
            case MissionState.None:
                _isDead = false;
                TransitionToScene(0);
                SetGameState(GameState.Intro);
                break;
            case MissionState.Mission1:
                TransitionToScene(1);
                SetGameState(GameState.Room1);
                break;
            case MissionState.Mission2:
                break;
            case MissionState.Mission3:
                break;
            case MissionState.Mission4:
                TransitionToScene(2);
                SetGameState(GameState.Room2);
                break;
            case MissionState.Mission5:
                break;
            case MissionState.Mission6:
                break;
            case MissionState.Ending:
                if (!_isDead) SetGameState(GameState.Victory);
                break;
        }
    }

    public void SetNextMissionState()
    {
        int nextIndex = ((int)_missionState + 1) % System.Enum.GetValues(typeof(MissionState)).Length;
        SetMissionState((MissionState)nextIndex);
    }
    
    /// <param name="capturedCount">촬영한 미션 오브젝트 개수</param>
    public void SetMissionObjectCount(int capturedCount)
    {
        _currentMissionObjectCount = capturedCount;
        
        int currentMissionIndex = (int)_missionState;
        
        // 리스트 범위 확인
        if (currentMissionIndex >= 0 && currentMissionIndex < _missionObjectCounts.Count)
        {
            int requiredCount = _missionObjectCounts[currentMissionIndex];
            
            Debug.Log($"미션 오브젝트 개수 업데이트: {_currentMissionObjectCount}/{requiredCount}");
            
            // 필요한 개수와 현재 개수 비교
            if (_currentMissionObjectCount == requiredCount)
            {
                Debug.Log("클리어!");
                int feedbackIndex = GetMissionFeedbackIndex(_missionState);
                _missionText.ActivateFeedbackObject(feedbackIndex);
                // 체력 감소
                if (DataManager.Data)
                {
                    DataManager.Data.UseHealth();
                }
                SetNextMissionState();
            }
            else
            {
                Debug.Log("실패!");
                _missionText.ActivateFeedbackObject(0);
                // 체력 감소
                if (DataManager.Data)
                {
                    DataManager.Data.UseHealth();
                }
            }
        }
        else
        {
            Debug.LogWarning($"미션 인덱스가 범위를 벗어났습니다: {currentMissionIndex}");
        }
    }
    
    // 미션 상태에 따른 피드백 인덱스 반환
    private int GetMissionFeedbackIndex(MissionState missionState)
    {
        return missionState switch
        {
            MissionState.Mission1 => 1,
            MissionState.Mission2 => 2,
            MissionState.Mission3 => 3,
            MissionState.Mission4 => 4,
            MissionState.Mission5 => 5,
            MissionState.Mission6 => 6,
            _ => 0 // 기본값 (실패용)
        };
    }

    /// <summary>
    /// 현재 미션 오브젝트 개수 초기화
    /// </summary>
    private void ResetMissionObjectCount()
    {
        _currentMissionObjectCount = 0;
    }
    
#if UNITY_EDITOR
    // Inspector에서 SceneAsset 변경 시 sceneNames 자동 업데이트
    private void OnValidate()
    {
        // sceneNames 리스트를 sceneAssets와 동기화
        _sceneNames.Clear();
        foreach (var sceneAsset in _sceneAssets)
        {
            if (sceneAsset != null)
            {
                _sceneNames.Add(sceneAsset.name);
            }
        }
        
        // 인스펙터 표시용 배열 업데이트
        var missionNames = System.Enum.GetNames(typeof(MissionState));
        _missionObjectDisplay = new string[missionObjectCountsReadOnly.Length];
        for (int i = 0; i < missionObjectCountsReadOnly.Length && i < missionNames.Length; i++)
        {
            _missionObjectDisplay[i] = $"{missionNames[i]}: {missionObjectCountsReadOnly[i]}";
        }
        
        if (_mainCanvas)
        {
            UnityEditor.EditorApplication.delayCall += () => 
            {
                if (_mainCanvas != null)
                    _mainCanvas.enabled = _isMainCanvasActive;
            };
        }
    }
#endif
    
    public void TransitionToScene(int sceneIndex)
    {
        // 유효한 인덱스인지 확인
        if (sceneIndex < 0 || sceneIndex >= _sceneNames.Count)
        {
            Debug.LogError($"Invalid scene index: {sceneIndex}. Available scenes: {_sceneNames.Count}");
            return;
        }

        if (sceneIndex == 0)
        {
            _shouldInitializeScene0Load = true;
            
            // 씬0으로 돌아갈 때 DontDestroyOnLoad AudioManager 파괴
            if (AudioManager.BGMInstance)
            {
                Debug.Log("Destroying DontDestroyOnLoad AudioManager when returning to Scene 0");
                Destroy(AudioManager.BGMInstance.gameObject);
                // BGMInstance는 AudioManager의 OnDestroy에서 null로 설정됨
            }
        }
        
        string targetScene = _sceneNames[sceneIndex];
        SceneManager.LoadScene(targetScene);
    }

    public void SetMainCanvasActive(bool isActive)
    {
        _isMainCanvasActive = isActive;
        _mainCanvas.enabled = _isMainCanvasActive;
    }

    private void Initialize()
    {
        if (DataManager.Data) DataManager.Data.InitializeHealth();
        // SetGameState(GameState.Intro);
        // SetMissionState(MissionState.None);
        // SetMainCanvasActive(true);
        // if (_openingUIManager) _openingUIManager.SetPrologueActive(false);
    }
}