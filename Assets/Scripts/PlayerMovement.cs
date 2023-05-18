using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float move_speed;
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

    [Header("Keyboard key")]
    public KeyCode jump_key  = KeyCode.Space;
    public KeyCode sprint_key = KeyCode.LeftShift;

    public Transform orientation;

    float horizontal_input;
    float vertical_input;

    Vector3 move_direction;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Check for ground
        SpeedControl();
        ActionInput();
        //Apply drag
        if (grounded) {
            rb.drag = ground_drag;
        } else {
            rb.drag = 0;
        }

        //Restore stamina
        if(stamina < 100 && sprinting == false) {
            stamina += stamina_run_use/2 * Time.deltaTime;
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

        //Sprint
        if(Input.GetKey(sprint_key) && stamina > 0) {
            sprinting = true;
            stamina -= stamina_run_use * Time.deltaTime;
        } else {
            sprinting = false;
        }

        //Jump
        if(Input.GetKey(jump_key) && ready_to_jump && grounded) {//Also chec k if grounded later on
            ready_to_jump = false;
            Jump();
            Invoke(nameof(ResetJump), jump_cooldown);
        }
    }

    private void MovePlayer() 
    {
        //Calculate movement direction
        move_direction = orientation.forward * vertical_input + orientation.right * horizontal_input;

        //On ground
        if(grounded && !sprinting) {//Walk
            rb.AddForce(move_direction * move_speed * 10f, ForceMode.Force);
        } else if(grounded && sprinting) {//Run
            rb.AddForce(move_direction * sprint_speed * 10f, ForceMode.Force);
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
        if(!sprinting && flatVel.magnitude > move_speed)
        {
            Vector3 limited_vel = flatVel.normalized * move_speed;
            rb.velocity = new Vector3(limited_vel.x, rb.velocity.y, limited_vel.z);
        } else if(sprinting && flatVel.magnitude > sprint_speed) {
            Vector3 limited_vel = flatVel.normalized * sprint_speed;
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
}
