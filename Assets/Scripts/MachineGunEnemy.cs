using UnityEngine;

// MachineGunEnemy: chases the player and sprays inaccurate bullets at high rate
// Inherits from Enemy → Character
public class MachineGunEnemy : Enemy
{
    [Header("Range Settings")]
    [SerializeField] private float shootingRange = 8f; // Only shoots when within this distance

    [Header("Weapon Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 5f;       // 5 bullets per second
    [SerializeField] private float bulletDamage = 5f;   // Low damage per bullet (sprays many)
    [SerializeField] private float spreadAngle = 15f;   // How inaccurate the gun is (degrees)

    private RangedWeapon weapon;

    public override void Start()
    {
        base.Start(); // Enemy.Start() finds the player + sets up health
        weapon = new RangedWeapon(fireRate, bulletDamage, bulletPrefab);
    }

    // override Enemy.Update() — this is our custom AI
    public override void Update()
    {
        if (playerTargetTransform == null) return;

        // Recalc distance + face the player
        distanceToPlayer = Vector2.Distance(transform.position, playerTargetTransform.position);
        Rotate(playerTargetTransform.position);

        // Always chase the player (slower than Exploding so we can shoot while moving)
        movementDirection = (playerTargetTransform.position - transform.position).normalized;
        Move();

        // Open fire when in range
        if (distanceToPlayer <= shootingRange)
        {
            Attack();
        }
    }

    // override Character.Attack() — fire a bullet with random spread
    public override void Attack()
    {
        if (weapon == null || firePoint == null) return;

        // Calculate a random spread offset (between -spreadAngle and +spreadAngle degrees)
        float randomSpread = Random.Range(-spreadAngle, spreadAngle);

        // Build a rotation: firepoint's rotation + the random spread on the Z axis
        // (2D rotation happens around Z because the camera looks down the Z axis)
        Quaternion spreadRotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpread);

        // Fire — RangedWeapon's fireRate cooldown still applies inside .Use()
        weapon.Use(firePoint.position, spreadRotation);
    }
}
