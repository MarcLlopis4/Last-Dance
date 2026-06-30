using UnityEngine;
using System;

public class LevelModule : MonoBehaviour
{
    [SerializeField] private Transform spawnAnchor;
    [SerializeField] private Transform endAnchor;
    private bool hasEntered;
    public float ModuleLength { get; private set; }

    private Action<LevelModule> onReturnToPool;
    private float moveSpeed;
    private float despawnZ;

    private void Awake()
    {
        // Calculate the exact distance between your front and back anchors on the Z axis
        ModuleLength = Mathf.Abs(endAnchor.localPosition.z - spawnAnchor.localPosition.z);
    }

    public void Initialize(float speed, float despawnPositionZ, Action<LevelModule> returnAction)
    {
        moveSpeed = speed;
        despawnZ = despawnPositionZ;
        onReturnToPool = returnAction;
    }

    public void UpdateSpeed(float newSpeed) => moveSpeed = newSpeed;

    private void LateUpdate()
    {
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.Self);
        if (!hasEntered&& (transform.parent.localPosition - endAnchor.position).magnitude < despawnZ)
        {
            hasEntered = true;
        }
        // Despawn based on the module's actual rear end passing the threshold
        if (hasEntered&& (transform.parent.localPosition - endAnchor.position).magnitude>despawnZ)
        {
            transform.parent.GetComponent<ModuleManager>().SpawnNextModule();

            Destroy(gameObject);
        }
    }

    // Snaps this module flush against the mathematical end of the last module
    public void SnapToTargetZ(float targetZ)
    {
        // Local offset of the mesh/pivot relative to its own spawn anchor
        float spawnAnchorOffset = transform.localPosition.z - spawnAnchor.localPosition.z;

        Vector3 newPos = transform.localPosition;
        newPos.z = targetZ + spawnAnchorOffset;
        transform.localPosition = newPos;
    }
}