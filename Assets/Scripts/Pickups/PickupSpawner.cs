using System.Collections.Generic;
using UnityEngine;

// Listens for enemy deaths and rolls for a random pickup drop at the death location.
// Drag pickup prefabs into the pickupPrefabs list in the Inspector.
public class PickupSpawner : MonoBehaviour
{
    [Header("Drop Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.25f;       // 25% of kills drop a pickup
    [SerializeField] private List<GameObject> pickupPrefabs; // Drag NukePickup + GunPickup here

    // Subscribe to the enemy death event when this object turns on
    void OnEnable()
    {
        Enemy.OnEnemyDied += HandleEnemyDeath;
    }

    // ALWAYS unsubscribe when turned off — prevents memory leaks
    void OnDisable()
    {
        Enemy.OnEnemyDied -= HandleEnemyDeath;
    }

    // Called automatically every time any Enemy dies
    private void HandleEnemyDeath(Vector3 deathPosition)
    {
        // Bail if no pickups configured
        if (pickupPrefabs == null || pickupPrefabs.Count == 0) return;

        // Roll the dice — only drop sometimes
        if (Random.value > dropChance) return;

        // Pick a random pickup prefab from the list
        int index = Random.Range(0, pickupPrefabs.Count);
        GameObject prefabToDrop = pickupPrefabs[index];

        // Spawn it at the death position
        Instantiate(prefabToDrop, deathPosition, Quaternion.identity);
    }
}
