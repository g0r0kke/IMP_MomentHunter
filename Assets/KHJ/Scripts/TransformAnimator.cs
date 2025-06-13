using System.Collections;
using UnityEngine;

public class TransformAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 targetRotation;
    [SerializeField] private Vector3 targetScale = Vector3.one;
    
    [Header("Key Settings")]
    [SerializeField] private GameObject keyPrefab;
    
    private Vector3 startPosition;
    private Vector3 startRotation;
    private Vector3 startScale;
    
    private void Start()
    {
        startPosition = transform.localPosition;    // localPosition으로 변경
        startRotation = transform.localEulerAngles; // localEulerAngles로 변경
        startScale = transform.localScale;
    }
    
    /// <summary>
    /// 설정된 타겟으로 애니메이션
    /// </summary>
    public void AnimateTransform()
    {
        if (keyPrefab) keyPrefab.SetActive(false);
        
        StartCoroutine(DoAnimation());
    }
    
    private IEnumerator DoAnimation()
    {
        float elapsedTime = 0f;
        Vector3 positionDelta = targetPosition - startPosition;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
        
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);    
            transform.localEulerAngles = Vector3.Lerp(startRotation, targetRotation, t); 
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
        
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        // 정확한 최종값 설정
        transform.localPosition = targetPosition;    
        transform.localEulerAngles = targetRotation; 
        transform.localScale = targetScale;
    }
}
