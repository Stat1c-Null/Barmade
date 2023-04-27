using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float move_speed;
    public float ground_drag;
    public float jump_force;
    public float jump_cooldown;
    public float air_multiplier;
    bool ready_to_jump = true;

    [Header("Ground Check")]
    public float player_height;
    public LayerMask ground;
    bool grounded;

    [Header("Keyboard key")]
    public KeyCode jump_key  = KeyCode.Space;

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
        Debug.Log(rb.velocity);
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
    }

    private void MovePlayer() 
    {
        //Calculate movement direction
        move_direction = orientation.forward * vertical_input + orientation.right * horizontal_input;
        rb.AddForce(move_direction * move_speed, ForceMode.Force);
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
}
