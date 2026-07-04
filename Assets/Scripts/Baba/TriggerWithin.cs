using UnityEngine;

public class TriggerWithin : MonoBehaviour
{
    public enum TriggerType { corner, changeMode}
    [SerializeField] TriggerType type;
    private bool hasTriggered;
    public ModuleManager mM;
    public LevelModule parentRef;

    private void Start()
    {
        if(mM == null)
        {
            mM=FindAnyObjectByType<ModuleManager>();
            parentRef =GetComponentInParent<LevelModule>();
        }
}

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            switch (type)
            {
                case TriggerType.corner:
                    mM.StopRotation(parentRef, other.transform.position);
                    other.GetComponent<PlayerController>().CameraTurn();
                    break;

                case TriggerType.changeMode:
                    //other.GetComponent<PlayerController>().ChangeMode();
                    break;

            }
        }
        
    }
}
