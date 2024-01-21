using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    private Gun Gun;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            OnShoot();
        } else {
            StopShoot();
        }
    }

    public void OnShoot()
    {
        Gun.Shoot();
    }

    public void StopShoot(){
        Gun.StopShoot();
    }
}
