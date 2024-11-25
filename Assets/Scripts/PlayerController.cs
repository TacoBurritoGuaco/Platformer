using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb; //The player's rigidbody
    public bool moving; //Whether or not the player is moving
    public enum FacingDirection
    {
        left, right
    }
    FacingDirection dir; //facingDirection variable used to set the current direction being faced

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        //Going right 
        if (Input.GetKey(KeyCode.D))
        {
            playerInput.x = 2f;
        }
        //Going left
        if (Input.GetKey(KeyCode.A))
        {
            playerInput.x = -2f;
        }
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
       rb.AddForce(playerInput); //Adds force in the form of the player input variable
    }

    public bool IsWalking()
    {
        //if either key is being held down (meaning the player is attempting to move)
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            moving = true; //set to true
        } else
        {
            //Else, set to false
            moving = false;
        }
        return moving;
        
    }
    public bool IsGrounded()
    {
        return false;
    }

    public FacingDirection GetFacingDirection()
    {
        //If going left or has just stopped going left
        if (Input.GetKey(KeyCode.D) || Input.GetKeyUp(KeyCode.D))
        {
            dir = FacingDirection.right; //Set dir to right
        }
        //If going left or has just stopped going left
        if (Input.GetKey(KeyCode.A) || Input.GetKeyUp(KeyCode.A))
        {
            dir = FacingDirection.left; //set dir to left
        }
        return dir; //This way, if the player was facing a direction,
                    //the player still faces that direction after
                    //They have stopped moving at all.
    }
}
