using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{

    public Transform Grenade;
    public Transform GrenadeSpawnPoint;
    public float cooldown;
    private bool canShoot;
    public Transform aimPoint;
    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && canShoot)
        {
            canShoot = false;
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(Grenade, GrenadeSpawnPoint.position, aimPoint.rotation);
        StartCoroutine(ShootCooldown(cooldown));
    }

    private IEnumerator ShootCooldown(float timer){
        yield return new WaitForSeconds(timer);
        canShoot = true;
    }
}
