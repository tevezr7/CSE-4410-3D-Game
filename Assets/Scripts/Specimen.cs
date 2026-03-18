using UnityEngine;
using UnityEngine.UI;

public class Specimen : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public DeathScreen deathscreen;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] Slider healthBar;

    public void Hurt(float damage)
    {
        health -= damage;
        if (health <= 0) GameOver();
    }

    private void GameOver()
    {
        health = 0;
        UIController ui = FindFirstObjectByType<UIController>();
        int score = 0;
        int kills = 0;
        if (ui != null)
        {
            score = ui.score;
            kills = ui.kills;
        }
        if (deathscreen != null)
            deathscreen.ShowDeathScreen(score, kills);
        else
            Debug.LogWarning("DeathScreen not assigned");
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = health / maxHealth;
    }
}