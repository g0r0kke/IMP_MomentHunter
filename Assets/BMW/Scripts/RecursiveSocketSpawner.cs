using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RecursiveSocketSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private GameObject nextObjectToSpawn;
    [SerializeField] private int maxDepth;
    [SerializeField] private bool IsNextSpawn = false;
    private int currentDepth = 1;

    [Header("Transform Settings")]
    [SerializeField] private Vector3 spawnRotation;
    [SerializeField] private Vector3 spawnScale;
    [SerializeField] private float spawnedYPose = 0;
    [SerializeField] private float spawnedXPose = 0;

    private XRSocketInteractor socketInteractor;
    private GameObject spawnedObject;
    Vector3 spawnPosition;

    RecursiveSocketSpawner childSpawner;

    void Start()
    {
        if (objectToSpawn == null) objectToSpawn = this.gameObject;
        if (spawnRotation == Vector3.zero) spawnRotation = transform.rotation.eulerAngles;
        if (spawnScale  == Vector3.one) spawnScale = transform.localScale;
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketInteractor.selectEntered.AddListener(OnObjectPlaced);
        socketInteractor.selectExited.AddListener(OnObjectRemoved);
    }

    private void OnObjectPlaced(SelectEnterEventArgs args)
    {

        if (currentDepth >= maxDepth)
        {
            if (IsNextSpawn)
            {
                spawnRotation = nextObjectToSpawn.transform.rotation.eulerAngles;
                spawnScale = nextObjectToSpawn.transform.localScale;

                spawnPosition = socketInteractor.attachTransform.position;
                if (spawnedYPose != 0) spawnPosition = spawnPosition + Vector3.up * spawnedYPose;
                spawnPosition = spawnPosition + Vector3.left * 0.16f;

                spawnedObject = Instantiate(nextObjectToSpawn, spawnPosition, Quaternion.Euler(spawnRotation));
                spawnedObject.transform.localScale = spawnScale;

                childSpawner = spawnedObject.GetComponent<RecursiveSocketSpawner>();
                if (childSpawner != null)
                {
                    childSpawner.currentDepth = 1;
                }
            }
            return;
        }

        spawnPosition = socketInteractor.attachTransform.position;
        if (spawnedYPose != 0) spawnPosition = spawnPosition + Vector3.up * spawnedYPose;
        if (spawnedXPose != 0) spawnPosition = spawnPosition + Vector3.right * spawnedXPose;

        spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.Euler(spawnRotation));
        spawnedObject.transform.localScale = spawnScale;

        childSpawner = spawnedObject.GetComponent<RecursiveSocketSpawner>();
        if (childSpawner != null)
        {
            childSpawner.currentDepth = currentDepth + 1;
        }
    }

    private void OnObjectRemoved(SelectExitEventArgs args)
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }
    }
}
