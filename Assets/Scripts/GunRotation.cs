using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotation : MonoBehaviour
{

    public Transform camera;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.rotation;
    }
}
