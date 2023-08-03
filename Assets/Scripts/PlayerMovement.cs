using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float move_speed;
    public float walk_speed;
    public float sprint_speed;
    public float ground_drag;
    //Jumping
    [Header("Jumping")]
    public float jump_force;
    public float jump_cooldown;
    public float air_multiplier;
    bool ready_to_jump = true;
    bool grounded;
    //Stamina
    [Header("Sprinting")]
    public float stamina_run_use;
    public float staminaRecharge;
    public float stamina;
    public float max_stamina;
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
    private RaycastHit crouch_hit;
    public LayerMask groundLayer;

    public float player_height;
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

        //Set stamina
        stamina = max_stamina;
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

        //Detect if player is above ground
        if(Physics.Raycast(transform.position, Vector3.down, out crouch_hit, Mathf.Infinity, groundLayer))  {
            Debug.Log("Test");
        }
    }

    private void ActionInput()
    {
        horizontal_input = Input.GetAxisRaw("Horizontal");
        vertical_input = Input.GetAxisRaw("Vertical");

        //Jump
        if(Input.GetKeyDown(jump_key) && ready_to_jump && grounded) {//Also chec k if grounded later on
            ready_to_jump = false;
            Jump();
            Invoke(nameof(ResetJump), jump_cooldown);
        }

        //Crouch
        if(Input.GetKey(crouch_key)) {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            //Push player down to the ground so it wont be floating
            //rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
        }

        //Stop crouching
        if (Input.GetKeyUp(crouch_key)) {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 100f, ForceMode.Impulse);
        }
    }

    //Set speed for different movement states
    private void StateHandler()
    {
        //Mode sprinting
        if(grounded && Input.GetKey(sprint_key) && stamina > 0)
        {
            state = MoveState.sprint;
            move_speed = sprint_speed;
            stamina -= stamina_run_use * Time.deltaTime;
        }

        //Mode Walking
        else if(grounded && !Input.GetKey(crouch_key)) {
            state = MoveState.walk;
            move_speed = walk_speed;
            RestoreStamina();
        }

        //Mode crouching
        else if(Input.GetKey(crouch_key) && grounded) {
            state = MoveState.crouch;
            move_speed = crouch_speed;
            RestoreStamina();
        }

        //Mode Air
        else {
            state = MoveState.air;
        }
    }

    private void RestoreStamina() 
    {
        if(stamina < max_stamina) {
            stamina += staminaRecharge * Time.deltaTime;
        }
    }

    private void MovePlayer() 
    {
        //Calculate movement direction
        move_direction = orientation.forward * vertical_input + orientation.right * horizontal_input;
        //On SLope
        if(OnSlope())
        {
            
            rb.AddForce(GetSlopeMoveDirection() * move_speed * 20f, ForceMode.Force);
            //Push player into the slope when they are walking down
            if(rb.velocity.y > 0) {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        //On ground
        if(grounded) {//Walk
            rb.AddForce(move_direction * move_speed * 10f, ForceMode.Force);
        } 
        //In the air
        else if(!grounded) {
            rb.AddForce(move_direction.normalized * move_speed * 10f * air_multiplier, ForceMode.Force);
        }

        //Tunr off gravity while on the slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        //Limit speed on slope
        if(OnSlope())
        {
            if(rb.velocity.magnitude > move_speed) {
                rb.velocity = rb.velocity.normalized * move_speed;
            }
        }

        //Limiting speed on ground or air
        else {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //Limit velocity if needed
            if(flatVel.magnitude > move_speed)
            {
                Vector3 limited_vel = flatVel.normalized * move_speed;
                rb.velocity = new Vector3(limited_vel.x, rb.velocity.y, limited_vel.z);
            } 
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

    private bool OnSlope()//Detect if player is moving on the slope
    {
        //Detect slope by shooting a raycast laser down
        if(Physics.Raycast(transform.position, Vector3.down, out slope_hit, player_height * 0.5f + 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, slope_hit.normal);//Check if angle is gay
            return angle < max_slope_angle && angle != 0;//True if slope is not straight and angle is bigger then 0
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()//Calculate the direction in which player needs to move on the slope
    {
        return Vector3.ProjectOnPlane(move_direction, slope_hit.normal).normalized;//Make slope movement similar to movement on normal ground
    }
}
