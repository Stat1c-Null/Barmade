using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float move_speed;
    public float walk_speed;
    public float sprint_speed;
    public float ground_drag;
    //Jumping
    public float jump_force;
    public float jump_cooldown;
    public float air_multiplier;
    bool ready_to_jump = true;
    bool grounded;
    //Stamina
    public float stamina_run_use;
    public float stamina;
    bool sprinting;

    [Header("Crouching")]
    public float crouch_speed;
    public float crouchYScale;
    float startYScale;

    [Header("Keyboard key")]
    public KeyCode jump_key  = KeyCode.Space;
    public KeyCode sprint_key = KeyCode.LeftShift;
    public KeyCode crouch_key = KeyCode.LeftControl;

    [Header("Slope Handling")]
    public float max_slope_angle;
    private RaycastHit slope_hit;

    public Transform orientation;

    float horizontal_input;
    float vertical_input;

    Vector3 move_direction;

    Rigidbody rb;

    public MoveState state;

    public enum MoveState
    {
        walk,
        sprint,
        crouch,
        air
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //Get players default size
        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        //Check for ground
        SpeedControl();
        //Input
        ActionInput();
        //Movement states 
        StateHandler();
        //Apply drag
        if (grounded) {
            rb.drag = ground_drag;
        } else {
            rb.drag = 0;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void ActionInput()
    {
        horizontal_input = Input.GetAxisRaw("Horizontal");
        vertical_input = Input.GetAxisRaw("Vertical");

        //Jump
        if(Input.GetKey(jump_key) && ready_to_jump && grounded) {//Also chec k if grounded later on
            ready_to_jump = false;
            Jump();
            Invoke(nameof(ResetJump), jump_cooldown);
        }

        //Crouch
        if(Input.GetKey(crouch_key)) {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            //Push player down to the ground so it wont be floating
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //Stop crouching
        if (Input.GetKeyUp(crouch_key)) {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    //Set speed for different movement states
    private void StateHandler()
    {
        //Mode sprinting
        if(grounded && Input.GetKey(sprint_key))
        {
            state = MoveState.sprint;
            move_speed = sprint_speed;
        }

        //Mode Walking
        else if(grounded) {
            state = MoveState.walk;
            move_speed = walk_speed;
        }

        //Mode crouching
        else if(Input.GetKey(crouch_key)) {
            state = MoveState.crouch;
            move_speed = crouch_speed;
        }

        //Mode Air
        else {
            state = MoveState.air;
        }
    }

    private void MovePlayer() 
    {
        //Calculate movement direction
        move_direction = orientation.forward * vertical_input + orientation.right * horizontal_input;

        //On ground
        if(grounded) {//Walk
            rb.AddForce(move_direction * move_speed * 10f, ForceMode.Force);
        } 
        //In the air
        else if(!grounded) {
            rb.AddForce(move_direction.normalized * move_speed * 10f * air_multiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //Limit velocity if needed
        if(flatVel.magnitude > move_speed)
        {
            Vector3 limited_vel = flatVel.normalized * move_speed;
            rb.velocity = new Vector3(limited_vel.x, rb.velocity.y, limited_vel.z);
        } 
    }

     private void Jump()
    {
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jump_force, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        ready_to_jump = true;
    }

    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }

    void OnCollisionExit(Collision collision) 
    {
        if(collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }

    private bool OnSlope()
    {
        //Detect slope by shooting a raycast laser down
        //if(Physics.Raycast(transform.position, Vector3.down, out slope_hit, play))
        return false;
    }
}
