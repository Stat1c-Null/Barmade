using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{

    public Grenade GrenadeController;
    public Transform Grenade;
    public Transform GrenadeSpawnPoint;
    public float cooldown;
    private bool canShoot;
    public Transform aimPoint;
    public Camera mainCamera;
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
         //create ray from camera to mousePosition
        /*Ray ray = mainCamera.ScreenPointToRay (Input.mousePosition);

        //Create grenade from the prefab
        Grenade newGrenade = Instantiate (GrenadeController.gameObject).GetComponent<Grenade> ();

        //Make the new grenade start at bullet spawn point
        newGrenade.transform.position = GrenadeSpawnPoint.position;

        //set grenade direction
        newGrenade.SetDirection (ray.direction);*/

        StartCoroutine(ShootCooldown(cooldown));
    }

    private IEnumerator ShootCooldown(float timer){
        yield return new WaitForSeconds(timer);
        canShoot = true;
    }
}
