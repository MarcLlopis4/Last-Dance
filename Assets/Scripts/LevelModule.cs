// LevelModule.cs
using UnityEngine;

public class LevelModule : MonoBehaviour
{
    [SerializeField] private Transform spawnAnchor;
    [SerializeField] private Transform endAnchor;
    public Transform SpawnAnchor { get => spawnAnchor; set => spawnAnchor = value; }
    public Transform EndAnchor { get => endAnchor; set => endAnchor = value; }

    public TrackType trackType;
    public ModuleManager mM;

    private float despawnWorldZ;
    private bool hasTriggered;

    public void Initialize(float despawnPositionZ)
    {
        despawnWorldZ = despawnPositionZ;
        hasTriggered = false;
    }

    // LevelModule.cs
    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            mM.StopRotation(this, other.transform.position);
            other.GetComponent<PlayerController>().CameraTurn();
        }
    }


    private void LateUpdate()
    {
        if (endAnchor.position.z < despawnWorldZ)
        {
            if (mM.activeModules.Count > 0 && mM.activeModules.Peek() == this)
            {
                mM.activeModules.Dequeue();
                mM.SpawnNextModule();
                mM.ReturnModuleToPool(this);
            }
        }
    }
}