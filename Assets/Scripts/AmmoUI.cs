using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    private BaseGun activeGun;

    public void SetActiveGun(BaseGun gun)
    {
        activeGun = gun;
    }

    void Update()
    {
        if (activeGun == null)
        {
            Debug.Log("AmmoUI: activeGun is null");
            return;
        }
        Debug.Log($"AmmoUI: {activeGun.currentAmmo} / {activeGun.reserveAmmo}");
        ammoText.text = $"{activeGun.currentAmmo} / {activeGun.reserveAmmo}";
    }
}