using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject[] weapons;
    public int currentWeapon = 0;
    public bool[] weaponUnlocked; 
    private RayShooter rayShooter;
    public Transform muzzlePoint;
    private FPSInput fpsInput;

    private void Start()
    {
        rayShooter = FindFirstObjectByType<RayShooter>();
        fpsInput = FindFirstObjectByType<FPSInput>();
        UpdateActiveGun();
        FindFirstObjectByType<AmmoUI>().SetActiveGun(weapons[currentWeapon].GetComponentInChildren<BaseGun>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchTo(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchTo(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchTo(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SwitchTo(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SwitchTo(4);
    }

    public void SwitchTo(int index)
    {
        if (index >= weapons.Length) return;
        if (!weaponUnlocked[index]) return;
        if (fpsInput.isReloading) return;
        weapons[currentWeapon].SetActive(false);
        currentWeapon = index;
        weapons[index].SetActive(true);
        UpdateActiveGun(); // add this
        FindFirstObjectByType<AmmoUI>().SetActiveGun(weapons[currentWeapon].GetComponentInChildren<BaseGun>()); // add this
    }

    private void UpdateActiveGun()
    {
        BaseGun activeGun = weapons[currentWeapon].GetComponentInChildren<BaseGun>();
        Animator anim = weapons[currentWeapon].GetComponentInChildren<Animator>();
        FPSInput fpsInput = FindFirstObjectByType<FPSInput>();

        if (anim != null)
        {
            rayShooter.SetActiveAnimator(anim);
            fpsInput.SetActiveAnimator(anim);
        }

        if (activeGun != null)
        {
            rayShooter.SetActiveGun(activeGun);
            rayShooter.SetMuzzlePoint(activeGun.muzzlePoint);
            fpsInput.SetActiveGun(activeGun);
            FindFirstObjectByType<AmmoUI>().SetActiveGun(activeGun);
        }
    }
}
