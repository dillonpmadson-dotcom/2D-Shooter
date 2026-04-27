using UnityEngine;

// ShooterEnemy: keeps distance from player, fires slow but accurate shots
// Inherits from Enemy (which inherits from Character)
public class ShooterEnemy : Enemy
{
    [Header("Shooter Settings")]
    [SerializeField] private float preferredDistance = 6f;   // Tries to stay this far from player
    [SerializeField] private float distanceTolerance = 0.5f; // Dead zone around preferred distance

    [Header("Weapon Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.33f;  // 0.33 shots/sec = 1 shot per 3 seconds
    [SerializeField] private float bulletDamage = 15f;

    // The actual weapon instance — built in Start()
    private RangedWeapon weapon;

    public override void Start()
    {
        base.Start(); // Enemy.Start() finds the player + sets up health
        weapon = new RangedWeapon(fireRate, bulletDamage, bulletPrefab);
    }

    // override = replace the base Enemy.Update() with our own behavior
    public override void Update()
    {
        if (playerTargetTransform == null) return;

        // Recalculate distance and face the player (we still want this from base)
        distanceToPlayer = Vector2.Distance(transform.position, playerTargetTransform.position);
        Rotate(playerTargetTransform.position);

        // Decide whether to back away, hold position, or close in
        if (distanceToPlayer < preferredDistance - distanceTolerance)
        {
            // Too close — back away from player
            movementDirection = (transform.position - playerTargetTransform.position).normalized;
            Move();
        }
        else if (distanceToPlayer > preferredDistance + distanceTolerance)
        {
            // Too far — close in slowly
            movementDirection = (playerTargetTransform.position - transform.position).normalized;
            Move();
        }
        else
        {
            // In the sweet spot — stop and shoot
            movementDirection = Vector2.zero;
        }

        // Always try to shoot (the weapon's fire rate enforces cooldown)
        Attack();
    }

    // Override Character.Attack() — the Shooter's attack is firing the weapon
    public override void Attack()
    {
        if (weapon != null && firePoint != null)
        {
            weapon.Use(firePoint.position, firePoint.rotation, this);
        }
    }
}
