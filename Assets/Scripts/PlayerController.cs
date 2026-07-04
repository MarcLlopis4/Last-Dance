using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;


public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;    
    private Rigidbody rigidbody;   

    [SerializeField] private float SideVelocity;
    [SerializeField] private float jumpForce;
    [SerializeField] private Egg egg;
    [SerializeField] public GameObject eggInitialPos;
    [SerializeField] public GameObject camClosePos;
    [SerializeField] public CameraControll cam;
    [SerializeField] public ModuleManager mM;
        

    bool canJump =true;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         playerInput = GetComponent<PlayerInput>();
         rigidbody = GetComponent<Rigidbody>();
        //Vector3 initialPos = eggInitialPos.transform.position;
       // egg.transform.position = initialPos;
    }

    public void CameraTurn()
    {
        cam.CameraTurn(camClosePos.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playerInput != null)
        {
            Vector2 Movement = playerInput.actions["Move"].ReadValue<Vector2>();
            float ValueX = Movement.x;
            //float ValueY = Movement.y;

            Debug.Log("ValueX" + ValueX);
            rigidbody.AddRelativeForce(ValueX * SideVelocity, 0, 0, ForceMode.Force);
        }
        




    }

    public void UnsetPlayerInput()
    {
        playerInput = null;
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
        switch(context.phase)
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
                }
                

                return;



        }
    }

    public void EggPush(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:

                if (egg.canPush == true)
                {
                   
                    egg.PushUp(eggInitialPos.transform.position);
                }


                return;



        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 surfaceNormal = collision.GetContact(0).normal;

        if (Vector3.Angle(surfaceNormal, Vector3.up) == 0)
        {
            Debug.Log(Vector3.Angle(surfaceNormal, Vector3.up));
            canJump = true;
            egg.setinitialPosY = true;
            egg.transform.parent = null;
        }
    }
}
