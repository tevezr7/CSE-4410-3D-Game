using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Shop Interaction")]
    public Transform player;
    public float interactRange = 3f;

    [Header("UI Panels")]
    public GameObject shopPanel;
    public GameObject promptUI;       // "Press F to open shop" prompt
    public GameObject victoryPanel;

    [Header("Prompt")]
    public TextMeshProUGUI promptText;

    [Header("Score Reference")]
    public UIController uiController;

    [Header("Weapon References - Drag from WeaponSwitcher")]
    public GameObject arWeapon;
    public GameObject smgWeapon;
    public GameObject shotgunWeapon;
    public GameObject sniperWeapon;

    [Header("Player Reference")]
    public PlayerCharacter playerCharacter;

    [Header("Item Prices")]
    public int arPrice = 500;
    public int smgPrice = 400;
    public int shotgunPrice = 600;
    public int sniperPrice = 800;
    public int barbedPrice = 300;
    public int minePrice = 700;
    public int turretPrice = 800;
    public int healthUpgradePrice = 700;
    public int healthPrice = 1000;
    public int grenadePrice = 200;
    public int pistolPrice = 150;
    public int winPrice = 9999;


    [Header("Health Upgrade Settings")]
    public float healthUpgradeAmount = 25f;

    [Header("Shop Item UI - Status Texts")]
    public TextMeshProUGUI arStatusText;
    public TextMeshProUGUI smgStatusText;
    public TextMeshProUGUI shotgunStatusText;
    public TextMeshProUGUI sniperStatusText;
    public TextMeshProUGUI turretStatusText;
    public TextMeshProUGUI barbedStatusText;
    public TextMeshProUGUI mineStatusText;
    public TextMeshProUGUI healthUpgradeStatusText;
    public TextMeshProUGUI healthStatusText;
    public TextMeshProUGUI grenadeStatusText;
    public TextMeshProUGUI pistolStatusText;
    public TextMeshProUGUI winStatusText;

    [Header("Misc")]
    public int grenadeMax = 5;

    // Track what has been purchased
    private bool arPurchased = false;
    private bool smgPurchased = false;
    private bool shotgunPurchased = false;
    private bool sniperPurchased = false;
    private bool pistolPurchased = true; // starts with pistol  

    private FPSInput fpsInput;
    private WeaponSwitcher weaponSwitcher;
    private TowerPlacer towerPlacer;
    private VictoryScreen victoryScreen;

    private bool isOpen = false;
    private bool playerInRange = false;

    // Unity Events

    void Start()
    {
        shopPanel.SetActive(false);
        promptUI.SetActive(false);
        fpsInput = FindFirstObjectByType<FPSInput>();
        weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();
        towerPlacer = FindFirstObjectByType<TowerPlacer>();
        victoryScreen = FindFirstObjectByType<VictoryScreen>();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        CheckPlayerDistance();

        // Show prompt when in range and shop is closed
        if (playerInRange && !isOpen)
        {
            promptUI.SetActive(true);
            promptText.text = "Press F to open Shop";
        }
        else if (!playerInRange && !isOpen)
        {
            promptUI.SetActive(false);
        }

        // Open/close shop with F key
        if (playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (isOpen)
                CloseShop();
            else
                OpenShop();
        }

        // Close shop with Escape
        if (isOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseShop();
        }
    }

    // Distance Check 

    void CheckPlayerDistance()
    {
        if (player == null) return;
        float dist = Vector3.Distance(player.position, transform.position);
        playerInRange = dist <= interactRange;

        // Auto close if player walks away
        if (!playerInRange && isOpen)
            CloseShop();
    }

    // Open / Close 

    void OpenShop()
    {
        isOpen = true;
        shopPanel.SetActive(true);
        promptUI.SetActive(false);

        // Pause game while shopping
        Time.timeScale = 0f;

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Refresh all item statuses
        RefreshShopUI();
    }

    public void CloseShop()
    {
        isOpen = false;
        shopPanel.SetActive(false);

        // Resume game
        Time.timeScale = 1f;

        // Lock cursor back for FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // UI Refresh 

    void RefreshShopUI()
    {
        BaseGun arGun = arWeapon?.GetComponentInChildren<BaseGun>();
        BaseGun smgGun = smgWeapon?.GetComponentInChildren<BaseGun>();
        BaseGun shotgunGun = shotgunWeapon?.GetComponentInChildren<BaseGun>();
        BaseGun sniperGun = sniperWeapon?.GetComponentInChildren<BaseGun>();
        BaseGun pistolGun = weaponSwitcher.weapons[0].GetComponentInChildren<BaseGun>();
        UpdateStatus(arStatusText, arPurchased, arPrice, arGun);
        UpdateStatus(smgStatusText, smgPurchased, smgPrice, smgGun);
        UpdateStatus(shotgunStatusText, shotgunPurchased, shotgunPrice, shotgunGun);
        UpdateStatus(sniperStatusText, sniperPurchased, sniperPrice, sniperGun);
        UpdateStatus(pistolStatusText, pistolPurchased, pistolPrice, pistolGun);

        // Traps and health upgrades can be bought multiple times
        if (healthStatusText != null)
            healthStatusText.text = playerCharacter.currentHealth >= playerCharacter.maxHealth ? "FULL" : "Cost: " + healthPrice + " pts  |  " + playerCharacter.currentHealth + "/" + playerCharacter.maxHealth;
        if (winStatusText != null)
            winStatusText.text = "Cost: " + winPrice + " pts  |  Ends the game with a victory";
        if (barbedStatusText != null)
            barbedStatusText.text = "Cost: " + barbedPrice + " pts";
        if (mineStatusText != null)
            mineStatusText.text = "Cost: " + minePrice + " pts";
        if (turretStatusText != null)
            turretStatusText.text = "Cost: " + turretPrice + " pts";
        if (healthUpgradeStatusText != null)
            healthUpgradeStatusText.text = "Cost: " + healthUpgradePrice + " pts  |  +" + healthUpgradeAmount + " Max HP";
        if (grenadeStatusText != null)
            grenadeStatusText.text = fpsInput.grenadeCount >= grenadeMax ? "MAX" : "Cost: " + grenadePrice + " pts  |  " + fpsInput.grenadeCount + "/" + grenadeMax;
    }

    void UpdateStatus(TextMeshProUGUI statusText, bool purchased, int price, BaseGun gun)
    {
        if (statusText == null) return;
        if (!purchased) { statusText.text = "Cost: " + price + " pts"; return; }
        if (gun == null) { statusText.text = "OWNED"; return; }
        if (gun.currentAmmo == gun.magSize && gun.reserveAmmo == gun.maxAmmo)
            statusText.text = "FULL";
        else
            statusText.text = "Refill: " + price + " pts  |  " + gun.currentAmmo + "/" + gun.reserveAmmo;
    }

    //  Purchase Methods (called by buttons) 
    public void BuyWin()
    {
        if (!CanAfford(winPrice)) return;
        Time.timeScale = 0f;
        UIController ui = FindFirstObjectByType<UIController>();
        int score = ui?.score ?? 0;
        int kills = ui?.kills ?? 0;
        FPSInput fpsInput = FindFirstObjectByType<FPSInput>();
        if (fpsInput != null) fpsInput.enabled = false;
        Crosshair crosshair = FindFirstObjectByType<Crosshair>();
        if (crosshair != null) crosshair.enabled = false;
        CloseShop();
        if (victoryScreen != null)
            victoryScreen.ShowVictoryScreen(score, kills);
    }
    public void BuyGrenade()
    {
        if (fpsInput.grenadeCount >= grenadeMax) { ShowMessage("Max grenades!"); return; }
        if (!CanAfford(grenadePrice)) return;
        DeductScore(grenadePrice);
        fpsInput.grenadeCount++;
        ShowMessage("Grenade purchased!");
        RefreshShopUI();
    }

    public void BuyPistolAmmo()
    {
        BaseGun gun = weaponSwitcher.weapons[0].GetComponentInChildren<BaseGun>();
        if (gun == null) return;
        if (gun.currentAmmo == gun.magSize && gun.reserveAmmo == gun.maxAmmo)
        {
            ShowMessage("Ammo is full!"); return;
        }
        if (!CanAfford(pistolPrice)) return;
        DeductScore(pistolPrice);
        gun.reserveAmmo = gun.maxAmmo;
        gun.currentAmmo = gun.magSize;
        ShowMessage("Pistol ammo refilled!");
        RefreshShopUI();
    }
    public void BuyAR()
    {
        Debug.Log($"BuyAR called | arPurchased: {arPurchased} | score: {uiController.score} | arPrice: {arPrice}");
        if (!arPurchased)
        {
            // first purchase — unlock it
            if (!CanAfford(arPrice)) return;
            DeductScore(arPrice);
            arPurchased = true;
            weaponSwitcher.weaponUnlocked[1] = true;
            arWeapon.SetActive(false); // let WeaponSwitcher handle active state
            ShowMessage("AR unlocked!");
        }
        else
        {
            // already owned — check if refill is needed
            BaseGun gun = arWeapon.GetComponentInChildren<BaseGun>();
            if (gun == null) return;
            if (gun.currentAmmo == gun.magSize && gun.reserveAmmo == gun.maxAmmo)
            {
                ShowMessage("Ammo is full!"); return;
            }
            if (!CanAfford(arPrice)) return;
            DeductScore(arPrice);
            gun.reserveAmmo = gun.maxAmmo;
            gun.currentAmmo = gun.magSize;
            ShowMessage("AR ammo refilled!");
        }
        RefreshShopUI();
    }

    public void BuySMG()
    {
        if(!smgPurchased)
        {
            // first purchase — unlock it
            if (!CanAfford(smgPrice)) return;
            DeductScore(smgPrice);
            smgPurchased = true;
            weaponSwitcher.weaponUnlocked[2] = true;
            smgWeapon.SetActive(false); // let WeaponSwitcher handle active state
            ShowMessage("SMG unlocked!");
        }
        else
        {
            // already owned — check if refill is needed
            BaseGun gun = smgWeapon.GetComponentInChildren<BaseGun>();
            if (gun == null) return;
            if (gun.currentAmmo == gun.magSize && gun.reserveAmmo == gun.maxAmmo)
            {
                ShowMessage("Ammo is full!"); return;
            }
            if (!CanAfford(smgPrice)) return;
            DeductScore(smgPrice);
            gun.reserveAmmo = gun.maxAmmo;
            gun.currentAmmo = gun.magSize;
            ShowMessage("SMG ammo refilled!");
        }
        RefreshShopUI();
    }

    public void BuyShotgun()
    {
        if(!shotgunPurchased)
        {
            // first purchase — unlock it
            if (!CanAfford(shotgunPrice)) return;
            DeductScore(shotgunPrice);
            shotgunPurchased = true;
            weaponSwitcher.weaponUnlocked[3] = true;
            shotgunWeapon.SetActive(false); // let WeaponSwitcher handle active state
            ShowMessage("Shotgun unlocked!");
        }
        else
        {
            // already owned — check if refill is needed
            BaseGun gun = shotgunWeapon.GetComponentInChildren<BaseGun>();
            if (gun == null) return;
            if (gun.currentAmmo == gun.magSize && gun.reserveAmmo == gun.maxAmmo)
            {
                ShowMessage("Ammo is full!"); return;
            }
            if (!CanAfford(shotgunPrice)) return;
            DeductScore(shotgunPrice);
            gun.reserveAmmo = gun.maxAmmo;
            gun.currentAmmo = gun.magSize;
            ShowMessage("Shotgun ammo refilled!");
        }
        RefreshShopUI();
    }

    public void BuySniper()
    {
        if(!sniperPurchased)
        {
            // first purchase — unlock it
            if (!CanAfford(sniperPrice)) return;
            DeductScore(sniperPrice);
            sniperPurchased = true;
            weaponSwitcher.weaponUnlocked[4] = true;
            sniperWeapon.SetActive(false); // let WeaponSwitcher handle active state
            ShowMessage("Sniper unlocked!");
        }
        else
        {
            // already owned — check if refill is needed
            BaseGun gun = sniperWeapon.GetComponentInChildren<BaseGun>();
            if (gun == null) return;
            if (gun.currentAmmo == gun.magSize && gun.reserveAmmo == gun.maxAmmo)
            {
                ShowMessage("Ammo is full!"); return;
            }
            if (!CanAfford(sniperPrice)) return;
            DeductScore(sniperPrice);
            gun.reserveAmmo = gun.maxAmmo;
            gun.currentAmmo = gun.magSize;
            ShowMessage("Sniper ammo refilled!");
        }
        RefreshShopUI();
    }

    public void BuyTurret()
    {
        if (!CanAfford(turretPrice)) return;
        DeductScore(turretPrice);
        towerPlacer.AddTower("Turret");
        ShowMessage("Turret purchased! Press T to place.");
        RefreshShopUI();
    }

    public void BuyBarbed()
    {
        if (!CanAfford(barbedPrice)) return;
        DeductScore(barbedPrice);
        towerPlacer.AddTower("BarbedWire");
        ShowMessage("Barbed Wire purchased! Press X to place.");
        RefreshShopUI();
    }

    public void BuyMine()
    {
        if (!CanAfford(minePrice)) return;
        DeductScore(minePrice);
        towerPlacer.AddTower("Mine");
        ShowMessage("Mine purchased! Press Z to place.");
        RefreshShopUI();
    }

    public void BuyHealthUpgrade()
    {
        if (!CanAfford(healthUpgradePrice)) return;
        if (playerCharacter == null) { Debug.LogWarning("PlayerCharacter not assigned!"); return; }

        DeductScore(healthUpgradePrice);
        playerCharacter.maxHealth += healthUpgradeAmount;
        playerCharacter.currentHealth += healthUpgradeAmount;
        ShowMessage("Max health increased by " + healthUpgradeAmount + "!");
        RefreshShopUI();
    }

    public void BuyHealthPotion()
    {
        if (!CanAfford(healthPrice)) return;
        if (playerCharacter.currentHealth >= playerCharacter.maxHealth) { ShowMessage("Already full health!"); return; }
        DeductScore(healthPrice);
        playerCharacter.currentHealth = Mathf.Min(playerCharacter.currentHealth + 50f, playerCharacter.maxHealth);
        ShowMessage("Health Potion purchased!");
        RefreshShopUI();
    }

    // CantAfford and DeductScore

    bool CanAfford(int price)
    {
        if (uiController == null) return false;
        if (uiController.score < price)
        {
            ShowMessage("Not enough points!");
            return false;
        }
        return true;
    }

    void DeductScore(int amount)
    {
        if (uiController == null) return;
        uiController.score -= amount;
        uiController.scoreLabel.text = uiController.score.ToString();
    }

    void ShowMessage(string message)
    {
        Debug.Log("[Shop] " + message);
        // TODO: Hook this up to a UI feedback text label later
    }
}
