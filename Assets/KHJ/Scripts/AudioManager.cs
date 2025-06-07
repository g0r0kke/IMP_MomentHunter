using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioClip[] _audioClips;
    
    void Start()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        
        // DataManager의 볼륨 변경 이벤트에 구독
        DataManager.OnMasterVolumeChanged += OnMasterVolumeChanged;
        
        // 현재 마스터 볼륨 즉시 적용
        ApplyMasterVolume();
    }
    
    void OnDestroy()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        DataManager.OnMasterVolumeChanged -= OnMasterVolumeChanged;
    }
    
    // 볼륨 변경 이벤트 핸들러
    private void OnMasterVolumeChanged(float newVolume)
    {
        ApplyMasterVolume();
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
            return DataManager.Data.MasterVolume;
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
