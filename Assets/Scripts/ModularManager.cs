// ModuleManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class ModuleManager : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private List<ModuleConfig> availableModules;
    [SerializeField] private int initialModulesOnScreen = 5;

    [Header("Movement & Flow")]
    public float gameSpeed = 10f;
    public float speedMultiplier = 0.05f;
    public float brakingPercentage = 0.5f;
    [SerializeField] private float despawnWorldZ = -15f;
    [SerializeField] private Vector3 moveAxis = Vector3.back; // fixed world-space TRACK translation axis   never changes
    [SerializeField] private float turnDuration = 0.35f;      // seconds to ease a corner pivot

    public Queue<LevelModule> activeModules = new Queue<LevelModule>();
    private Dictionary<GameObject, Queue<LevelModule>> pools = new Dictionary<GameObject, Queue<LevelModule>>();
    private Dictionary<LevelModule, GameObject> moduleSourcePrefab = new Dictionary<LevelModule, GameObject>();

    private LevelModule lastEnqueued;
    private TrackType lastCurve = TrackType.Straight;
    private bool isTurning = false;
    [HideInInspector]public bool braking = false;

    // The direction the PLAYER is conceptually heading   always opposite the track's translation axis,
    // since the player is stationary and the track scrolls toward them.
    private Vector3 TravelDirection => -moveAxis;

    private void Start()
    {
        for (int i = 0; i < initialModulesOnScreen; i++)
        {
            SpawnNextModule();
        }
    }

    private void Update()
    {
        gameSpeed += speedMultiplier * Time.deltaTime;

        // Freeze translation while a corner pivot is easing in   see StopRotation/EaseRotation.
        if (!isTurning)
        {
            if(braking == false)
            {
                transform.position += moveAxis * gameSpeed * Time.deltaTime;
            }
            else
            {
                transform.position += moveAxis * gameSpeed * brakingPercentage * Time.deltaTime;
            }
            
        }
    }

    // ModuleManager.cs
    public void StopRotation(LevelModule activatingModule, Vector3 playerWorldPosition)
    {
        if (isTurning) return;
        if (activatingModule.trackType == TrackType.Straight) return;

        StartCoroutine(EaseRotation(activatingModule, playerWorldPosition));
    }

    private IEnumerator EaseRotation(LevelModule activatingModule, Vector3 pivot)
    {
        Vector3 currentExit = activatingModule.EndAnchor.forward;
        float totalAngle = Vector3.SignedAngle(currentExit, TravelDirection, Vector3.up);

        if (Mathf.Abs(totalAngle) < 0.01f) yield break;

        isTurning = true;

        float elapsed = 0f;
        float appliedAngle = 0f;

        while (elapsed < turnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / turnDuration);
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            float targetAngle = totalAngle * easedT;

            transform.RotateAround(pivot, Vector3.up, targetAngle - appliedAngle);
            appliedAngle = targetAngle;

            yield return null;
        }

        transform.RotateAround(pivot, Vector3.up, totalAngle - appliedAngle);
        isTurning = false;
    }

    private ModuleConfig GetRandomModuleConfig()
    {
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
        if (!pools.TryGetValue(prefab, out var pool))
        {
            pool = new Queue<LevelModule>();
            pools[prefab] = pool;
        }

        LevelModule module;
        if (pool.Count > 0)
        {
            module = pool.Dequeue();
            module.gameObject.SetActive(true);
        }
        else
        {
            GameObject obj = Instantiate(prefab, this.transform);
            module = obj.GetComponent<LevelModule>();
            module.mM = this;
        }

        moduleSourcePrefab[module] = prefab;
        return module;
    }

    public void ReturnModuleToPool(LevelModule module)
    {
        module.gameObject.SetActive(false);
        if (moduleSourcePrefab.TryGetValue(module, out var prefab) && pools.TryGetValue(prefab, out var pool))
        {
            pool.Enqueue(module);
        }
        else
        {
            Destroy(module.gameObject);
        }
    }

    public void SpawnNextModule()
    {
        ModuleConfig selectedConfig = GetRandomModuleConfig();

        if (selectedConfig.trackType != TrackType.Straight && selectedConfig.trackType == lastCurve)
        {
            selectedConfig = availableModules[0];
        }

        LevelModule module = GetModuleFromPool(selectedConfig.prefab);
        module.trackType = selectedConfig.trackType;

        if (lastEnqueued != null)
        {
            module.transform.SetPositionAndRotation(
                lastEnqueued.EndAnchor.position,
                lastEnqueued.EndAnchor.rotation);
        }
        else
        {
            module.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

        module.Initialize(despawnWorldZ);

        if ((module.trackType != TrackType.Straight) && (module.trackType != TrackType.UpRamp))
        {
            lastCurve = module.trackType;
        }

        activeModules.Enqueue(module);
        lastEnqueued = module;
    }
}