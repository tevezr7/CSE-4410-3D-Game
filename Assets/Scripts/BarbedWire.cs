using UnityEngine;
using System;

public class BarbedWire : MonoBehaviour
{
    public float slowMultiplier = 0.4f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        ZombieAI zombie = other.GetComponentInParent<ZombieAI>();
        if (zombie != null)
            zombie.speedModifier = slowMultiplier;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) return;
        ZombieAI zombie = other.GetComponentInParent<ZombieAI>();
        if (zombie != null)
            zombie.speedModifier = 1f;
    }

}
