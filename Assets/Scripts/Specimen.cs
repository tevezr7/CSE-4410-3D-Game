using UnityEngine;

public class Specimen : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    [SerializeField] private GameObject gameOverPanel;

    public void Hurt(float damage)
    {
        health -= damage;
        if (health <= 0) GameOver();
    }

    private void GameOver()
    {
        health = 0;
        Time.timeScale = 0f;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}