using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Egg : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    private CapsuleCollider coll;
    [SerializeField] private Animator animator;
    [SerializeField] private float EggForce;
    public GameObject dino;

    [SerializeField] private CameraControll camera;
    [SerializeField] private ModuleManager moduleManager;
    [SerializeField] private UIManager uiManager;

    public bool canPush;
    public float zOffset;
    public GameObject egginitialPos;
    public bool setinitialPosY;

    public float rotationoffset;
    float timeToRotate = .7f;

    bool jiggling = true;

    bool eggHit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        canPush = true;
        transform.position = new Vector3(dino.transform.position.x, egginitialPos.transform.position.y, dino.transform.position.z + zOffset);
       
        //StartCoroutine(EggRotates());
        
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (eggHit == false)
        {
            transform.position = new Vector3(dino.transform.position.x, transform.position.y, dino.transform.position.z + zOffset);

            if (setinitialPosY && canPush)
            {

                transform.position = new Vector3(dino.transform.position.x, egginitialPos.transform.position.y, dino.transform.position.z + zOffset);

            }
        }
       

        if (jiggling == true)
        {
            //StartCoroutine(EggJiggleMove());
        }
    }

    public void PushUp(Vector3 localinitialPos)
    {
        StartCoroutine(Pushing(localinitialPos));
    }
    public void StopJiggling()
    {
        StopCoroutine("EggJiggleMove");
    }

    private IEnumerator Pushing(Vector3 localInitialPos)
    {
        //animator.SetBool("Animate",false);
        // animator.enabled = false;

        

        if (transform.parent != null)
        {
            Rigidbody parentRb = transform.parent.GetComponentInParent<Rigidbody>();
            Vector3 parentVelocity = Vector3.zero;
            parentVelocity = parentRb.linearVelocity/3;
            transform.parent = null;
            rb.isKinematic = false;
            rb.linearVelocity = parentVelocity;
            Debug.Log("Velocity" + parentVelocity);// Use .velocity in older Unity versions
        }
        transform.parent = null;
        rb.isKinematic = false;

       
        canPush = false;
        
        jiggling = false;
        
     
        
        rb.AddForce(0, EggForce, 0, ForceMode.Impulse);
        timeToRotate = .1f;
        rb.useGravity = true;
        yield return new WaitForSeconds(0.2f);
       
       
        //yield return new WaitForSeconds(1);
      
       

       while (transform.position.y >= egginitialPos.transform.position.y)
       {
            //Debug.Log(localInitialPos.y);
            yield return null;
       }
        transform.parent = dino.transform;
        transform.position = egginitialPos.transform.position;
        rb.isKinematic = true;
        timeToRotate = .7f;
        Debug.Log("posicion igual");
        rb.useGravity = false;
        canPush = true;
        //animator.SetBool("Animate", true);
       // animator.enabled = true;

        yield return null;
    }

    public void EggDeath()
    {
        rb.constraints = RigidbodyConstraints.None;
        eggHit = true;
        canPush = false;
        rb.useGravity = true;
        moduleManager.gameSpeed = 0;
        moduleManager.speedMultiplier = 0;
        dino.GetComponent<PlayerController>().UnsetPlayerInput();
        StopAllCoroutines();
        //Invoke("TimeScaleWithDelay", 1);

        uiManager.DeathPanelActivation();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            if (eggHit == false)
            {
                EggDeath();
            }
            
            //Vector3 tempPos = collision.gameObject.GetComponent<PlayerController>().eggInitialPos.transform.localPosition;

            //transform.localPosition = tempPos;



        }
    }

    private void TimeScaleWithDelay()
    {
        //Time.timeScale = 0;
    }

    private IEnumerator EggRotates()
    {
        float originzRot = transform.localEulerAngles.z;
        float duration = 0;
        float speed = 100;
       

        float startPos = originzRot;
        while (true)
        {
            float destinyAdd = Random.Range(-rotationoffset, rotationoffset);
           
            while (duration<timeToRotate)
            {

               
               
                float zAngle = Mathf.LerpAngle(startPos, originzRot + rotationoffset, duration / timeToRotate);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, zAngle);

              
              
               
                yield return null;
                duration += Time.deltaTime;
            }
            duration = 0;
            startPos = transform.localEulerAngles.z;

            while (duration < timeToRotate)
            {
                float zAngle = Mathf.LerpAngle(startPos, originzRot + -rotationoffset, duration / timeToRotate);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, zAngle);

               
                yield return null;
                duration += Time.deltaTime;
            }
                
            duration = 0;
            startPos = transform.localEulerAngles.z;

            yield return null;
        }
       

       
    }
    private IEnumerator EggJiggleMove()
    {
        jiggling = false;
        Vector3 originPos =transform.position;
        float durationi = 0;


        Vector3 startPos = originPos;
        while (true)
        {
            float destinyAdd = Random.Range(-rotationoffset, rotationoffset);
            
            while (durationi < 1)
            {

                transform.position = Vector3.Lerp(new Vector3(startPos.x, startPos.y, startPos.z),
                    new Vector3(transform.position.x, originPos.y + (0.1f), transform.position.z), durationi / 1);
                durationi += Time.deltaTime;
                Debug.Log(durationi);
                yield return null;
            }
            durationi = 0;
            Debug.Log("Position" + transform.position + new Vector3(transform.position.x, originPos.y + (0.1f), transform.position.z));
            startPos = transform.position;
            Debug.Log("midmove" + startPos);
            yield return null;
            while (durationi < 1)
            {
                transform.position = Vector3.Lerp(new Vector3(startPos.x, startPos.y, startPos.z),
                    new Vector3(transform.position.x, originPos.y + (-0.1f), transform.position.z), durationi / 1);
                durationi += Time.deltaTime;

                yield return null;
            }
            startPos = transform.position;
            Debug.Log("end" + startPos);
            durationi = 0;
            

            yield return null;
        }


        
    }


}
