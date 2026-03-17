using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CompassController2 : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public RectTransform compassStrip;
    public TextMeshProUGUI compassText;

    [Header("Compass Settings")]
    public float compassWidth = 2000f;
    // How quickly the compass strip slides to the new position (higher = snappier)
    public float smoothSpeed = 8f;

    [Header("Enemy Dots")]
    // Assign a small red dot UI Image prefab here in the Inspector
    public GameObject enemyDotPrefab;
    // Parent RectTransform to hold the dots (e.g. the CompassUI object)
    public RectTransform dotContainer;
    // How far left/right on the strip an enemy dot can appear (in pixels)
    public float dotRangeWidth = 500f;
    // Only show dots for enemies within this world-unit distance
    public float enemyDetectionRange = 50f;

    // ── Private state ─────────────────────────────────────────────
    private float currentOffset;
    private float targetOffset;

    // Pool of dot images so we never allocate per-frame
    private List<RectTransform> dotPool = new List<RectTransform>();

    // ─────────────────────────────────────────────────────────────
    void Update()
    {
        if (player == null) return;

        UpdateCompassStrip();
        UpdateEnemyDots();
    }

    // ── Smooth compass strip scrolling ────────────────────────────
    void UpdateCompassStrip()
    {
        float playerY = player.eulerAngles.y;

        // Target pixel offset based on player facing direction
        targetOffset = (playerY / 360f) * compassWidth;

        // Smooth towards target using Lerp on the raw value
        // We work in raw pixel space so no angle-wrap issues here
        currentOffset = Mathf.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothSpeed);

        compassStrip.anchoredPosition = new Vector2(-currentOffset, 0);
        compassText.text = GetDirection(playerY);
    }

    // ── Enemy dot radar ───────────────────────────────────────────
    void UpdateEnemyDots()
    {
        if (enemyDotPrefab == null || dotContainer == null) return;

        // Find all zombies currently in the scene
        ZombieAI[] zombies = FindObjectsByType<ZombieAI>(FindObjectsSortMode.None);

        // Grow pool if needed
        while (dotPool.Count < zombies.Length)
        {
            GameObject dot = Instantiate(enemyDotPrefab, dotContainer);
            dotPool.Add(dot.GetComponent<RectTransform>());
        }

        float playerY = player.eulerAngles.y;

        for (int i = 0; i < dotPool.Count; i++)
        {
            if (i >= zombies.Length)
            {
                // Hide unused dots
                dotPool[i].gameObject.SetActive(false);
                continue;
            }

            Transform zombie = zombies[i].transform;
            float dist = Vector3.Distance(player.position, zombie.position);

            if (dist > enemyDetectionRange)
            {
                dotPool[i].gameObject.SetActive(false);
                continue;
            }

            // Angle from player to zombie in world space
            Vector3 dir = zombie.position - player.position;
            float angleToEnemy = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            if (angleToEnemy < 0) angleToEnemy += 360f;

            // Relative angle: how far left/right is the enemy from where player faces
            float relativeAngle = Mathf.DeltaAngle(playerY, angleToEnemy);

            // Map relative angle (-180..180) to pixel offset on the strip
            // Clamp so dots don't fly off the edge of the compass
            float dotX = (relativeAngle / 180f) * dotRangeWidth;

            // Only show dot if it's within the visible compass width
            if (Mathf.Abs(dotX) > dotRangeWidth)
            {
                dotPool[i].gameObject.SetActive(false);
                continue;
            }

            dotPool[i].gameObject.SetActive(true);
            dotPool[i].anchoredPosition = new Vector2(dotX, 0f);
        }
    }

    // ── Direction label ───────────────────────────────────────────
    string GetDirection(float angle)
    {
        if (angle >= 337.5f || angle < 22.5f) return "N";
        if (angle < 67.5f) return "NE";
        if (angle < 112.5f) return "E";
        if (angle < 157.5f) return "SE";
        if (angle < 202.5f) return "S";
        if (angle < 247.5f) return "SW";
        if (angle < 292.5f) return "W";
        return "NW";
    }
}