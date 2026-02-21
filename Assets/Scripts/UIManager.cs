using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] Slider staminaBar;
    private PlayerCharacter playerCharacter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCharacter = FindFirstObjectByType<PlayerCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = playerCharacter.currentHealth / playerCharacter.maxHealth;
        staminaBar.value = playerCharacter.currentStamina / playerCharacter.maxStamina;
    }

    //simple ui script responsible for updating the sliders based on current health and stamina!
}
