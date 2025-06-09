using System.Collections;
using UnityEngine;

public class TransformAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 targetRotation;
    [SerializeField] private Vector3 targetScale = Vector3.one;
    
    private Vector3 startPosition;
    private Vector3 startRotation;
    private Vector3 startScale;
    
    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        startScale = transform.localScale;
    }
    
    /// <summary>
    /// 설정된 타겟으로 애니메이션
    /// </summary>
    public void AnimateTransform()
    {
        StartCoroutine(DoAnimation());
    }
    
    private IEnumerator DoAnimation()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 정확한 최종값 설정
        transform.position = targetPosition;
        transform.eulerAngles = targetRotation;
        transform.localScale = targetScale;
    }
}
