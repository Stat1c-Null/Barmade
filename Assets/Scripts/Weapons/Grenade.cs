using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    Rigidbody rb;
    public float strength;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * strength, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
