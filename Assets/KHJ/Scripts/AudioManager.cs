using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager BGMInstance { get; private set; }
    
    [Header("Destroy Settings")]
    [SerializeField] private bool _dontDestroyOnLoad = false;
    
    [Header("Audio Components")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips;
    
    [Header("Auto Play Settings")]
    [SerializeField] private bool _autoPlayOnStart = false;
    [SerializeField] private int _backgroundMusicClipId = 0;
    [SerializeField] private bool _loopBackgroundMusic = true;
    
    void Awake()
    {
        if (_dontDestroyOnLoad)
        {
            // 이미 인스턴스가 있는지 확인
            if (BGMInstance != null)
            {
                // 이미 존재하면 새로운 것을 파괴
                Debug.Log("DontDestroyOnLoad AudioManager already exists. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            
            // 첫 번째 인스턴스로 설정
            BGMInstance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AudioManager set to DontDestroyOnLoad and assigned as BGMInstance");
        }
        else
        {
            // DontDestroyOnLoad가 아닌 경우는 BGMInstance 설정 안함
            Debug.Log("AudioManager created without DontDestroyOnLoad");
        }
    }
    
    void Start()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        
        // DataManager의 볼륨 변경 이벤트에 구독
        DataManager.OnMasterVolumeChanged += OnMasterVolumeChanged;
        
        // 현재 마스터 볼륨 즉시 적용
        ApplyMasterVolume();
        
        // 자신이 BGMInstance이고 자동재생이 활성화된 경우에만 재생
        if (_autoPlayOnStart && BGMInstance == this)
        {
            PlayBackgroundMusic();
        }
    }
    
    void OnDestroy()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        DataManager.OnMasterVolumeChanged -= OnMasterVolumeChanged;
        
        // 자신이 BGMInstance였다면 null로 설정
        if (BGMInstance == this)
        {
            BGMInstance = null;
        }
    }
    
    // 에디터에서 값 변경 시 호출
    private void OnValidate()
    {
        // 배경음악 클립 ID 범위 체크
        if (_audioClips != null && _audioClips.Length > 0)
        {
            _backgroundMusicClipId = Mathf.Clamp(_backgroundMusicClipId, 0, _audioClips.Length - 1);
        }
        
        // 런타임 중일 때만 업데이트
        if (Application.isPlaying)
        {
            // 마스터 볼륨 즉시 적용
            ApplyMasterVolume();
            
            // 자동 재생 설정이 변경된 경우 처리
            if (_autoPlayOnStart && !_audioSource.isPlaying)
            {
                PlayBackgroundMusic();
            }
            else if (!_autoPlayOnStart && _audioSource.isPlaying)
            {
                StopAudio();
            }
        }
    }
    
    // 볼륨 변경 이벤트 핸들러
    private void OnMasterVolumeChanged(float newVolume)
    {
        ApplyMasterVolume();
    }
    
    // 배경음악 재생
    public void PlayBackgroundMusic()
    {
        if (!IsValidClipId(_backgroundMusicClipId))
        {
            Debug.LogWarning($"Invalid background music clip ID: {_backgroundMusicClipId}");
            return;
        }
        
        _audioSource.clip = _audioClips[_backgroundMusicClipId];
        _audioSource.loop = _loopBackgroundMusic;
        
        // DataManager의 마스터 볼륨 적용
        ApplyMasterVolume();
        
        _audioSource.Play();
        
        Debug.Log($"Background music started: {_audioClips[_backgroundMusicClipId].name}");
    }

    public void PlayAudio(int clipId)
    {
        // 안전성 체크
        if (!IsValidClipId(clipId))
        {
            Debug.LogWarning($"Invalid clip ID: {clipId}");
            return;
        }

        _audioSource.clip = _audioClips[clipId];
        _audioSource.loop = false; // 일반 오디오는 반복하지 않음
        
        // DataManager의 마스터 볼륨 적용
        ApplyMasterVolume();
        
        _audioSource.Play();
    }
    
    // AudioSource의 clip을 바꾸지 않고 추가로 소리 재생
    // 이전 소리를 중단하지 않고 동시에 여러 소리 재생 가능
    public void PlayOneShot(int clipId)
    {
        if (!IsValidClipId(clipId))
        {
            Debug.LogWarning($"Invalid clip ID: {clipId}");
            return;
        }
        
        float volumeScale = GetMasterVolume();
        _audioSource.PlayOneShot(_audioClips[clipId], volumeScale);
    }
    
    public void StopAudio()
    {
        _audioSource.Stop();
        Debug.Log("Audio stopped");
    }
    
    private bool IsValidClipId(int clipId)
    {
        return _audioClips != null && clipId >= 0 && clipId < _audioClips.Length && _audioClips[clipId];
    }
    
    // DataManager에서 마스터 볼륨 가져오기
    private float GetMasterVolume()
    {
        if (DataManager.Data)
        {
            return DataManager.Data.GetMasterVolume();
        }
        
        return 1f; // DataManager가 없으면 기본값 1.0 반환
    }
    
    // AudioSource에 마스터 볼륨 적용
    private void ApplyMasterVolume()
    {
        if (_audioSource)
        {
            _audioSource.volume = GetMasterVolume();
        }
    }
}