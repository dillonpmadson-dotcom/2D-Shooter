using System.Collections.Generic;
using UnityEngine;

// GameManager: spawns random enemy types from screen edges on a timer.
// Difficulty ramps over time: spawn interval shrinks + enemy cap grows.
public class GameManager : MonoBehaviour
{
    [Header("Spawn — Starting Values")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private float startSpawnInterval = 2f;       // Slow start
    [SerializeField] private int startMaxEnemiesAlive = 8;

    [Header("Spawn — End Values (Hardest Difficulty)")]
    [SerializeField] private float minSpawnInterval = 0.5f;       // Fast — but capped
    [SerializeField] private int maxEnemiesAliveCap = 20;

    [Header("Difficulty Ramp")]
    [SerializeField] private float secondsToReachMaxDifficulty = 90f; // After 90s = full difficulty

    [Header("Spawn Position")]
    [SerializeField] private float edgePadding = 1.5f;
    [SerializeField] private Camera mainCamera;

    // Internal timing
    private float timeSinceLastSpawn;
    private float gameStartTime;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        gameStartTime = Time.time;
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        // Calculate the current difficulty level (0 = start, 1 = fully ramped)
        float difficultyT = Mathf.Clamp01((Time.time - gameStartTime) / secondsToReachMaxDifficulty);

        // Lerp = blend the start and end values by the difficulty percentage
        float currentSpawnInterval = Mathf.Lerp(startSpawnInterval, minSpawnInterval, difficultyT);
        int currentMaxEnemies = Mathf.RoundToInt(Mathf.Lerp(startMaxEnemiesAlive, maxEnemiesAliveCap, difficultyT));

        // Bail out if we're at the cap or it's not time yet
        if (timeSinceLastSpawn < currentSpawnInterval) return;
        if (CountAliveEnemies() >= currentMaxEnemies) return;
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return;

        timeSinceLastSpawn = 0f;
        SpawnRandomEnemy();
    }

    private void SpawnRandomEnemy()
    {
        int index = Random.Range(0, enemyPrefabs.Count);
        GameObject prefabToSpawn = enemyPrefabs[index];

        Vector3 spawnPosition = GetRandomEdgePosition();
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    // Returns a world-space point a little past one of the four screen edges
    private Vector3 GetRandomEdgePosition()
    {
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        Vector3 cameraPos = mainCamera.transform.position;

        int edge = Random.Range(0, 4);
        float x = 0f;
        float y = 0f;

        switch (edge)
        {
            case 0: // Top
                x = Random.Range(-cameraWidth, cameraWidth);
                y = cameraHeight + edgePadding;
                break;
            case 1: // Bottom
                x = Random.Range(-cameraWidth, cameraWidth);
                y = -cameraHeight - edgePadding;
                break;
            case 2: // Left
                x = -cameraWidth - edgePadding;
                y = Random.Range(-cameraHeight, cameraHeight);
                break;
            case 3: // Right
                x = cameraWidth + edgePadding;
                y = Random.Range(-cameraHeight, cameraHeight);
                break;
        }

        return new Vector3(cameraPos.x + x, cameraPos.y + y, 0f);
    }

    private int CountAliveEnemies()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        return enemies.Length;
    }
}
