using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;


public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;    
    private Rigidbody rigidbody;   

    [SerializeField] private float SideVelocity;
    [SerializeField] private float jumpForce;

    bool canJump =true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         playerInput = GetComponent<PlayerInput>();
         rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 Movement = playerInput.actions["Move"].ReadValue<Vector2>();
        float ValueX = Movement.x;

        rigidbody.AddRelativeForce(ValueX * SideVelocity ,0,0, ForceMode.Force);




    }

    public void Jump(InputAction.CallbackContext context)
    {
        switch(context.phase)
        {
            case InputActionPhase.Started:
                if (canJump)
                {
                    canJump = false;
                    rigidbody.AddForce(0, jumpForce, 0, ForceMode.Force);
                }
                

                return;



        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 surfaceNormal = collision.GetContact(0).normal;

        if (Vector3.Angle(surfaceNormal, Vector3.up) < 90)
        {
            Debug.Log(Vector3.Angle(surfaceNormal, Vector3.up));
            canJump = true;
        }
    }
}
