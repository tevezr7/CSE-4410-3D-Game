using UnityEngine;
using System;

public class TurretAI : MonoBehaviour
{
    public float damage = 10f;
    public float fireRate = 60f;
    public float range = 15f;
    public int ammo = 200;

    public Transform turretHead;
    public Transform muzzlePoint;
    public GameObject bulletPrefab;

    private float nextFireTime = 0f;
    private Transform currentTarget;

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
        turretHead.rotation = Quaternion.LookRotation(dir);

        if(Time.time >= nextFireTime)
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
        if(bulletPrefab != null)
        {
            Instantiate(bulletPrefab, muzzlePoint.position, turretHead.rotation);
        }
    }
}
