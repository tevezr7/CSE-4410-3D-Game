using UnityEngine;
using System.Collections;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject zombiePrefab;
    // These are placeholders for future zombie types - leave unassigned for now
    public GameObject shooterZombiePrefab;
    public GameObject bossZombiePrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 5f;
    public bool startOnPlay = true;

    [Header("Wave UI")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI waveAnnouncementText; // Big text that fades in/out

    // Public so Inspector can see current state
    public int enemiesAlive = 0;
    public int currentWave = 0;

    private bool isWaveActive = false;

    // Unity Events 

    private void Start()
    {
        // Hide announcement text at start
        if (waveAnnouncementText != null)
            waveAnnouncementText.gameObject.SetActive(false);

        CheckForExistingEnemies();

        if (startOnPlay && enemiesAlive <= 0)
            StartWave(1);
    }

    // Context Menu for Editor Testing 

    [ContextMenu("Start First Wave")]
    public void StartFirstWave()
    {
        StartWave(1);
    }

    [ContextMenu("Skip To Next Wave")]
    public void SkipToNextWave()
    {
        // Kill all existing enemies and go to next wave
        GameObject[] existing = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var go in existing)
            Destroy(go);
        enemiesAlive = 0;
        currentWave++;
        StartWave(currentWave);
    }

    // Wave Logic 

    public void StartWave(int waveIndex)
    {
        currentWave = waveIndex;
        enemiesAlive = 0;
        isWaveActive = true;

        // Update wave number on HUD
        UpdateWaveUI();

        // Show wave announcement
        StartCoroutine(ShowWaveAnnouncement(waveIndex));

        // Calculate enemy counts
        int baseZombies = Mathf.RoundToInt(4 * Mathf.Pow(1.5f, waveIndex - 1));
        int shooterCount = GetShooterCount(waveIndex);
        int bossCount = GetBossCount(waveIndex);

        // Always spawn basic zombies
        SpawnEnemies(zombiePrefab, baseZombies);

        // Only spawn shooters/bosses if prefabs are assigned
        if (shooterZombiePrefab != null)
            SpawnEnemies(shooterZombiePrefab, shooterCount);

        if (bossZombiePrefab != null)
            SpawnEnemies(bossZombiePrefab, bossCount);

        Debug.Log($"[EnemySpawner] Wave {waveIndex} started - {baseZombies} zombies");
    }

    private int GetShooterCount(int wave)
    {
        if (wave < 5) return 0;
        if (wave >= 10) return 4 + (wave - 10);
        return 1 + ((wave - 5) / 2);
    }

    private int GetBossCount(int wave)
    {
        if (wave < 10) return 0;
        if (wave == 10) return 1;
        return (wave - 10) / 5 + 1;
    }

    private void SpawnEnemies(GameObject prefab, int count)
    {
        if (prefab == null || spawnPoints.Length == 0) return;

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            var enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            var health = enemy.GetComponent<ReactiveTarget>();
            if (health != null)
                health.OnDeath += HandleEnemyDeath;

            enemiesAlive++;
        }
    }

    public void CheckForExistingEnemies()
    {
        GameObject[] existing = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesAlive = existing.Length;
        currentWave = 1;

        foreach (var go in existing)
        {
            ReactiveTarget rt = go.GetComponent<ReactiveTarget>();
            if (rt != null)
                rt.OnDeath += HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath(ReactiveTarget rt)
    {
        enemiesAlive--;
        Debug.Log($"[EnemySpawner] Enemy died. Remaining: {enemiesAlive}");

        if (enemiesAlive <= 0 && isWaveActive)
        {
            isWaveActive = false;
            StartCoroutine(NextWave());
        }
    }

    private IEnumerator NextWave()
    {
        Debug.Log($"[EnemySpawner] Wave {currentWave} complete! Next wave in {timeBetweenWaves}s");

        // Show wave complete message
        if (waveAnnouncementText != null)
        {
            waveAnnouncementText.gameObject.SetActive(true);
            waveAnnouncementText.text = $"Wave {currentWave} Complete!";
            yield return new WaitForSeconds(2f);
            waveAnnouncementText.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;
        StartWave(currentWave);
    }

    // ?? UI ????????????????????????????????????????????????????????

    void UpdateWaveUI()
    {
        if (waveText != null)
            waveText.text = "Wave: " + currentWave;
    }

    IEnumerator ShowWaveAnnouncement(int wave)
    {
        if (waveAnnouncementText == null) yield break;

        waveAnnouncementText.gameObject.SetActive(true);
        waveAnnouncementText.text = "Wave " + wave;
        yield return new WaitForSeconds(2f);
        waveAnnouncementText.gameObject.SetActive(false);
    }
}