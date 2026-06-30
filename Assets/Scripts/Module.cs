using UnityEngine;

public enum TrackType { Straight, LeftTurn, RightTurn, UpRamp, DownRamp }
[CreateAssetMenu(fileName = "NewModuleConfig", menuName = "Runner/Module Config")]
public class ModuleConfig : ScriptableObject
{
    public GameObject prefab;
    public TrackType trackType;
    [Range(1, 100)] public int spawnWeight = 50;
    public int difficultyRating = 1;
}
