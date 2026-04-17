using System.Collections;
using UnityEngine;

// ExplodingEnemy: charges at the player, then detonates for big damage when close
// Inherits from Enemy → Character
public class ExplodingEnemy : Enemy
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionTriggerDistance = 1.5f;  // How close to start the fuse
    [SerializeField] private float explosionRadius = 2.5f;            // How big the blast is
    [SerializeField] private float explosionDamage = 40f;             // How much damage on detonation
    [SerializeField] private float fuseTime = 0.6f;                   // Telegraph delay before boom

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer bodyRenderer; // The visible shape (drag in via Inspector)
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashInterval = 0.08f; // How fast the flashes alternate

    // Internal state
    private bool isExploding;

    // override = replace base Enemy.Update() with our own behavior
    public override void Update()
    {
        if (playerTargetTransform == null || isExploding) return;

        distanceToPlayer = Vector2.Distance(transform.position, playerTargetTransform.position);
        Rotate(playerTargetTransform.position);

        // Always sprint at the player
        movementDirection = (playerTargetTransform.position - transform.position).normalized;
        Move();

        // Within trigger distance? Light the fuse!
        if (distanceToPlayer <= explosionTriggerDistance)
        {
            StartCoroutine(ExplodeRoutine());
        }
    }

    // override Character.Attack() — Exploding enemy's "attack" IS the explosion
    public override void Attack()
    {
        StartCoroutine(ExplodeRoutine());
    }

    // Coroutine: flash for fuseTime, then deal damage in radius and die
    private IEnumerator ExplodeRoutine()
    {
        isExploding = true;

        // Stop moving during fuse — sit and flash menacingly
        movementDirection = Vector2.zero;

        // Auto-grab the body renderer if it wasn't assigned in Inspector
        if (bodyRenderer == null) bodyRenderer = GetComponent<SpriteRenderer>();

        // Remember the original color so we can flip back to it between flashes
        Color originalColor = bodyRenderer != null ? bodyRenderer.color : Color.white;

        // Flash back-and-forth between original and flash color until the fuse runs out
        float elapsed = 0f;
        bool isWhite = false;
        while (elapsed < fuseTime)
        {
            if (bodyRenderer != null)
            {
                bodyRenderer.color = isWhite ? originalColor : flashColor;
            }
            isWhite = !isWhite;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        // BOOM — find everything in explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            // Damage any Character we hit (player or other enemies)
            Character victim = hit.GetComponentInParent<Character>();
            if (victim != null && victim != this && victim.healthModule != null)
            {
                victim.healthModule.DecreaseHealth(explosionDamage);
            }
        }

        // Self-destruct (we exploded, after all)
        Destroy(gameObject);
    }

    // Visualize the explosion radius in the Scene view (Editor-only debug aid)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
