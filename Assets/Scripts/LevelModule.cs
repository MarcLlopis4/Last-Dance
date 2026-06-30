using UnityEngine;
using System;

public class LevelModule : MonoBehaviour
{
    [SerializeField] private Transform spawnAnchor;
    [SerializeField] private Transform endAnchor;

    public Transform SpawnAnchor { get => spawnAnchor; set => spawnAnchor = value; }
    public Transform EndAnchor { get => endAnchor; set => endAnchor = value; }

    private bool hasEntered;
    public float ModuleLength { get; private set; }

    public TrackType trackType;
    private float moveSpeed;
    private float despawnZ;

    public Vector3 direction = Vector3.back;

    public ModuleManager mM;

    private void Awake()
    {
        // Calculate the exact distance between your front and back anchors on the Z axis
        ModuleLength = Mathf.Abs(endAnchor.localPosition.z - spawnAnchor.localPosition.z);
    }

    public void Initialize(float speed, float despawnPositionZ)
    {
        moveSpeed = speed;
        despawnZ = despawnPositionZ;
    }

    public void UpdateSpeed(float newSpeed) => moveSpeed = newSpeed;
    public void UpdateDirection(Vector3 newDirection) => direction= newDirection;

    private void LateUpdate()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        if (!hasEntered&& (transform.parent.localPosition - endAnchor.position).magnitude < despawnZ)
        {
            hasEntered = true;
        }
        // Despawn based on the module's actual rear end passing the threshold
        if ((hasEntered&& (transform.parent.localPosition - endAnchor.position).magnitude>despawnZ)||((transform.parent.localPosition-endAnchor.position).magnitude>100f))
        {
            transform.parent.GetComponent<ModuleManager>().SpawnNextModule();
            transform.parent.GetComponent<ModuleManager>().activeModules.Dequeue();

            Destroy(gameObject);
        }
    }

    // Snaps this module flush against the mathematical end of the last module
    public void SnapToTargetZ(Vector3 target)
    {
        // Local offset of the mesh/pivot relative to its own spawn anchor
        Vector3 spawnAnchorOffset = transform.localPosition - spawnAnchor.localPosition;

        Vector3 newPos = transform.localPosition;
        newPos = target + spawnAnchorOffset;
        transform.localPosition = newPos;
    }
}