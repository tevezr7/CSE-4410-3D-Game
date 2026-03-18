using UnityEngine;
using System;

public class Explode : MonoBehaviour
{
    public float damage = 100f;
    public float radius = 5f;
    public LayerMask damageLayer;
    public GameObject explosionEffectPrefab;

    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioSource audioSource;

    public void Explosion()
    {
        if (audioSource != null && explosionSound != null)
            audioSource.PlayOneShot(explosionSound);
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, radius, damageLayer);
        foreach (Collider hit in hits)
        {
            ReactiveTarget target = hit.GetComponentInParent<ReactiveTarget>();
            if (target != null)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                float damageMult = 1f - (dist / radius); //damage decreases with distance
                target.ReactToHit(damage * damageMult);
            }
        }

        Destroy(gameObject, 1f);
    }



}
