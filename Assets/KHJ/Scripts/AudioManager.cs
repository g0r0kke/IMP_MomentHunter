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
    }

    public void PlayAudio(int clipId)
    {
        // 안전성 체크
        if (_audioClips == null || clipId < 0 || clipId >= _audioClips.Length)
        {
            Debug.LogWarning($"Invalid clip ID: {clipId}");
            return;
        }
        
        if (_audioClips[clipId] == null)
        {
            Debug.LogWarning($"Audio clip at index {clipId} is null");
            return;
        }

        _audioSource.clip = _audioClips[clipId];
        _audioSource.Play();
    }
    
    // AudioSource의 clip을 바꾸지 않고 추가로 소리 재생
    // 이전 소리를 중단하지 않고 동시에 여러 소리 재생 가능
    public void PlayOneShot(int clipId)
    {
        if (IsValidClipId(clipId))
            _audioSource.PlayOneShot(_audioClips[clipId]);
    }
    
    public void StopAudio()
    {
        _audioSource.Stop();
    }
    
    private bool IsValidClipId(int clipId)
    {
        return _audioClips != null && clipId >= 0 && clipId < _audioClips.Length && _audioClips[clipId] != null;
    }
}
