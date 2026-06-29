using System.Collections.Generic;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private List<ModuleConfig> availableModules;
    [SerializeField] private int initialModulesOnScreen = 5;
    
    [Header("Movement & Flow")]
    [SerializeField] private float gameSpeed = 10f;
    [SerializeField] private float speedMultiplier = 0.05f; // Speed up over time
    [SerializeField] private float despawnZPosition = -15f;
    [SerializeField] private Vector3 spawnOrigin = Vector3.zero;

    private Queue<LevelModule> activeModules = new Queue<LevelModule>();
    private Dictionary<GameObject, Queue<LevelModule>> pools = new Dictionary<GameObject, Queue<LevelModule>>();
    private Vector3 nextSpawnPoint;

    private void Start()
    {
        nextSpawnPoint = spawnOrigin;

        // Initialize the track
        for (int i = 0; i < initialModulesOnScreen; i++)
        {
            SpawnNextModule(i == 0); // Force a safe/easy module for the very first one if desired
        }
    }

    private void LateUpdate()
    {
        gameSpeed += speedMultiplier * Time.deltaTime;
        
        // Dynamic speed updates for all active tracks
        foreach (var module in activeModules)
        {
            module.UpdateSpeed(gameSpeed);
        }
    }

    private void SpawnNextModule(bool forceSafe = false)
    {
        ModuleConfig selectedConfig = GetRandomModuleConfig();
        LevelModule module = GetModuleFromPool(selectedConfig.prefab);

        module.Initialize(gameSpeed, despawnZPosition, RecycleModule);
        module.SnapToAnchor(nextSpawnPoint);

        // Update the pointer for the next spawn
        nextSpawnPoint = module.EndAnchor.position;
        activeModules.Enqueue(module);
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

        GameObject obj = Instantiate(prefab);
        return obj.GetComponent<LevelModule>();
    }

    private void RecycleModule(LevelModule module)
    {
        // Dequeue from active tracker
        activeModules.Dequeue();
        module.gameObject.SetActive(false);

        // Figure out which pool this belongs to based on a custom ID or naming convention
        // For a jam, finding by prefab name override works fine:
        string cleanName = module.gameObject.name.Replace("(Clone)", "").Trim();
        GameObject matchingPrefab = availableModules.Find(m => m.prefab.name == cleanName)?.prefab;

        if (matchingPrefab != null)
        {
            pools[matchingPrefab].Enqueue(module);
        }
        else
        {
            Destroy(module.gameObject); // Fallback safety
        }

        // Catch up the spawn system: shift the target back to match where the current track ends
        // Because modules are moving, our nextSpawnPoint needs to move relative to the last chunk
        UpdateNextSpawnPoint();
        SpawnNextModule();
    }

    private void UpdateNextSpawnPoint()
    {
        // Find the last item currently alive in the queue to hook onto
        LevelModule[] activeArray = activeModules.ToArray();
        if (activeArray.Length > 0)
        {
            nextSpawnPoint = activeArray[activeArray.Length - 1].EndAnchor.position;
        }
        else
        {
            nextSpawnPoint = spawnOrigin;
        }
    }
}