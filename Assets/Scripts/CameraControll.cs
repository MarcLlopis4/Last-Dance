using System.Collections;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    [SerializeField] private GameObject Egg;
    [SerializeField] private float xoffset;
    [SerializeField] private float yoffset;
    [SerializeField] private float zoffset;
    [SerializeField] private float smoothTime;
    [SerializeField] private float xMaxDistance = 1.6f;
    private bool camturning;

    Vector3 velocity = Vector3.zero;
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {



        if (camturning==false)
        {
            Vector3 target4 = new Vector3(Egg.transform.position.x + xoffset, Egg.transform.position.y + yoffset, Egg.transform.position.z - zoffset
               );
            transform.position = Vector3.SmoothDamp(transform.position, target4, ref velocity, smoothTime);
           
        }
            
        


        

    }

    public void CameraTurn(Vector3 posToLerp)
    {
        StartCoroutine(CamTurn(posToLerp));
    }


    private IEnumerator CamTurn(Vector3 posToLerp)
    {
        camturning = true;
        Debug.Log("CAmTurning");
        float timer = .35f;
        float time = 0;
        Vector3 InitialPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        while ((time<timer))
        {

            transform.position = Vector3.Lerp(InitialPos, posToLerp, time/timer);
            yield return null;
            time += Time.deltaTime;
        }
        camturning=false;
        Debug.Log("CAmEndLErp");


        yield return null;
    }


    
}
