using System;
using System.Collections.Generic;
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
    Tutorial,
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
    
    [Header("UI Settings")]
    [SerializeField] private bool _isMainCanvasActive = true;
    
    [Header("Scene Configuration")]
#if UNITY_EDITOR
    [SerializeField] private List<SceneAsset> _sceneAssets = new List<SceneAsset>(); // 씬 에셋 리스트
#endif
    [SerializeField] private List<string> _sceneNames = new List<string>(); // 씬 이름 리스트 (빌드용)
    
    void Awake()
    {
        // Singleton implementation
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        SetMainCanvasActive(true);
    }

    public void SetGameState(GameState newGameState)
    {
        if (_gameState == newGameState)
            return;
        
        this._gameState = newGameState;

        switch (_gameState)
        {
            case GameState.Victory:
                TransitionToScene(3);
                SetMissionState(MissionState.Ending);
                break;
            case GameState.Defeat:
                SetMissionState(MissionState.Ending);
                Debug.Log("Defeat");
                break;
            default:
                break;
        }
    }

    public void SetMissionState(MissionState newMissionState)
    {
        if (_missionState == newMissionState)
            return;
        
        this._missionState = newMissionState;

        OnMissionStateChanged?.Invoke(_missionState);
        
        if (_missionText)
        {
            _missionText.UpdateMissionText();
        }
        
        switch (_missionState)
        {
            case MissionState.None:
                TransitionToScene(0);
                SetGameState(GameState.Intro);
                break;
            case MissionState.Tutorial:
                TransitionToScene(1);
                SetGameState(GameState.Room1);
                break;
            case MissionState.Mission1:
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
                break;
        }
    }

    public void SetNextMissionState()
    {
        int nextIndex = ((int)_missionState + 1) % System.Enum.GetValues(typeof(MissionState)).Length;
        SetMissionState((MissionState)nextIndex);
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
        
        string targetScene = _sceneNames[sceneIndex];
        SceneManager.LoadScene(targetScene);
    }

    public void SetMainCanvasActive(bool isActive)
    {
        _isMainCanvasActive = isActive;
        _mainCanvas.enabled = _isMainCanvasActive;
    }
}
