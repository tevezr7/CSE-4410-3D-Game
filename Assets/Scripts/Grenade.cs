using UnityEngine;
using System;

public class Grenade : Explode
{
    public float fuseTime = 6f;
    public float throwForce = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke(nameof(Explosion), fuseTime);
        Debug.Log($"Grenade spawned at: {transform.position}");
    }

    public void Throw()
    {
        if (rb != null)
        {
            rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        }
    }

}
