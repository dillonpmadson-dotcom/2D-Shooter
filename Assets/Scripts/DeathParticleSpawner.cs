using UnityEngine;

// Listens for enemy deaths and spawns a colored particle burst at the death position.
public class DeathParticleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject deathParticlePrefab;

    void OnEnable()
    {
        Enemy.OnEnemyDiedWithColor += SpawnAt;
    }

    void OnDisable()
    {
        Enemy.OnEnemyDiedWithColor -= SpawnAt;
    }

    private void SpawnAt(Vector3 position, Color color)
    {
        if (deathParticlePrefab == null) return;

        // Spawn the particle prefab and tint it to match the enemy color
        GameObject burst = Instantiate(deathParticlePrefab, position, Quaternion.identity);
        BurstParticle burstScript = burst.GetComponent<BurstParticle>();
        if (burstScript != null)
        {
            burstScript.SetColor(color);
        }
    }
}
