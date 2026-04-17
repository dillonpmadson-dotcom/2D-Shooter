using System.Collections.Generic;
using UnityEngine;

// GameManager: spawns random enemy types from screen edges on a timer
// Drag your enemy prefabs into the enemyPrefabs list in the Inspector
public class GameManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private List<GameObject> enemyPrefabs;  // Drag ShooterEnemy, ExplodingEnemy, MachineGunEnemy here
    [SerializeField] private float spawnInterval = 2f;       // Seconds between spawn attempts
    [SerializeField] private int maxEnemiesAlive = 8;        // Hard cap so the screen isn't overrun
    [SerializeField] private float edgePadding = 1.5f;       // How far OUTSIDE the screen edge to spawn

    [Header("References")]
    [SerializeField] private Camera mainCamera;              // If left null, GameManager grabs Camera.main

    // Internal timer
    private float timeSinceLastSpawn;

    void Start()
    {
        // Auto-grab the main camera if not assigned in the Inspector
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        // Tick the spawn timer
        timeSinceLastSpawn += Time.deltaTime;

        // Bail out if we're at the cap or it's not time yet
        if (timeSinceLastSpawn < spawnInterval) return;
        if (CountAliveEnemies() >= maxEnemiesAlive) return;
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return;

        // Reset timer and spawn one enemy
        timeSinceLastSpawn = 0f;
        SpawnRandomEnemy();
    }

    // Picks a random prefab and a random off-screen edge position, then spawns it
    private void SpawnRandomEnemy()
    {
        // Pick a random enemy type from the list
        int index = Random.Range(0, enemyPrefabs.Count);
        GameObject prefabToSpawn = enemyPrefabs[index];

        // Pick a random point just OUTSIDE the screen edges
        Vector3 spawnPosition = GetRandomEdgePosition();

        // Spawn it!
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    // Returns a world-space point a little past one of the four screen edges
    private Vector3 GetRandomEdgePosition()
    {
        // Get the camera's view bounds in world space
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        Vector3 cameraPos = mainCamera.transform.position;

        // Pick which edge to spawn from (0=top, 1=bottom, 2=left, 3=right)
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

    // Counts how many Enemy-derived objects are currently alive in the scene
    private int CountAliveEnemies()
    {
        // FindObjectsByType is the modern replacement for FindObjectsOfType
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        return enemies.Length;
    }
}
