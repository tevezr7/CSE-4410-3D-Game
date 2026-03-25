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
            return;
        }
        ammoText.text = $"{activeGun.currentAmmo} / {activeGun.reserveAmmo}";
    }
}
