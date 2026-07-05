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
    [SerializeField] private float eggCatchmargin;
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

    bool eggHit = false;

    // Cached so we don't repeatedly call GetComponent, and so FixedUpdate
    // can check whether the dino is currently in rolling mode.
    private PlayerController playerController;

    [Header("Rolling")]
    // Tag your scrolling scenery modules with this so the egg knows what
    // counts as "ground" while rolling.
    [SerializeField] private string groundTag = "Ground";
    // Radius of the egg's collider, used to convert world scroll speed
    // into an angular (spin) speed: angularSpeed = groundSpeed / radius.
    [SerializeField] private float eggRadius = 0.5f;
    // Flip this if the egg visibly spins backwards for your scene's
    // forward axis / module travel direction.
    [SerializeField] private bool invertRollDirection = false;
    private int groundContacts = 0;

    // Exposes the egg's own Rigidbody so PlayerController can drive it
    // directly (e.g. to apply rolling forces) without needing a second,
    // easily-desynced reference of its own.
    public Rigidbody Rb => rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Application.targetFrameRate = 120;
        canPush = true;
        playerController = dino.GetComponent<PlayerController>();
        transform.parent = dino.transform;
        transform.position = new Vector3(dino.transform.position.x, egginitialPos.transform.position.y, dino.transform.position.z + zOffset);


        //StartCoroutine(EggRotates());


    }

    // Moves the egg back to its normal resting spot relative to the dino.
    // Called by PlayerController when leaving rolling mode so the egg's
    // usual push/catch behavior can resume cleanly.
    public void ResetToRestingPosition()
    {
        transform.position = new Vector3(dino.transform.position.x, egginitialPos.transform.position.y, dino.transform.position.z + zOffset);
        transform.localEulerAngles = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y < -5f)
        {
            EggDeath();
        }
        // While rolling, the egg's own Rigidbody is being driven by physics
        // (see PlayerController.FixedUpdate/ChangeMode). Don't fight it by
        // forcing a position here - that's only for the push/catch mechanic.
        if (playerController != null && playerController.rolling)
        {
            if (groundContacts > 0)
            {
                ApplyForwardRoll();
            }
            return;
        }

        if (setinitialPosY && canPush && eggHit == false)
        {

            transform.position = new Vector3(dino.transform.position.x, egginitialPos.transform.position.y, dino.transform.position.z + zOffset);
            Debug.Log("InGround" + transform.position);

        }
        else if (setinitialPosY == false)
        {
            Debug.Log("InAir");
            transform.position = new Vector3(dino.transform.position.x, transform.position.y, dino.transform.position.z + zOffset);
        }
        else
        {

            //transform.position = new Vector3(dino.transform.position.x, transform.position.y, dino.transform.position.z + zOffset);

        }



    }

    // Spins the egg forward to match the scenery's scroll speed, so it
    // looks like it's rolling forward even though it's actually staying
    // in place (Z is frozen - see PlayerController.ChangeMode) while the
    // modules slide past underneath it.
    // NOTE: "moduleManager.gameSpeed * moduleManager.speedMultiplier" is a
    // guess at your world-scroll-speed formula based on how EggDeath()
    // zeroes both fields - swap in whatever field actually represents
    // scroll speed (units/sec) in your ModuleManager if this isn't it.
    private void ApplyForwardRoll()
    {
        float groundSpeed = moduleManager.gameSpeed *moduleManager.brakingPercentage/1.5f;
        float angularSpeed = groundSpeed / Mathf.Max(eggRadius, 0.01f);
        if (invertRollDirection) angularSpeed = -angularSpeed;

        // Only touch the roll axis - leave whatever the side-to-side input
        // force in PlayerController is already doing to the other axes alone.
        Vector3 angularVel = rb.angularVelocity;
        angularVel.x = angularSpeed;
        rb.angularVelocity = angularVel;
    }

    // Called by PlayerController right as rolling mode starts, so a stray
    // collision from the mode-switch moment can't leave the contact count
    // in a bad state.
    public void ResetGroundContacts()
    {
        groundContacts = 0;
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
            parentVelocity = parentRb.linearVelocity / 3;
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

        if (transform.position.x < egginitialPos.transform.position.x + eggCatchmargin && transform.position.x > egginitialPos.transform.position.x - eggCatchmargin)
        {
            if (playerController.rolling) yield break;
            transform.parent = dino.transform;
            transform.position = egginitialPos.transform.position;
            rb.isKinematic = true;
            timeToRotate = .7f;
            Debug.Log("posicion igual");
            rb.useGravity = false;
            canPush = true;

        }


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
        moduleManager.speedMultiplier = 0;
        playerController.UnsetPlayerInput();
        StopAllCoroutines();
        //Invoke("TimeScaleWithDelay", 1);
        StartCoroutine(StopTime());
        uiManager.DeathPanelActivation();
    }
    IEnumerator StopTime()
    {
        while (moduleManager.gameSpeed > 0)
        {

            moduleManager.gameSpeed -=0.01f*Time.deltaTime;
        }

        moduleManager.gameSpeed = 0;
        yield return null;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (playerController != null && playerController.rolling)
        {
            if (collision.gameObject.CompareTag(groundTag))
            {
                groundContacts++;
            }
            return;
        }

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

    private void OnCollisionExit(Collision collision)
    {
        if (playerController != null && playerController.rolling && collision.gameObject.CompareTag(groundTag))
        {
            groundContacts = Mathf.Max(0, groundContacts - 1);
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

            while (duration < timeToRotate)
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
        Vector3 originPos = transform.position;
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