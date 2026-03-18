using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryScreen : MonoBehaviour
{
    [Header("Victory Screen Panel")]
    public GameObject victoryPanel;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI enemiesKilledText;

    // When PlayerCharacter health reaches 0
    public void ShowVictoryScreen(int score, int kills)
    {
        // Pauses the game
        Time.timeScale = 0f;

        // Frees mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show the panel
        victoryPanel.SetActive(true);

        // Show stats
        scoreText.text = "Score: " + score;
        enemiesKilledText.text = "Enemies Killed: " + kills;
    }

    public void OnRestartButton()
    {
        // Resume time before reloading
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitButton()
    {
        // Quits
        Time.timeScale = 1f;
        Application.Quit();

        // Stop Play Mode 
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
