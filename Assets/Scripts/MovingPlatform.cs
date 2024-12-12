using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body; //The platform's rigidbody

    //necessary values found in the player's movement method
    public float maxSpeed = 5f;
    public float accelerationTime = 0.25f;
    public float decelerationTime = 0.15f;
    private float accelerationRate;
    private float decelerationRate;
    private Vector2 velocity;

    public Coroutine platformMove; //the couroutine that starts the platform's movement pattern
    public float platformSpeed; //the float that determines the speed at which the platform moves

    public float switchTime; //A float tied to time (in seconds) that determines when platform directions should swap
    public float maxTime; //The maximum amount of time a platform moves in a direction
    public bool switchDirection; //Boolean that determines what happens when directions switch
    //True = Right
    //False = Left

    private void Start()
    {
        accelerationRate = maxSpeed / accelerationTime;
        decelerationRate = maxSpeed / decelerationTime;
    }

    public void Update()
    {
        platformMove = StartCoroutine(MovementUpdate()); //Start movement

        //If statement taht determines whether or not the platform is moving left or right
        if (switchTime < 0)
        {
            switchDirection = true; //platform now moves towards the right
        }
        else if (switchTime > maxTime)
        {
            switchDirection = false; //platform now moves towards the left
        }

        //if statement that determines whether or not the timer is decreasing or increasing
        if (switchDirection)
        { //If switchDirection is true
            switchTime += (1 * Time.deltaTime); //switchTime increases
        } else if (!switchDirection) //If switchDirection is false
        {
            switchTime -= (1 * Time.deltaTime); //switchTime decreases
        }

        body.velocity = velocity;
    }
    public IEnumerator MovementUpdate()
    {
        if (switchDirection) //if the platform is going towards the right
        {
            velocity.x += accelerationRate * platformSpeed * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
            velocity.x -= decelerationRate * Time.deltaTime;
            velocity.x = Mathf.Max(velocity.x, 0);
        }
        if (!switchDirection) //if the platform is going towards the left
        {
            velocity.x += accelerationRate * -platformSpeed * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
            velocity.x += decelerationRate * Time.deltaTime;
            velocity.x = Mathf.Min(velocity.x, 0);
        }
        yield return null;
    }
}
