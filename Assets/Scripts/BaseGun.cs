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

    public bool CanShoot() => currentAmmo > 0;
    public enum FireMode { Single, Auto }
    public FireMode fireMode;

    public void Shoot()
    {
        if (!CanShoot()) return;
        currentAmmo--;
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
        isReloading = true;
    }

    public void OnReloadEnd()
    {
        isReloading = false;
    }

}
