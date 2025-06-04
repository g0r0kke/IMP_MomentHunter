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

    [SerializeField] private GameState _gameState = GameState.Intro;
    public GameState GameState => _gameState;
    
    [SerializeField] private MissionState _missionState = MissionState.None;
    public MissionState MissionState => _missionState;
    
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGameState(GameState newGameState)
    {
        if (_gameState == newGameState)
            return;
        
        this._gameState = newGameState;

        switch (_gameState)
        {
            case GameState.Intro:
                break;
            case GameState.Room1:
                break;
            case GameState.Room2:
                break;
            case GameState.Victory:
                break;
            case GameState.Defeat:
                break;
        }
    }

    public void SetMissionState(MissionState newMissionState)
    {
        if (_missionState == newMissionState)
            return;
        
        this._missionState = newMissionState;

        switch (_missionState)
        {
            case MissionState.Tutorial:
                break;
            case MissionState.Mission1:
                break;
            case MissionState.Mission2:
                break;
            case MissionState.Mission3:
                break;
            case MissionState.Mission4:
                break;
            case MissionState.Mission5:
                break;
            case MissionState.Mission6:
                break;
            case MissionState.Ending:
                break;
        }
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
}
