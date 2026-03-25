using UnityEngine;
using System.Collections;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject zombiePrefab;
    public GameObject shooterZombiePrefab;
    public GameObject bossZombiePrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 5f;
    public bool startOnPlay = true;

    [Header("Wave UI")]
    public TextMeshProUGUI waveText;

    // Public so Inspector can see current state
    public int enemiesAlive = 0;
    public int currentWave = 0;

    private bool isWaveActive = false;

    private void Start()
    {
        // Pick up any pre-placed enemies as wave 1
        CheckForExistingEnemies();

        if (waveText != null)
            waveText.text = "Wave: 1";

        if (startOnPlay)
        {
            currentWave = 1;
            isWaveActive = true;

            // If no pre-placed enemies, spawn wave 1 normally
            if (enemiesAlive <= 0)
                SpawnWaveEnemies(1);
        }
    }

    // Context Menu for Editor Testing

    /*[ContextMenu("Start First Wave")]
    public void StartFirstWave()
    {
        StartWave(1);
    }

    [ContextMenu("Skip To Next Wave")]
    public void SkipToNextWave()
    {
        GameObject[] existing = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var go in existing)
            Destroy(go);
        enemiesAlive = 0;
        currentWave++;
        StartWave(currentWave);
    } */

    // Wave Logic

    public void StartWave(int waveIndex)
    {
        currentWave = waveIndex;
        isWaveActive = true;
        UpdateWaveUI();
        SpawnWaveEnemies(waveIndex);
    }

    private void SpawnWaveEnemies(int waveIndex)
    {
        int baseZombies = Mathf.RoundToInt(4 * Mathf.Pow(1.5f, waveIndex - 1));
        int shooterCount = GetShooterCount(waveIndex);
        int bossCount = GetBossCount(waveIndex);

        SpawnEnemies(zombiePrefab, baseZombies);

        if (shooterZombiePrefab != null)
            SpawnEnemies(shooterZombiePrefab, shooterCount);

        if (bossZombiePrefab != null)
            SpawnEnemies(bossZombiePrefab, bossCount);

        Debug.Log($"[EnemySpawner] Wave {waveIndex} started — {enemiesAlive} total enemies alive");

        // Safety: if nothing actually spawned, don't deadlock
        if (enemiesAlive <= 0)
        {
            Debug.LogWarning("[EnemySpawner] No enemies spawned! Check prefabs and spawnPoints in Inspector.");
            isWaveActive = false;
        }
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
        if (prefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning($"[EnemySpawner] Can't spawn: prefab={prefab}, spawnPoints={spawnPoints?.Length ?? 0}");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            var enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            var health = enemy.GetComponent<ReactiveTarget>();
            if (health != null)
                health.OnDeath += HandleEnemyDeath;
            else
                Debug.LogWarning($"[EnemySpawner] Spawned enemy has no ReactiveTarget!");

            enemiesAlive++;
        }
    }

    public void CheckForExistingEnemies()
    {
        // Reset before counting to prevent double-counting
        Debug.Log("[EnemySpawner] CheckForExistingEnemies called");

        enemiesAlive = 0;

        GameObject[] existing = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesAlive = existing.Length;

        foreach (var go in existing)
        {
            ReactiveTarget rt = go.GetComponent<ReactiveTarget>();
            if (rt != null)
            {
                // Unsubscribe first to prevent duplicate subscriptions
                rt.OnDeath -= HandleEnemyDeath;
                rt.OnDeath += HandleEnemyDeath;
            }
        }

        Debug.Log($"[EnemySpawner] Found {enemiesAlive} pre-placed enemies");
    }

    private void HandleEnemyDeath(ReactiveTarget rt)
    {
        // Unsubscribe so we never double-fire
        rt.OnDeath -= HandleEnemyDeath;

        enemiesAlive--;
        Debug.Log($"[EnemySpawner] Enemy died. Remaining: {enemiesAlive} | isWaveActive: {isWaveActive}");

        if (enemiesAlive <= 0 && isWaveActive)
        {
            isWaveActive = false;
            StartCoroutine(NextWave());
        }
    }

    private IEnumerator NextWave()
    {
        Debug.Log($"[EnemySpawner] Wave {currentWave} complete! Next wave in {timeBetweenWaves}s");
        yield return new WaitForSeconds(timeBetweenWaves);
        currentWave++;
        StartWave(currentWave);
    }

    private void UpdateWaveUI()
    {
        if (waveText != null)
            waveText.text = "Wave: " + currentWave;
    }
}