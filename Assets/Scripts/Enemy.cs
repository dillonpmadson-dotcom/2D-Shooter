using UnityEngine;

// Base class for all enemy types
// Subclasses (ExplodingEnemy, MachineGunEnemy, ShooterEnemy) inherit from this
public class Enemy : Character
{
    // Protected so child enemy classes can use it for their own AI
    protected Transform playerTargetTransform;

    // Distance to player (recalculated each frame, used by subclasses)
    protected float distanceToPlayer;

    // Each enemy type can set its own point value via the Inspector
    [SerializeField] protected int scoreValue = 10;

    public override void Start()
    {
        base.Start();

        // Safely look for the Player — if none exists yet, leave target null
        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            playerTargetTransform = player.transform;
        }
    }

    // virtual = subclasses can override and replace this behavior
    public virtual void Update()
    {
        // If the player has been destroyed, do nothing
        if (playerTargetTransform == null) return;

        // Update distance — subclasses use this to decide what to do
        distanceToPlayer = Vector2.Distance(transform.position, playerTargetTransform.position);

        // Default behavior: chase the player
        movementDirection = (playerTargetTransform.position - transform.position).normalized;

        // Always face the player
        Rotate(playerTargetTransform.position);

        Move();
    }

    // Override base Die() to award score before destroying
    public override void Die()
    {
        if (isDead) return;
        ScoreManager.AddScore(scoreValue);
        base.Die(); // base.Die() handles the actual Destroy(gameObject)
    }
}
