using System.Collections.Generic;
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

    private Queue<LevelModule> activeModules = new Queue<LevelModule>();
    private Dictionary<GameObject, Queue<LevelModule>> pools = new Dictionary<GameObject, Queue<LevelModule>>();

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
        if (!pools.ContainsKey(prefab)) pools[prefab] = new Queue<LevelModule>();

        if (pools[prefab].Count > 0)
        {
            LevelModule pooledModule = pools[prefab].Dequeue();
            pooledModule.gameObject.SetActive(true);
            return pooledModule;
        }

        GameObject obj = Instantiate(prefab,this.transform);
        return obj.GetComponent<LevelModule>();
    }
    public void SpawnNextModule()
    {
        ModuleConfig selectedConfig = GetRandomModuleConfig();
        LevelModule module = GetModuleFromPool(selectedConfig.prefab);

        module.Initialize(gameSpeed, despawnZPosition, RecycleModule);

        // Snap directly to our mathematically flawless tracking float
        module.SnapToTargetZ(trackedTrackEndZ);

        // Advance our tracking float by the exact length of the new module
        trackedTrackEndZ += module.ModuleLength;

        activeModules.Enqueue(module);
    }

    // ... Keep your GetRandomModuleConfig() and GetModuleFromPool() identical ...

    private void RecycleModule(LevelModule module)
    {
        activeModules.Dequeue();
        module.gameObject.SetActive(false);

        string cleanName = module.gameObject.name.Replace("(Clone)", "").Trim();
        GameObject matchingPrefab = availableModules.Find(m => m.prefab.name == cleanName)?.prefab;
        module.transform.localPosition = Vector3.zero;

        if (matchingPrefab != null) pools[matchingPrefab].Enqueue(module);
        else Destroy(module.gameObject);

        // No more UpdateNextSpawnPoint() calculation needed! 
        // The Update loop continuously shifts trackedTrackEndZ perfectly.
        SpawnNextModule();
    }
}