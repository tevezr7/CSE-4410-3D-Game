using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class UIController : MonoBehaviour
{
    [SerializeField] TMP_Text scoreLabel;
    [SerializeField] SettingsPopup settingsPopup;

    private int score = 0;

    // Update is called once per frame

    //void Update()
    //{
    //  scoreLabel.text = Time.realtimeSinceStartup.ToString();
    //}

    //using event system to update score when enemy is hit instead of messenger broadcast
    private void OnEnable()
    {
        GameEvents.OnEnemyHit += OnEnemyHit;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyHit -= OnEnemyHit; 
    }

    private void OnEnemyHit()
    {
        score += 1;
        scoreLabel.text = score.ToString();
    }



    private void Start()
    {
        score = 0;
        scoreLabel.text = score.ToString();

        settingsPopup.Close();
    }

    public void OnOpenSettings()
    {
        //Debug.Log("Opening Settings...");
        settingsPopup.Open();
    }
}
