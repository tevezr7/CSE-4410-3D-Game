using UnityEngine;

public class BaseGun : MonoBehaviour
{
    [SerializeField] public string gunName;
    [SerializeField] public int magSize;
    [SerializeField] public int currentAmmo;
    [SerializeField] public int reserveAmmo;
    [SerializeField] public int maxAmmo;
    [SerializeField] public float damage;
    [SerializeField] public float fireRate;

    public bool CanShoot() => currentAmmo > 0;

    public void Shoot()
    {
        if (!CanShoot()) return;
        currentAmmo--;
    }

    public void Reload()
    {
        int needed = magSize - currentAmmo;
        int taken = Mathf.Min(needed, reserveAmmo);
        currentAmmo += taken;
        reserveAmmo -= taken;
    }

}
