using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    private int currentWeapon = 0;
    private RayShooter rayShooter;

    private void Start()
    {
        rayShooter = GetComponent<RayShooter>();
        UpdateActiveGun();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchTo(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchTo(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchTo(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SwitchTo(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SwitchTo(4);
        if (Keyboard.current.vKey.wasPressedThisFrame || Mouse.current.backButton.wasPressedThisFrame) SwitchTo(5);
    }

    private void SwitchTo(int index)
    {
        if (index >= weapons.Length) return;
        weapons[currentWeapon].SetActive(false);
        currentWeapon = index;
        weapons[index].SetActive(true);
    }

    private void UpdateActiveGun()
    {
        BaseGun activeGun = weapons[currentWeapon].GetComponentInChildren<BaseGun>();
        rayShooter.SetActiveGun(activeGun);
    }
}
