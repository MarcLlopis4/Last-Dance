using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Egg : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float EggForce;
    public GameObject dino;

    public bool canPush;
    public float zOffset;
    public GameObject egginitialPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        canPush = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(dino.transform.position.x, transform.position.y, dino.transform.position.z + zOffset);
    }

    public void PushUp(Vector3 localinitialPos)
    {
        StartCoroutine(Pushing(localinitialPos));
    }

    private IEnumerator Pushing(Vector3 localInitialPos)
    {
        canPush = false;
        transform.parent = null;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.AddRelativeForce(0, -EggForce, 0, ForceMode.Impulse);
        rb.useGravity = true;
        yield return new WaitForSeconds(0.2f);
       
       
        //yield return new WaitForSeconds(1);
      
        float timeLerp = 0;
        float maxtimeLerp = 1;

       while (transform.position.y >= egginitialPos.transform.position.y)
       {
            Debug.Log(localInitialPos.y);
            yield return null;
       }
        transform.position = egginitialPos.transform.position;
        transform.parent = dino.transform;
        rb.isKinematic = true;
        Debug.Log("posicion igual");
        rb.useGravity = false;
        canPush = true;
       
        yield return null;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Vector3 tempPos = collision.gameObject.GetComponent<PlayerController>().eggInitialPos.transform.localPosition;

            //transform.localPosition = tempPos;



        }
    }


}
