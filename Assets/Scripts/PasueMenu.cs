using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Settings")]
    public Slider masterVolumeSlider;

    private bool isPaused = false;

    void Start()
    {
        // Make sure both panels are hidden at start
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        // Load saved volume if it exists, default to 1
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = savedVolume;

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = savedVolume;
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // ── Pause / Resume ────────────────────────────────────────────

    public void Pause()
    {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Lock cursor back for FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ── Buttons ───────────────────────────────────────────────────

    public void OnResumeButton()
    {
        Resume();
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitButton()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnSettingsButton()
    {
        // Hide pause buttons, show settings
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnBackButton()
    {
        // Go back from settings to pause menu
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // ── Settings ──────────────────────────────────────────────────

    public void OnMasterVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
}