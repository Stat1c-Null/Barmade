using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Gun : MonoBehaviour
{
    [SerializeField]
    private bool AddBulletSpread = true;
    [SerializeField]
    private Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField]
    private ParticleSystem ShootingSystem;
    [SerializeField]
    private Transform BulletSpawnPoint;
    [SerializeField]
    private ParticleSystem ImpactParticleSystem;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    private float ShootDelay;
    [SerializeField]
    private LayerMask Mask;

    private Animator Animator;
    private float LastShootTime;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void Shoot()
    {
        if (LastShootTime + ShootDelay < Time.time)
        {
            //TODO later watch video

            Animator.SetBool("IsShooting", true);
            ShootingSystem.Play();
            Vector3 direction = GetDirection();

            if (Physics.Raycast(BulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
            {
                
                TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                //Spawn and render the trail 
                StartCoroutine(SpawnTrail(trail, hit));

                LastShootTime = Time.time;
            }
        }


    }

    public void StopShoot()
    {
        ShootingSystem.Stop();
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;
        //If bullet spread is on add some randomization
        if(AddBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                Random.Range(-BulletSpreadVariance.y, BulletSpreadVariance.y),
                Random.Range(-BulletSpreadVariance.z, BulletSpreadVariance.z)
            );

            direction.Normalize();
        }

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPos = Trail.transform.position;

        //move trail from spawn point to hit point over some time
        while (time < 1)
        {
            Trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / Trail.time;

            yield return null;
        }
        Animator.SetBool("IsShooting", false);
        //Make sure particles are facing direction of the shot
        Trail.transform.position = hit.point;
        Instantiate(ImpactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(Trail.gameObject, Trail.time);
    }
}
