using System.Collections;
using UnityEngine;

/// <summary>
/// Animates transform properties (position, rotation, scale) over time.
/// Supports smooth transitions from start values to target values with customizable duration.
/// </summary>
public class TransformAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 targetRotation;
    [SerializeField] private Vector3 targetScale = Vector3.one;
    
    [Header("Key Settings")]
    [SerializeField] private MeshRenderer keyMesh;
    
    /// <summary>
    /// Starting position for animation (local space)
    /// </summary>
    private Vector3 startPosition;
    /// <summary>
    /// Starting rotation for animation (local space)
    /// </summary>
    private Vector3 startRotation;
    /// <summary>
    /// Starting scale for animation (local space)
    /// </summary>
    private Vector3 startScale;
    
    /// <summary>
    /// Store initial transform values on start
    /// </summary>
    private void Start()
    {
        startPosition = transform.localPosition;    // Use localPosition
        startRotation = transform.localEulerAngles; // Use localEulerAngles
        startScale = transform.localScale;
    }
    
    /// <summary>
    /// Starts animation to configured target values
    /// </summary>
    public void AnimateTransform()
    {
        // Deactivate key prefab if assigned
        if (keyMesh) keyMesh.enabled = false;
        
        StartCoroutine(DoAnimation());
    }
    
    /// <summary>
    /// Coroutine that performs the smooth animation over time
    /// </summary>
    /// <returns>Coroutine enumerator</returns>
    private IEnumerator DoAnimation()
    {
        float elapsedTime = 0f;
        
        // Animate over duration
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
        
            // Lerp all transform properties
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);    
            transform.localEulerAngles = Vector3.Lerp(startRotation, targetRotation, t); 
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
        
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        // Set exact final values to avoid floating point precision issues
        transform.localPosition = targetPosition;    
        transform.localEulerAngles = targetRotation; 
        transform.localScale = targetScale;
    }
}
