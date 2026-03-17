using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public float maxHealth = 50;
    public float currentHealth;
    public float damage = 10;
    public float maxStamina = 100;
    public float currentStamina;
    private float staminaCost = 10;
    private float staminaRegen = 5;

    //SEB ADDED. DELETE IF NEEDED
    public DeathScreen deathscreen;
    //end

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hurt(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            //SEB ADDED. DELETE IF NEEDED
            Die();
            //SEB END

            Debug.Log("Player is dead");
            //no negative health allowed!! 
        }
        else
        {
            Debug.Log("Player hit! Current health: " + currentHealth);
        }
    }

    //SEB ADDED. DELETE IF NEEDED
    private void Die()
    {
        // Get score and kill count (from UIController)
        UIController ui = FindFirstObjectByType<UIController>();
        int score = 0;
        int kills = 0;

        if (ui != null)
        {
            score = ui.score;
            kills = ui.kills;
        }

        // Shows Death Screen
        if (deathscreen != null)
            deathscreen.ShowDeathScreen(score, kills);
        else
            Debug.LogWarning("DeathScreen not assigned");

        // Disables player input after death
        FPSInput fpsInput = GetComponentInChildren<FPSInput>();
        if (fpsInput != null)
            fpsInput.enabled = false;
    }
        //SEB END

            public void StaminaDrain()
            {
                currentStamina -= staminaCost * Time.deltaTime;
                if (currentStamina < 0)
                {
                    currentStamina = 0;
                }
            }

    public void StaminaRegen()
    {
        currentStamina += staminaRegen * Time.deltaTime; 
        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
    }

}
