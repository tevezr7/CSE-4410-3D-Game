using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class UIController : MonoBehaviour
{
    // SEB EDITED (changed [SerializedField] to public for shop. DELETE IF NEEDED
    public TMP_Text scoreLabel;
    // SEB END
    [SerializeField] SettingsPopup settingsPopup;

    public int score = 0;
    public int kills = 0;

    // Update is called once per frame

    //void Update()
    //{
    //  scoreLabel.text = Time.realtimeSinceStartup.ToString();
    //}

    //using event system to update score when enemy is hit instead of messenger broadcast
    private void OnEnable()
    {
        GameEvents.OnEnemyKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyKilled -= OnEnemyKilled;
    }

    private void OnEnemyKilled()
    {
        score += 100; // will change in future with if statement for special zombies
        kills += 1;
        scoreLabel.text = score.ToString();
    }


    // SEB ADDED. DELETE IF NEEDED
    // private void Start()
    // {
        // score = 0;
        // scoreLabel.text = score.ToString();

        // settingsPopup.Close();
    // }

    // public void OnOpenSettings()
    // {
        //Debug.Log("Opening Settings...");
        // settingsPopup.Open();
    // }
}
