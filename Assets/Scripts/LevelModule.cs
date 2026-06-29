using UnityEngine;
using System;

public class LevelModule : MonoBehaviour
{
    [SerializeField] private Transform endAnchor;
    [SerializeField] private Transform spawnAnchor;

    public Transform EndAnchor => endAnchor;

    private Action<LevelModule> onReturnToPool;
    private float moveSpeed;
    private float despawnZ;

    public void Initialize(float speed, float despawnPositionZ, Action<LevelModule> returnAction)
    {
        moveSpeed = speed;
        despawnZ = despawnPositionZ;
        onReturnToPool = returnAction;
    }

    public void UpdateSpeed(float newSpeed) => moveSpeed = newSpeed;

    private void LateUpdate()
    {
        // Move towards the screen (assuming movement along the Z-axis)
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

        // Despawn once completely past the despawn threshold
        if (endAnchor.position.z < despawnZ)
        {
            onReturnToPool?.Invoke(this);
        }
    }

    // Position this module flush against the previous module's end anchor
    public void SnapToAnchor(Vector3 targetPosition)
    {
        Vector3 offset = transform.position - spawnAnchor.position;
        transform.position = targetPosition + offset;
    }
}