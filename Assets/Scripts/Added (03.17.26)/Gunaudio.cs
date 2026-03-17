using UnityEngine;

// Attach this script to each weapon GameObject (Pistol, AR, SMG, Shotgun, Sniper, Knife)
// Each weapon gets its own audio clips assigned in the Inspector

public class GunAudio : MonoBehaviour
{
    [Header("Audio Source")]
    // The AudioSource component on this weapon - add one to each weapon GameObject
    public AudioSource audioSource;

    [Header("Gun Sounds")]
    public AudioClip fireSound;         // Sound when firing
    public AudioClip reloadSound;       // Sound when reloading
    public AudioClip emptyClickSound;   // Sound when trying to fire with no ammo

    [Header("Knife Sounds")]
    public AudioClip knifeSwipeSound;   // Only used on the Knife GameObject

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float fireVolume = 1f;
    [Range(0f, 1f)]
    public float reloadVolume = 0.8f;
    [Range(0f, 1f)]
    public float emptyVolume = 0.6f;

    // Public called by BaseGun or FPSInput 

    public void PlayFireSound()
    {
        if (fireSound == null) return;
        audioSource.PlayOneShot(fireSound, fireVolume);
    }

    public void PlayReloadSound()
    {
        if (reloadSound == null) return;
        audioSource.PlayOneShot(reloadSound, reloadVolume);
    }

    public void PlayEmptyClick()
    {
        if (emptyClickSound == null) return;
        audioSource.PlayOneShot(emptyClickSound, emptyVolume);
    }

    public void PlayKnifeSwipe()
    {
        if (knifeSwipeSound == null) return;
        audioSource.PlayOneShot(knifeSwipeSound, fireVolume);
    }
}