using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb; //The player's rigidbody

    public BoxCollider2D box; //The player's box collider
    public CompositeCollider2D tile; //The tilemap's collider (Note: used to be tilemapcollider
    //But I realized what the player was colliding with was not the tiles specifically)

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
        //If the box collider is touching the tilemap collider
        if (box.IsTouching(tile))
        {
            Debug.Log("On Ground");
            return true; //The player is grounded
        }
        else //Otherwise, the player is not grounded
        {
            Debug.Log("In Air");
            return false;
        }  
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
