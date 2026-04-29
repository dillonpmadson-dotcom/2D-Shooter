using UnityEngine;

// Base class for all enemy types
// Subclasses (ExplodingEnemy, MachineGunEnemy, ShooterEnemy) inherit from this
public class Enemy : Character
{
    // Death events — listeners react when any enemy dies
    public static System.Action<Vector3> OnEnemyDied;
    public static System.Action<Vector3, Color> OnEnemyDiedWithColor;

    // Protected so child enemy classes can use it for their own AI
    protected Transform playerTargetTransform;

    // Distance to player (recalculated each frame, used by subclasses)
    protected float distanceToPlayer;

    // Each enemy type configures these in the Inspector
    [SerializeField] protected int scoreValue = 10;
    [SerializeField] protected Color deathParticleColor = Color.white;

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
        // If the player has been destroyed OR is dead, do nothing
        if (playerTargetTransform == null) return;
        Player player = playerTargetTransform.GetComponent<Player>();
        if (player != null && player.IsDead) return;

        // Update distance — subclasses use this to decide what to do
        distanceToPlayer = Vector2.Distance(transform.position, playerTargetTransform.position);

        // Default behavior: chase the player
        movementDirection = (playerTargetTransform.position - transform.position).normalized;

        // Always face the player
        Rotate(playerTargetTransform.position);

        Move();
    }

    // Override base Die() to award score and notify listeners (like PickupSpawner)
    public override void Die()
    {
        if (isDead) return;
        ScoreManager.AddScore(scoreValue);

        // Fire the death events so listeners can react
        OnEnemyDied?.Invoke(transform.position);
        OnEnemyDiedWithColor?.Invoke(transform.position, deathParticleColor);

        base.Die(); // base.Die() handles the actual Destroy(gameObject)
    }
}
