using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("Movement")]
    private float maxSpeed;
    public float accelerationForce;    
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airSpeedMultiplier;
    public float airDrag;
    public float airGravityMultiplier;
    [Tooltip("Adjust the offset for the jump apex. This will affect when extra gravity kicks in.")]
    public float jumpApexOffset;
    private bool isReadyToJump;
    private bool isJumpingOnSlope;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float originalYScale;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    public float slopeAccelerationForce;
    public float stickToSlopeForce;
    private RaycastHit slopeHit;

    [Header("References")]
    public MovementState state; 
    public Transform orientation; 
    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 moveInput;
    
    #region Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;   
    private InputAction crouchAction; 
    #endregion

    #endregion

    public enum MovementState
    {
        Walking,
        Sprinting,
        Crouching,
        Air
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 

        moveAction =InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        crouchAction = InputSystem.actions.FindAction("Crouch");

        jumpAction.performed += ctx => Jump(); 
        crouchAction.performed += ctx => Crouch();
        crouchAction.canceled += ctx => StopCrouch();  

        originalYScale = transform.localScale.y; 
        isReadyToJump = true;
    }
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        CheckIsGrounded();
        StateHandler();

        if(isGrounded)
            rb.linearDamping = groundDrag;                  // Apply drag when grounded
        else
            rb.linearDamping = airDrag;                     // No drag when in the air
    }

    void FixedUpdate()
    {
        MovePlayer();
        ClampVelocity();
    }

    #region State Handling
    private void StateHandler()
    {
        // Crouching State
        if(crouchAction.IsPressed())
        {
            state = MovementState.Crouching; 
            maxSpeed = crouchSpeed;
        }

        // Sprint State
        else if(isGrounded && sprintAction.IsPressed())
        {
            state = MovementState.Sprinting;
            maxSpeed = sprintSpeed;
        }

        // Walking State
        else if(isGrounded && !sprintAction.IsPressed())
        {
            state = MovementState.Walking;
            maxSpeed = walkSpeed;
        }

        // Air State
        else if(!isGrounded)
        {
            state = MovementState.Air;
        } 
    }
    #endregion

    #region Slope Handling
    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))     // Check if the player is on a slope
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);                                       // Get the angle of the slope

            if(angle < maxSlopeAngle && angle != 0)                                                         // If the angle is less than the max slope angle
                return true; 
        }
        return false; 
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;                           // Project the movement direction onto the slope plane
    }

    #endregion

    #region Movement
    private void MovePlayer()
    {    
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;                                    // Calculate the movement direction based on input and orientation

        if(OnSlope() && !isJumpingOnSlope)
        {
            Vector3 slopeMoveDirection = GetSlopeMoveDirection();                                                
            rb.AddForce(slopeMoveDirection * slopeAccelerationForce, ForceMode.Force);

            if(rb.linearVelocity.y > 0) 
                rb.AddForce(Vector3.down * stickToSlopeForce, ForceMode.Force);                                                 // Apply downward force when going up a slope
        }
        else if(isGrounded) 
            rb.AddForce(moveDirection.normalized * accelerationForce, ForceMode.Force); 

        else if(!isGrounded)
        {
            Vector3 airMove = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;

            rb.AddForce(airMove * (accelerationForce * airSpeedMultiplier), ForceMode.Force); 

            if(rb.linearVelocity.y < 0 + jumpApexOffset) 
                rb.AddForce(Vector3.down * airGravityMultiplier, ForceMode.Force);                                              // Apply downward force when falling
        }

        rb.useGravity = !OnSlope();                                                                                             // Disable gravity when on slope
    }

    private void ClampVelocity()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);                                            // Get the flat velocity (ignore y-axis)

        float currentMaxSpeed = maxSpeed;                                               

        if(flatVel.magnitude > currentMaxSpeed)                                                               
        {
            Vector3 excess = flatVel - (flatVel.normalized * currentMaxSpeed);
            rb.AddForce(-excess, ForceMode.VelocityChange);                                                                     // Apply a force to reduce the velocity to the max speed
        }
    }

    private void CheckIsGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask); 
    }
    #endregion

    #region Crouching
    private void StopCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, originalYScale, transform.localScale.z);                                     // Reset the player's scale to original size
        transform.position = new Vector3(transform.position.x, transform.position.y + (originalYScale - crouchYScale), transform.position.z);   // Adjust the player's position to fit the original scale
    }

    private void Crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);                                       // Change the player's scale to crouch
        transform.position = new Vector3(transform.position.x, transform.position.y - (originalYScale - crouchYScale), transform.position.z);   // Adjust the player's position to fit the crouch scale
    }
    #endregion

    #region Jumping
    private void Jump()
    {
        if (!isReadyToJump || !isGrounded) return;

        isReadyToJump = false; 

        isJumpingOnSlope = true;

        rb.useGravity = true;                                                                                                   // Enable gravity when jumping

        Invoke(nameof(ResetJump), jumpCooldown);                                                                                // Reset the jump cooldown after a delay

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);                                          // Reset the y-velocity before jumping

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);                                                               // Apply the jump force
    }

    private void ResetJump()
    {
        isReadyToJump = true; 
        isJumpingOnSlope = false;
    }
    #endregion

    #region Getters
    public float GetVelocity()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        return flatVel.magnitude; 
    }

    public float GetMaxSpeed()
    {
        return maxSpeed; 
    }
    #endregion

    #region Debugging
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.2f)); 

        if(OnSlope())
        {
            Gizmos.color = Color.green; 
            Gizmos.DrawRay(transform.position, slopeHit.normal); 
        }
    }
    #endregion
}
