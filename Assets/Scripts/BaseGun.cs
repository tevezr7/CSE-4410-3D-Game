using UnityEngine;

public class BaseGun : MonoBehaviour
{
    public string gunName;
    public int magSize;
    public int currentAmmo;
    public int reserveAmmo;
    public int maxAmmo;
    public float damage;
    public float fireRate;
    public bool isReloading;
    public float recoilStrength;
    public float recoilHorizontal;
    public Transform muzzlePoint;
    // SEB ADDED (For gun sounds), DELETE IF NEEDED
    public GunAudio gunAudio;
    // SEB END

    public bool CanShoot() => currentAmmo > 0;
    public enum FireMode { Single, Auto }
    public FireMode fireMode;

    public void Shoot()
    {
        // SEB EDITED, EDIT IF NEEDED
        if (!CanShoot())
        {
            //Play empty click when trying to fire with no ammo
            if (gunAudio != null) gunAudio.PlayEmptyClick();
            return;
        }
            currentAmmo--;
        // Play fire sound
        if (gunAudio != null) gunAudio.PlayFireSound();
        // SEB END
   
    }

    public void Reload()
    {
        Debug.Log("Reload called!");
        int needed = magSize - currentAmmo;
        int taken = Mathf.Min(needed, reserveAmmo);
        currentAmmo += taken;
        reserveAmmo -= taken;
        Debug.Log($"After reload: {currentAmmo} / {reserveAmmo}");
    }

    public void OnReloadStart()
    {
        // SEB EDIT
        isReloading = true;
        // Play reload sound
        if (gunAudio != null) gunAudio.PlayReloadSound();
        // SEB END
    }

    public void OnReloadEnd()
    {
        isReloading = false;
    }

}
