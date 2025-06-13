using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RecursiveSocketSpawner : MonoBehaviour
{
    // SPAWN CONFIGURATION
    [Header("Spawn Settings")]
    [SerializeField] private GameObject objectToSpawn;       // Base prefab to spawn initially
    [SerializeField] private GameObject nextObjectToSpawn;   // Alternate prefab for deep recursion
    [SerializeField] private int maxDepth = 3;               // Maximum recursion depth allowed
    [SerializeField] private bool IsNextSpawn = false;       // Switch to alternate spawning mode
                     private int currentDepth = 1;                            // Current recursion depth counter

    // TRANSFORM SETTINGS
    [Header("Transform Settings")]
    [SerializeField] private Vector3 spawnRotation;          // Initial rotation for spawned objects
    [SerializeField] private Vector3 spawnScale;             // Initial scale for spawned objects
    [SerializeField] private float spawnedZPose = 0;         // Z-axis position offset
    [SerializeField] private float spawnedYPose = 0;         // Y-axis position offset
    [SerializeField] private float spawnedXPose = 0;         // X-axis position offset

    // RUNTIME REFERENCES
    private XRSocketInteractor socketInteractor;             // XR socket interaction component
    private GameObject spawnedObject;                        // Currently spawned object instance
    private Vector3 spawnPosition;                           // Calculated spawn position
    private RecursiveSocketSpawner childSpawner;             // Reference to child spawner component

    void Start()
    {
        // Initialize default values if not set
        if (objectToSpawn == null) objectToSpawn = this.gameObject;
        if (spawnRotation == Vector3.zero) spawnRotation = transform.rotation.eulerAngles;
        if (spawnScale == Vector3.one) spawnScale = transform.localScale;

        // Configure socket interactor events
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketInteractor.selectEntered.AddListener(OnObjectPlaced);
        socketInteractor.selectExited.AddListener(OnObjectRemoved);
    }

    // Handles object placement in socket with recursive spawning logic
    private void OnObjectPlaced(SelectEnterEventArgs args)
    {
        if (currentDepth >= maxDepth)
        {
            HandleDeepRecursionSpawn();
            return;
        }

        // Calculate base spawn position from socket
        spawnPosition = socketInteractor.attachTransform.position;

        // Apply positional offsets
        spawnPosition += new Vector3(
            spawnedXPose,
            spawnedYPose,
            spawnedZPose
        );

        // Instantiate new object with configured transforms
        spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.Euler(spawnRotation));
        spawnedObject.transform.localScale = spawnScale;

        // Configure child spawner recursion depth
        childSpawner = spawnedObject.GetComponent<RecursiveSocketSpawner>();
        if (childSpawner != null)
        {
            childSpawner.currentDepth = currentDepth + 1; // Increment depth for recursion
        }
    }

    // Handles special case spawning when maximum depth is reached
    private void HandleDeepRecursionSpawn()
    {
        if (!IsNextSpawn) return;

        // Calculate alternate object rotation
        spawnRotation = nextObjectToSpawn.transform.rotation.eulerAngles;
        spawnRotation = new Vector3(spawnRotation.x, spawnRotation.y - 90, spawnRotation.z);

        // Set scale from alternate prefab
        spawnScale = nextObjectToSpawn.transform.localScale;

        // Calculate position with Y-offset and backward adjustment
        spawnPosition = socketInteractor.attachTransform.position;
        if (spawnedYPose != 0) spawnPosition += Vector3.up * spawnedYPose;
        spawnPosition += Vector3.back * 0.28f;

        // Instantiate alternate object
        spawnedObject = Instantiate(nextObjectToSpawn, spawnPosition, Quaternion.Euler(spawnRotation));
        spawnedObject.transform.localScale = spawnScale;

        // Reset child spawner depth if exists
        childSpawner = spawnedObject.GetComponent<RecursiveSocketSpawner>();
        if (childSpawner != null)
        {
            childSpawner.currentDepth = 1; // Reset depth counter
        }
    }

    // Cleans up spawned objects when removed from socket
    private void OnObjectRemoved(SelectExitEventArgs args)
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject); // Remove spawned hierarchy
        }
    }
}
