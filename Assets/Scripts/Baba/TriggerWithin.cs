using UnityEngine;

public class TriggerWithin : MonoBehaviour
{
    public enum TriggerType { corner, changeMode }
    [SerializeField] TriggerType type;
    private bool hasTriggered;
    public ModuleManager mM;
    public LevelModule parentRef;

    private void Start()
    {
        if (mM == null)
        {
            mM = FindAnyObjectByType<ModuleManager>();
            parentRef = GetComponentInParent<LevelModule>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (hasTriggered) return;

        PlayerController player = ResolvePlayerController(other);
        if (player == null) return;

        hasTriggered = true;
        switch (type)
        {
            case TriggerType.corner:
                mM.StopRotation(parentRef, other.transform.position);
                player.CameraTurn();
                break;

            case TriggerType.changeMode:
                Debug.LogError(other.gameObject.name);
                player.ChangeMode();
                break;
        }
    }

    // While rolling, the dino's own collider is disabled and the Egg's
    // collider is what's actually touching the world (see
    // PlayerController.ChangeMode), so the collider entering this trigger
    // is the Egg, not the dino. Fall back to the Egg's public "dino"
    // reference to still find the right PlayerController.
    private PlayerController ResolvePlayerController(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null) return player;

        Egg egg = other.GetComponent<Egg>();
        if (egg != null && egg.dino != null)
        {
            return egg.dino.GetComponent<PlayerController>();
        }

        return null;
    }
}