using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerDirection
{
    left, right
}

public enum PlayerState
{
    idle, walking, jumping, dead,
    dash //An additional player state that determines when the player is dashing
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    private PlayerDirection currentDirection = PlayerDirection.right;
    public PlayerState currentState = PlayerState.idle;
    public PlayerState previousState = PlayerState.idle;

    //MECHANIC #1 - DASh
    //Most of my time here should be spend familiarizing myself with the systems already stablished inside this script
    //My first goal is to figure out how to set my dash state and variables

    public bool hasDashed; //if the player has pressed the dash key
    public float dashForce; //The force the player dashes with

    public float dashCoolMax; //The maximum amount of time the player must wait until they can dash again (in seconds)
    public float dashCooldown; //The amount of time until the player can dash again (in seconds)

    //MECHANIC #2 - JUMP
    //This will use the jump function, and as such, should potentially be a tad easier to set up
    //Specially now that I've done the setup for the dash

    public bool firstJump; //boolean that dictates if this is the first time the character has jumped
    public bool secondJump; //boolean that dictates if the player can jump again
    

    [Header("Horizontal")]
    public float maxSpeed = 5f;
    public float accelerationTime = 0.25f;
    public float decelerationTime = 0.15f;

    [Header("Vertical")]
    public float apexHeight = 3f;
    public float apexTime = 0.5f;

    [Header("Ground Checking")]
    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    private float accelerationRate;
    private float decelerationRate;

    private float gravity;
    private float initialJumpSpeed;

    public Coroutine groundCheck; //The couroutine that checks for the ground
    public float coyoteTime; //The coyote time
    public bool isGrounded = false;
    public bool isDead = false;

    private Vector2 velocity;

    public void Start()
    {
        body.gravityScale = 0;

        accelerationRate = maxSpeed / accelerationTime;
        decelerationRate = maxSpeed / decelerationTime;

        gravity = -2 * apexHeight / (apexTime * apexTime);
        initialJumpSpeed = 2 * apexHeight / apexTime;
    }

    public void Update()
    {
        previousState = currentState;

        groundCheck = StartCoroutine(CheckForGround()); //Starts the ground check coroutine

        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxisRaw("Horizontal");

        if (isDead)
        {
            currentState = PlayerState.dead;
        }

        switch(currentState) //ok so from my understanding of this switch statement,
                             //this is what ultimately determines the current state of the player
        {
            case PlayerState.dead:
                // do nothing - we ded.
                break;
            case PlayerState.idle:
                if (!isGrounded) currentState = PlayerState.jumping;
                else if (hasDashed) currentState = PlayerState.dash; //if the player has dashed
                else if (velocity.x != 0) currentState = PlayerState.walking;
                break;
            case PlayerState.walking:
                if (!isGrounded) currentState = PlayerState.jumping;
                else if (hasDashed) currentState = PlayerState.dash; //if the player has dashed
                else if (velocity.x == 0) currentState = PlayerState.idle;
                break;
            case PlayerState.jumping:
                if (isGrounded)
                {
                    if (velocity.x != 0) currentState = PlayerState.walking;
                    else if (hasDashed) currentState = PlayerState.dash; //if the player has dashed
                    else currentState = PlayerState.idle;
                }
                break;
            case PlayerState.dash:
                if (isGrounded) //If the player is grounded
                {
                    if (velocity.x != 0) currentState = PlayerState.walking; //If they are moving,
                    //Playerstate is = walking
                    else currentState = PlayerState.idle;
                    //If the player is idle
                } else if (!isGrounded) //If the player is not grounded
                    //default to jumping
                {
                    currentState = PlayerState.jumping;
                }
                break;
        }

        MovementUpdate(playerInput);
        JumpUpdate();

        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = 0;

        dashUpdate(); //Updates the dash

        dashCooldown = dashCooldown - (1 * Time.deltaTime); //Lowers the dashCooldown variable over time (seconds)
        dashCooldown = Mathf.Clamp(dashCooldown, 0, dashCoolMax); //Clamps the dashCooldown variable

        body.velocity = velocity;
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if (playerInput.x < 0)
            currentDirection = PlayerDirection.left;
        else if (playerInput.x > 0)
            currentDirection = PlayerDirection.right;

        if (playerInput.x != 0)
        {
            velocity.x += accelerationRate * playerInput.x * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        else
        {
            if (velocity.x > 0)
            {
                velocity.x -= decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Max(velocity.x, 0);
            }
            else if (velocity.x < 0)
            {
                velocity.x += decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Min(velocity.x, 0);
            }
        }
    }

    private void JumpUpdate()
    {
        if (isGrounded && Input.GetButton("Jump")) //When the button is first pressed down
        {
            velocity.y = initialJumpSpeed;
            isGrounded = false;
            firstJump = true; //this is the first jump
        }
        //if the player lets go of the jump button and this is the first jump they've made
        if (firstJump == true && Input.GetButtonUp("Jump"))
        {
            secondJump = true; //the player can now make a second jump
            firstJump = false; //This is no longer the first jump the player made
            //The purpose of this bool is to make it so
            //the player cannot make another jump afterwards
        }
        else if (!isGrounded && Input.GetButton("Jump") && secondJump) //If the player has jumped beforehand
                                                                      //And they try to jump again
                                                                      //If the player is NOT grounded currently
        {
            velocity.y = initialJumpSpeed; //Jump again
            secondJump = false; //reset secondJump to be false
        }
    }
    //This function works similarly to jumpupdate and performs the jump
    //whenever the key is pressed (In this case, E) the player dashes
    private void dashUpdate()
    {
        hasDashed = false; //Set hasDashed to false by default
        if (Input.GetKey(KeyCode.E) && (dashCooldown <= 0)) // PS. Must integrate this into larger button system later
            //If the dash key is pressed and the dash cooldown is less than or equal to 0
        {
            hasDashed = true; //Set hasDashed to true for the next time the playerState switch runs
            dashCooldown = dashCoolMax; //Resets the dashcooldown back to max
            //The force of the initial dash is applied to the x velocity
            if (currentDirection == PlayerDirection.right) velocity.x += dashForce; //Positive force
            if (currentDirection == PlayerDirection.left) velocity.x -= dashForce; //Inverted force (To go opposite way)
        }
    }

    //Turning this into an IEnumerator to run it as a couroutine independant of the rest of the code
    private IEnumerator CheckForGround()
    {
        if (!Physics2D.OverlapBox(
            transform.position + Vector3.down * groundCheckOffset,
            groundCheckSize,
            0,
            groundCheckMask))
        {
            yield return new WaitForSeconds(coyoteTime);
        }
        isGrounded = Physics2D.OverlapBox(
            transform.position + Vector3.down * groundCheckOffset,
            groundCheckSize,
            0,
            groundCheckMask);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckOffset, groundCheckSize);
    }

    public bool IsWalking()
    {
        return velocity.x != 0;
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }

    public PlayerDirection GetFacingDirection()
    {
        return currentDirection;
    }
}
