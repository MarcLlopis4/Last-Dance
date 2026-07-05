using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI;


public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody rigidbody;

    [Header("RollingSettings")]
    [SerializeField] private Collider dragonCollider;
    [SerializeField] private Transform dragonRigTrans;
    [SerializeField] private Transform rollingRef;
    [SerializeField] private Animator eggAnimator;
    [SerializeField] public Animator dragonAnim;
    [SerializeField] private float rollAnimTransSpeed;
    [SerializeField] private float SideVelocity;
    [SerializeField] private float jumpForce;
    [SerializeField] private LevelManager lM;
    public Egg egg;
    [SerializeField] public GameObject eggInitialPos;
    [SerializeField] public GameObject camClosePos;
    [SerializeField] public CameraControll cam;
    [SerializeField] public ModuleManager mM;
    [SerializeField] public PhysicsMaterial wallFloorMat;
    public bool rolling;

    bool canJump = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        rigidbody = GetComponent<Rigidbody>();
        //Vector3 initialPos = eggInitialPos.transform.position;
        //egg.transform.position = initialPos;
    }

    public void CameraTurn()
    {
        cam.CameraTurn(camClosePos.transform.position);
    }
    public void ChangeMode()
    {
        rolling = !rolling;
        mM.rolling = rolling;
        eggAnimator.enabled = !rolling;
        dragonAnim.SetBool("Grabbing", rolling);

        // Standing/pushing mode: egg is locked to X/Z and can't rotate,
        // since Egg.cs manually positions it for the push/catch mechanic.
        RigidbodyConstraints constraintsA = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        // Rolling mode: only Z is locked (forward progress is handled
        // elsewhere, e.g. ModuleManager scrolling). X and rotation stay
        // free so the egg can actually roll side to side.
        RigidbodyConstraints constraintsB = RigidbodyConstraints.FreezePositionZ;

        if (rolling)
        {
            //StartCoroutine(MoveTo(Vector3.zero, rollingRef.transform.localPosition));
            dragonRigTrans.localPosition = rollingRef.transform.localPosition;
            // Detach the egg so its own Rigidbody is free to move/rotate
            // instead of being dragged around as a child transform.
            egg.transform.SetParent(null, true);
            egg.ResetGroundContacts();
            egg.Rb.isKinematic = false;
            egg.Rb.useGravity = true;
            egg.Rb.constraints = constraintsB;
            foreach(Transform child in transform)
            {
                if (child.gameObject.name.StartsWith("VF"))
                {
                    child.gameObject.SetActive(false);
                }
            }
            // The dino's own body is now just a passenger; the egg is what
            // actually touches the ground and moves.
            rigidbody.isKinematic = true;
        }
        else
        {
            //StartCoroutine(MoveTo(dragonRigTrans.localPosition, Vector3.zero));

            // Hand the egg back to its normal push/catch behavior.
           
            egg.Rb.isKinematic = true;
            egg.Rb.constraints = constraintsA;
            egg.transform.SetParent(transform, true);
            egg.ResetToRestingPosition();
            egg.setinitialPosY = true;
            egg.canPush = true;
            foreach (Transform child in transform)
            {
                if (child.gameObject.name.StartsWith("VF"))
                {
                    child.gameObject.SetActive(true);
                }
            }
            rigidbody.isKinematic = false;
        }
        egg.canPush = !rolling;
        dragonRigTrans.SetParent(rolling ? egg.transform : transform, false);
        if (dragonRigTrans.parent == transform)
        {
            dragonRigTrans.transform.localPosition = Vector3.zero;
            dragonRigTrans.transform.localEulerAngles = Vector3.zero;
            rigidbody.constraints = RigidbodyConstraints.FreezePositionZ|RigidbodyConstraints.FreezeRotation;
            Debug.Log("RsetPossssss");
        }
        dragonCollider.enabled = !rolling;
        rigidbody.useGravity = !rolling;
       
        
    }

    void LateUpdate()
    {
        if (!rolling) return;

        // The dino's own rigidbody is kinematic while rolling (see
        // ChangeMode), so keep its root position following the egg on the
        // X axis - that's the axis the player actually controls. Y and Z
        // stay whatever the rest of the game (gravity-less anchor / world
        // scroll) already gives it.
        Vector3 pos = transform.position;
        pos.x = egg.transform.position.x;
        transform.position = pos;
    }

    IEnumerator MoveTo(Vector3 origin, Vector3 target)
    {
        float timePass = 0;
        while ((target - origin).magnitude > 0.01f)
        {
            timePass += Time.deltaTime;
            dragonRigTrans.localPosition = Vector3.Lerp(origin, target, timePass * rollAnimTransSpeed);
            yield return null;
        }
        if (!rolling)
        {
            
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerInput != null)
        {
            Vector2 Movement = playerInput.actions["Move"].ReadValue<Vector2>();
            float ValueX = Movement.x;
            //float ValueY = Movement.y;
            if (mM.isTurning) ValueX = 0;
            Debug.Log("ValueX" + ValueX);
            if (!rolling)
            {
                rigidbody.AddRelativeForce(ValueX * SideVelocity, 0, 0, ForceMode.Force);
            }
            else
            {
                Vector3 force = Camera.main.transform.right * ValueX;
                // Apply above the egg's center so the force also creates
                // torque, making it actually roll instead of just sliding.
                egg.Rb.AddForceAtPosition(force * SideVelocity/9f, egg.transform.position + Vector3.up * 0.5f, ForceMode.Force);
                
            }
            //if(playerInput.actions["Move"].ReadValue<Vector2>().x != 0)
            //{
            //    wallFloorMat.dynamicFriction = 0f;
            //}
            //else
            //{
            //    wallFloorMat.dynamicFriction = 0.3f;
            //}
        }
    }

    public void UnsetPlayerInput()
    {

        jumpForce = 0;
        playerInput = null;
        this.enabled = false;
    }
    public void Brake(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:

                mM.braking = true;

                return;

            case InputActionPhase.Canceled:


                mM.braking = false;

                return;



        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (rolling) return;

        switch (context.phase)
        {
            case InputActionPhase.Started:
                if (canJump)
                {
                    canJump = false;
                    if (egg.canPush == true)
                    {
                        egg.transform.parent = this.gameObject.transform;
                        egg.setinitialPosY = true;
                    }
                    //egg.StopJiggling();

                    rigidbody.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);

                    AudioManager.audioInstance.PlaySFX("Jump", transform.position);
                }


                return;



        }
    }

    public void EggPush(InputAction.CallbackContext context)
    {
        if (rolling) return;
        switch (context.phase)
        {
            case InputActionPhase.Started:

                if (egg.canPush == true)
                {

                    egg.PushUp(eggInitialPos.transform.position);
                    AudioManager.audioInstance.PlaySFX("LanzarHuevo", transform.position);
                }


                return;



        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        
        switch (context.phase)
        {
            case InputActionPhase.Started:

                Time.timeScale = 0;
                lM.OpenPanel(lM.pausePanel);


                return;



        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rolling) return;
        Vector3 surfaceNormal = collision.GetContact(0).normal;

        if (Vector3.Angle(surfaceNormal, Vector3.up) == 0)
        {
            Debug.Log(Vector3.Angle(surfaceNormal, Vector3.up));
            canJump = true;
            egg.setinitialPosY = true;
            //egg.transform.parent = this.transform;
        }
    }
}