// LevelModule.cs
using System.Linq;
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
    private bool hasActivated;
    private Trap[] childTraps;

    public void Initialize(float despawnPositionZ)
    {
        despawnWorldZ = despawnPositionZ;
        hasTriggered = false;
        hasActivated = false;
        childTraps = transform.GetComponentsInChildren<Trap>();
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

        if (!hasActivated && spawnAnchor.position.z<6f)
        {
            hasActivated = true;
            foreach (var child in childTraps)
            {
                child.Setup();
            }
        }
    }
}