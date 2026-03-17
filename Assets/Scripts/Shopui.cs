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
    public int trapPrice = 300;
    public int healthUpgradePrice = 700;
    public int specimenPotionPrice = 1000;

    [Header("Health Upgrade Settings")]
    public float healthUpgradeAmount = 25f;

    [Header("Shop Item UI - Status Texts")]
    public TextMeshProUGUI arStatusText;
    public TextMeshProUGUI smgStatusText;
    public TextMeshProUGUI shotgunStatusText;
    public TextMeshProUGUI sniperStatusText;
    public TextMeshProUGUI trapStatusText;
    public TextMeshProUGUI healthUpgradeStatusText;
    public TextMeshProUGUI specimenStatusText;

    // Track what has been purchased
    private bool arPurchased = false;
    private bool smgPurchased = false;
    private bool shotgunPurchased = false;
    private bool sniperPurchased = false;
    private bool specimenPotionPurchased = false;

    private bool isOpen = false;
    private bool playerInRange = false;

    // Unity Events

    void Start()
    {
        shopPanel.SetActive(false);
        promptUI.SetActive(false);
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
        UpdateStatus(arStatusText, arPurchased, arPrice);
        UpdateStatus(smgStatusText, smgPurchased, smgPrice);
        UpdateStatus(shotgunStatusText, shotgunPurchased, shotgunPrice);
        UpdateStatus(sniperStatusText, sniperPurchased, sniperPrice);
        UpdateStatus(specimenStatusText, specimenPotionPurchased, specimenPotionPrice);

        // Traps and health upgrades can be bought multiple times
        if (trapStatusText != null)
            trapStatusText.text = "Cost: " + trapPrice + " pts";
        if (healthUpgradeStatusText != null)
            healthUpgradeStatusText.text = "Cost: " + healthUpgradePrice + " pts  |  +" + healthUpgradeAmount + " Max HP";
    }

    void UpdateStatus(TextMeshProUGUI statusText, bool purchased, int price)
    {
        if (statusText == null) return;
        statusText.text = purchased ? "OWNED" : "Cost: " + price + " pts";
    }

    //  Purchase Methods (called by buttons) 

    public void BuyAR()
    {
        if (arPurchased) { ShowMessage("Already owned!"); return; }
        if (!CanAfford(arPrice)) return;

        DeductScore(arPrice);
        arPurchased = true;
        if (arWeapon != null) arWeapon.SetActive(true);
        ShowMessage("AR purchased!");
        RefreshShopUI();
    }

    public void BuySMG()
    {
        if (smgPurchased) { ShowMessage("Already owned!"); return; }
        if (!CanAfford(smgPrice)) return;

        DeductScore(smgPrice);
        smgPurchased = true;
        if (smgWeapon != null) smgWeapon.SetActive(true);
        ShowMessage("SMG purchased!");
        RefreshShopUI();
    }

    public void BuyShotgun()
    {
        if (shotgunPurchased) { ShowMessage("Already owned!"); return; }
        if (!CanAfford(shotgunPrice)) return;

        DeductScore(shotgunPrice);
        shotgunPurchased = true;
        if (shotgunWeapon != null) shotgunWeapon.SetActive(true);
        ShowMessage("Shotgun purchased!");
        RefreshShopUI();
    }

    public void BuySniper()
    {
        if (sniperPurchased) { ShowMessage("Already owned!"); return; }
        if (!CanAfford(sniperPrice)) return;

        DeductScore(sniperPrice);
        sniperPurchased = true;
        if (sniperWeapon != null) sniperWeapon.SetActive(true);
        ShowMessage("Sniper purchased!");
        RefreshShopUI();
    }

    public void BuyTrap()
    {
        if (!CanAfford(trapPrice)) return;

        DeductScore(trapPrice);
        // TODO: Add trap to player inventory when trap system is built
        ShowMessage("Trap purchased!");
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

    public void BuySpecimenPotion()
    {
        if (specimenPotionPurchased) { ShowMessage("Already owned!"); return; }
        if (!CanAfford(specimenPotionPrice)) return;

        DeductScore(specimenPotionPrice);
        specimenPotionPurchased = true;
        // TODO: Apply specimen potion effect when decided
        ShowMessage("Specimen Potion purchased!");
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
