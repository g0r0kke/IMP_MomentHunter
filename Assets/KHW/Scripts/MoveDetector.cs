using UnityEngine;

public class MoveTriggerZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger zone has the "Player" tag
        if (other.CompareTag("Player"))
        {
            TutorialManager.Instance?.OnMoveDone();   // Notify tutorial progress
        }
    }
}
