using System;
using UnityEngine;
using UnityEngine.Audio;

public class TurretAI : MonoBehaviour
{
    public float damage = 10f;
    public float fireRate = 5f;
    public float range = 15f;
    public int ammo = 200;

    public Transform turretHead;
    public Transform muzzlePoint;

    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioSource audioSource;

    private float nextFireTime = 0f;
    private Transform currentTarget;
    public BulletTrail bulletTrailPrefab;

    private void Start()
    {
        nextFireTime = Time.time + (1f / fireRate);
    }

    void Update()
    {
        if (ammo <= 0)
        {
            Destroy(gameObject, 2f); // destroy after 2 seconds to allow for death animation
            return;
        }

        currentTarget = GetNearestZombie();

        if (currentTarget == null) return;

        Vector3 dir = currentTarget.position - turretHead.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            turretHead.rotation = Quaternion.RotateTowards(turretHead.rotation, targetRot, 180f * Time.deltaTime);
        }



        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    private Transform GetNearestZombie()
    {
        Transform nearest = null;
        float minDist = range;

        foreach (ZombieAI zombie in ZombieAI.allZombies)
        {
            if(zombie == null) continue;
            float dist = Vector3.Distance(transform.position, zombie.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = zombie.transform;
            }
        }
        return nearest;
    }

    private void Fire()
    {
        ammo--;
        if (audioSource != null && fireSound != null)
            audioSource.PlayOneShot(fireSound);
        int mask = ~LayerMask.GetMask("Player", "Gun", "Arms", "BulletTrail");
        Ray ray = new Ray(muzzlePoint.position, turretHead.forward);
        Vector3 endPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, range, mask))
        {
            ReactiveTarget target = hit.collider.GetComponentInParent<ReactiveTarget>();
            if (target != null) target.ReactToHit(damage);
            endPoint = hit.point;
        }
        else
        {
            endPoint = muzzlePoint.position + turretHead.forward * range;
        }
        if (bulletTrailPrefab != null)
        {
            BulletTrail trail = Instantiate(bulletTrailPrefab, muzzlePoint.position, Quaternion.identity);
            trail.SpawnTrail(muzzlePoint.position, endPoint);
        }
    }
}
