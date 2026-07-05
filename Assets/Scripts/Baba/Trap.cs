using UnityEngine;
using UnityEngine.VFX;

public class Trap : MonoBehaviour
{
    private enum trapType { spikes, lava, fire, axe, fall }
    [SerializeField] private trapType type;


    private void Awake()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Egg egg = ResolveEgg(other);
        if (egg == null) return;
        switch (type)
        {
            case trapType.fall:
                DisableCollider(egg.GetComponent<Collider>());
                if (egg.dino != null)
                {
                    foreach(Collider coll in egg.dino.GetComponents<Collider>())
                    {
                        DisableCollider(coll);
                    }
                }
                egg.EggDeath();
                break;
            case trapType.axe:
                egg.EggDeath();
                break;

            default:
                egg.EggDeath();
                break;
        }
    }

    private static void DisableCollider(Collider collider)
    {
        if (collider != null) collider.enabled = false;
    }

    // Works whether it's the Egg's own collider that hit the trap (rolling
    // mode - see PlayerController.ChangeMode, where the dino's collider is
    // disabled) or the dino's collider directly (walking mode).
    private Egg ResolveEgg(Collider other)
    {
        Debug.Log(other.name);
        Egg egg = other.GetComponent<Egg>();
        if (egg != null) return egg;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null) return player.egg;

        return null;
    }
    public void Setup()
    {
        switch (type)
        {
            case trapType.axe:

                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }

                break;

            case trapType.spikes:

                foreach (Transform child in transform)
                {
                    AudioManager.audioInstance.PlaySFX("Spikes", transform.position);
                    child.gameObject.SetActive(true);
                }

                break;
            case trapType.fall:

                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }

                break;
            case trapType.fire:

                foreach (Transform child in transform)
                {
                    AudioManager.audioInstance.PlaySFX("Fuego", transform.position);

                    child.gameObject.SetActive(true);

                }

                break;
            case trapType.lava:

                foreach (Transform child in transform)
                {
                    AudioManager.audioInstance.PlaySFX("Lava", transform.position);
                    child.gameObject.SetActive(true);
                }

                break;
            default:
                foreach(Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}