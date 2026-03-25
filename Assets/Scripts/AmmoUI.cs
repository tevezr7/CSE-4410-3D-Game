using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI grenadeText;
    private BaseGun activeGun;
    private FPSInput fpsInput;

    void Start()
    {
        fpsInput = FindFirstObjectByType<FPSInput>();
    }
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
        if (grenadeText != null && fpsInput != null)
            grenadeText.text = "Grenades:" + fpsInput.grenadeCount;
    }
}
