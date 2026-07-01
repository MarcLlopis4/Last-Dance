using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private List<ModuleConfig> availableModules;
    [SerializeField] private int initialModulesOnScreen = 5;

    [Header("Movement & Flow")]
    public float gameSpeed = 10f;
    public float speedMultiplier = 0.05f;
    [SerializeField] private float despawnZPosition = -15f;

    public Queue<LevelModule> activeModules = new Queue<LevelModule>();
    public Dictionary<GameObject, Queue<LevelModule>> pools = new Dictionary<GameObject, Queue<LevelModule>>();

    private LevelModule lastEnqueued;

    // This is the secret sauce. Tracks the absolute Z edge of our track.
    private float trackedTrackEndZ = 0f;

    private void Start()
    {
        // Start track building at world 0 (or wherever your player stands)
        trackedTrackEndZ = 0f;

        for (int i = 0; i < initialModulesOnScreen; i++)
        {
            SpawnNextModule();
        }
    }

    private void Update()
    {
        gameSpeed += speedMultiplier * Time.deltaTime;

        // Physically shift our mathematical end point backwards alongside the world
        trackedTrackEndZ -= gameSpeed * Time.deltaTime;

        foreach (var module in activeModules)
        {
            module.UpdateSpeed(gameSpeed);
        }
    }
    private ModuleConfig GetRandomModuleConfig()
    {
        // Simple Weighted Random Selection
        int totalWeight = 0;
        foreach (var mod in availableModules) totalWeight += mod.spawnWeight;

        int randomRoll = Random.Range(0, totalWeight);
        int processedWeight = 0;

        foreach (var mod in availableModules)
        {
            processedWeight += mod.spawnWeight;
            if (randomRoll < processedWeight) return mod;
        }

        return availableModules[0];
    }

    private LevelModule GetModuleFromPool(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab,this.transform);
        obj.GetComponent<LevelModule>().mM=this;
        return obj.GetComponent<LevelModule>();
    }
    public void SpawnNextModule()
    {
        ModuleConfig selectedConfig = GetRandomModuleConfig();
        LevelModule module = GetModuleFromPool(selectedConfig.prefab);
        module.trackType = selectedConfig.trackType;
        if (lastEnqueued)
        {
            if (module.trackType != TrackType.Straight && module.trackType != lastEnqueued.trackType)
            {
                module.transform.LookAt(module.transform.position + lastEnqueued.EndAnchor.transform.forward);
            }
            else if(module.trackType!=TrackType.Straight&&module.trackType==lastEnqueued.trackType)
            {
                selectedConfig=availableModules[0];
                module=GetModuleFromPool(selectedConfig.prefab);
                module.trackType=selectedConfig.trackType;
                module.transform.LookAt(module.transform.position + lastEnqueued.EndAnchor.transform.forward);
            }
            else
            {
                module.transform.LookAt(module.transform.position + lastEnqueued.EndAnchor.transform.forward);
            }
            //else if(module.trackType!=0 && module.trackType==lastEnqueued.trackType)
            //{
            //    selectedConfig = GetRandomModuleConfig();
            //    module=GetModuleFromPool(selectedConfig.prefab);
            //    module.trackType = selectedConfig.trackType;
            //    if (module.trackType == lastEnqueued.trackType)
            //    {
            //        selectedConfig = GetRandomModuleConfig();
            //        module = GetModuleFromPool(selectedConfig.prefab);
            //        module.trackType = selectedConfig.trackType;
            //    }
            //    else
            //    {
            //        module = Instantiate(availableModules.ElementAt(0).prefab).GetComponent<LevelModule>();
            //        module.trackType = 0;
            //    }
            //}
        }
        else
        {

        }
            module.Initialize(gameSpeed, despawnZPosition);
        

        // Snap directly to our mathematically flawless tracking float
        module.SnapToTargetZ(lastEnqueued?.EndAnchor.transform.position??Vector3.zero);

        // Advance our tracking float by the exact length of the new module
        trackedTrackEndZ += module.ModuleLength;

        activeModules.Enqueue(module);
        lastEnqueued = module;
    }


    
}