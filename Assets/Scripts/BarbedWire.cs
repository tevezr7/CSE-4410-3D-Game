using UnityEngine;
using System;

public class BarbedWire : MonoBehaviour
{
    public float slowMultiplier = 0.4f;

    private void OnTriggerEnter(Collider other)
    {
        ZombieAI zombie = other.GetComponentInParent<ZombieAI>();
        if(zombie != null)
        {
            zombie.agent.speed *= slowMultiplier;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ZombieAI zombie = other.GetComponentInParent<ZombieAI>();
        if (zombie != null)
        {
            zombie.agent.speed /= slowMultiplier;
        }
    }

}
