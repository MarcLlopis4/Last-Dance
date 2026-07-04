using UnityEngine;
using UnityEngine.VFX;

public class Trap : MonoBehaviour
{
    private enum trapType { spikes, lava, fire, axe, fall}
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
        if (other.GetComponent<Egg>())
        {
            switch (type)
            {
                case trapType.fall:
                    other.GetComponent<Collider>().enabled = false;
                    other.GetComponent<Egg>().dino.GetComponent<Collider>().enabled = false;
                    break;

            default:
                    break;
            }
        }
        
    }

    public void Setup()
    {
        switch (type)
        {
            default:
                GetComponent<VisualEffect>().Play();
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
