using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject shooterZombiePrefab;
    public GameObject bossZombiePrefab;
    public Transform[] spawnPoints;

    public int enemiesAlive = 0;
    public int currentWave = 0;

    private void Start()
    {
        CheckForExistingEnemies();
    }

    public void StartWave(int waveIndex)
    {
        currentWave = waveIndex;
        enemiesAlive = 0;

        int baseZombies = Mathf.RoundToInt(4 * Mathf.Pow(1.5f, waveIndex - 1));
        int shooterCount = GetShooterCount(waveIndex);
        int bossCount = GetBossCount(waveIndex);

        SpawnEnemies(zombiePrefab, baseZombies);
        SpawnEnemies(shooterZombiePrefab, shooterCount);
        SpawnEnemies(bossZombiePrefab, bossCount);
    }

    private int GetShooterCount(int wave)
    {
        if (wave < 5) return 0;
        if (wave >= 10)
            return 4 + (wave - 10); // 4 at wave 10, +1 every wave after
        // wave 5 = 1, wave 7 = 2, wave 9 = 3
        return 1 + ((wave - 5) / 2);
    }

    private int GetBossCount(int wave)
    {
        if (wave < 10) return 0;
        if (wave == 10) return 1;
        // one boss every 5 waves after 10
        return (wave - 10) / 5 + 1;
    }

    private void SpawnEnemies(GameObject prefab, int count)
    {
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
        if (enemiesAlive <= 0)
        {
            StartCoroutine(NextWave());
        }
    }

    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(5f); // between wave delay
        currentWave++;
        StartWave(currentWave);
    }
}