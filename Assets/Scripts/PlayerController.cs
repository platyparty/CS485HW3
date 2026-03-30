using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody rb;
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpSpeed;
    Vector2 moveInput;
    bool jumpInput;
    bool allowJump = false; // jump is allowed when touching a surface
    bool resetInput;
    Animator animator;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Assert(rb, "RigidBody not found");

        animator = GetComponent<Animator>();
        Debug.Assert(animator, "Animator not found");
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        //Debug.Log("Move Triggered: " + moveInput);
    }

    public void OnJump(InputValue value)
    {
        jumpInput = value.isPressed;
        //Debug.Log("Jump Triggered: " + jumpInput);
    }

    public void OnResetPosition(InputValue value)
    {
        resetInput = value.isPressed;
    }

    public void OnDebug(InputValue value)
    {
        Debug.Log("Velocity: " + rb.linearVelocity.magnitude);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Move character based on WASD input
        if (moveInput.magnitude >= 0.01f)
        {
            //Debug.Log("moveInput: " + moveInput);
            Vector3 movementVector = new Vector3(moveInput.x, 0f, moveInput.y);
            movementVector *= moveSpeed;
            rb.AddForce(movementVector, ForceMode.Acceleration);
        }

        float speedBlend = rb.linearVelocity.magnitude / moveSpeed;
        animator.SetFloat("SpeedBlend", speedBlend);

        Vector3 facing = rb.linearVelocity;
        facing.y = 0f;
        if (facing.magnitude > 0.02f)
        {
            transform.LookAt(rb.position + facing);
        }

        // Causes player to jump if allowed
        if (jumpInput && allowJump)
        {
            jumpInput = false; // chatgpt pointed out that i need to reset this flag here.
                                // this makes it so the jump signal only happens once at
                                // a time. otherwise, the capsule endlessly ascended
                                // into the sky on pressing space once.

            allowJump = false;
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

        if (resetInput)
        {
            rb.MovePosition(new Vector3(1,1,1));
            rb.rotation = Quaternion.identity;
            rb.linearVelocity = new Vector3(0,0,0);
            resetInput = false;
        }
    }
    void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground")
            || collision.gameObject.CompareTag("JumpableSurface"))
            {
                Debug.Log("Collision detected; jump allowed");
                // Support for resetting jump logic
                allowJump = true;
            }
        }
}
