using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    Rigidbody rb;
    public float strength;
    Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * strength, ForceMode.Impulse);
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    // Update is called once per frame
    void Update()
    {
        //rb.AddForce(direction * strength, ForceMode.Impulse);
        
    }
}
