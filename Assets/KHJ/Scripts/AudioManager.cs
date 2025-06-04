using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] audioClips;
    
    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(int clipId)
    {
        // 안전성 체크
        if (audioClips == null || clipId < 0 || clipId >= audioClips.Length)
        {
            Debug.LogWarning($"Invalid clip ID: {clipId}");
            return;
        }
        
        if (audioClips[clipId] == null)
        {
            Debug.LogWarning($"Audio clip at index {clipId} is null");
            return;
        }

        audioSource.clip = audioClips[clipId];
        audioSource.Play();
    }
    
    // AudioSource의 clip을 바꾸지 않고 추가로 소리 재생
    // 이전 소리를 중단하지 않고 동시에 여러 소리 재생 가능
    public void PlayOneShot(int clipId)
    {
        if (IsValidClipId(clipId))
            audioSource.PlayOneShot(audioClips[clipId]);
    }
    
    public void StopAudio()
    {
        audioSource.Stop();
    }
    
    private bool IsValidClipId(int clipId)
    {
        return audioClips != null && clipId >= 0 && clipId < audioClips.Length && audioClips[clipId] != null;
    }
}
