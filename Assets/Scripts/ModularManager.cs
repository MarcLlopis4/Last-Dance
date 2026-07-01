using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private List<ModuleConfig> availableModules;
    [SerializeField] private int initialModulesOnScreen = 5;

    [Header("Movement & Flow")]
    [SerializeField] private float gameSpeed = 10f;
    [SerializeField] private float speedMultiplier = 0.05f;
    [SerializeField] private float despawnZPosition = -15f;
    [SerializeField] private Vector3 globalDirection;

    public Queue<LevelModule> activeModules = new Queue<LevelModule>();
    public Dictionary<GameObject, Queue<LevelModule>> pools = new Dictionary<GameObject, Queue<LevelModule>>();

    private LevelModule lastEnqueued;
    private TrackType lastCurve;

    // This is the secret sauce. Tracks the absolute Z edge of our track.
    private float trackedTrackEndZ = 0f;
    private Vector3 trackedTrackEnd = Vector3.zero;

    private void Start()
    {
        // Start track building at world 0 (or wherever your player stands)
        trackedTrackEndZ = 0f;
        trackedTrackEnd = Vector3.zero;


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
        trackedTrackEnd += globalDirection * gameSpeed * Time.deltaTime;

        foreach (var module in activeModules)
        {
            module.UpdateSpeed(gameSpeed);
        }
    }

    public void StopRotation(LevelModule activatingModule)
    {
        foreach (var module in activeModules)
        {
            if (module == activeModules.ElementAt(0)) 
            {
                globalDirection = -activatingModule.EndAnchor.forward;
            }
            module.rotating = true;
            module.UpdateDirection(globalDirection);
            module.StartRotation();
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
            else if(module.trackType!=TrackType.Straight&&module.trackType==lastCurve)
            {
                Debug.Log(module.gameObject.name);
                Destroy(module.gameObject);
                selectedConfig =availableModules[0];
                module=GetModuleFromPool(selectedConfig.prefab);
                module.trackType=selectedConfig.trackType;
                module.transform.LookAt(module.transform.position + lastEnqueued.EndAnchor.transform.forward);
                Debug.Log(module.gameObject.name);
            }
            else
            {
                module.transform.LookAt(module.transform.position + lastEnqueued.EndAnchor.transform.forward);
            }
        }
        else
        {
            Destroy(module.gameObject);
            module = null;
            selectedConfig = availableModules[0];
            module = GetModuleFromPool(selectedConfig.prefab);
            module.trackType = selectedConfig.trackType;
        }
        Debug.Log(gameSpeed.ToString() + " " + despawnZPosition.ToString());
        module.direction = globalDirection;
        module.Initialize(gameSpeed, despawnZPosition);
        

        // Snap directly to our mathematically flawless tracking float
        module.SnapToTargetZ(trackedTrackEnd, lastEnqueued?.EndAnchor.transform.position??Vector3.zero);

        // Advance our tracking float by the exact length of the new module

        trackedTrackEnd +=new Vector3(module.ModuleLengths.x * globalDirection.x, module.ModuleLengths.y * globalDirection.y, module.ModuleLengths.z * globalDirection.z);

        activeModules.Enqueue(module);
        lastEnqueued = module;
    }


    
}