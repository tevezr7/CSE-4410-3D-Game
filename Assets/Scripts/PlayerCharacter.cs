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

    //added stamina system to allow player to sprint and perform actions that consume stamina, with regeneration over time. will add dodging and sliding in the future! :D

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
            Debug.Log("Player is dead");
            //no negative health allowed!! 
        }
        else
        {
            Debug.Log("Player hit! Current health: " + currentHealth);
        }
    }

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
