using UnityEngine;
using UnityEngine.UI;

public class Specimen : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    private Specimen specimen;
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
        Time.timeScale = 0f;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        specimen = FindFirstObjectByType<Specimen>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = specimen.health / specimen.maxHealth;
    }
}