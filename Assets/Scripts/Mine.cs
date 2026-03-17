using UnityEngine;
using System;

public class Mine : Explode
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Explosion();
        }
    }

}
