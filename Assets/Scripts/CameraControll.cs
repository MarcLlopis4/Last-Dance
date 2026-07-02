using UnityEngine;

public class CameraControll : MonoBehaviour
{
    [SerializeField] private GameObject Egg;
    [SerializeField] private float xoffset;
    [SerializeField] private float yoffset;
    [SerializeField] private float zoffset;
    [SerializeField] private float smoothTime;

    Vector3 velocity = Vector3.zero;
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
       
        Vector3 target = new Vector3(Egg.transform.position.x + xoffset, Egg.transform.position.y+ yoffset, Egg.transform.position.z -zoffset
            );
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
    }
}
