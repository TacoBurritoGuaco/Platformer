using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb; //The player's rigidbody
    public enum FacingDirection
    {
        left, right
    }

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
        return false;
    }
    public bool IsGrounded()
    {
        return false;
    }

    public FacingDirection GetFacingDirection()
    {
        return FacingDirection.left;
    }
}
