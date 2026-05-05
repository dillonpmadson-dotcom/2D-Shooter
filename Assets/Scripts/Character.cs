using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Character : MonoBehaviour
{
    protected Vector2 movementDirection;
    protected bool isDead;

    // Public read-only access — anyone can check if this character is dead
    public bool IsDead => isDead;


    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Rigidbody2D RigidbodyModule;
    [SerializeField] protected float maxHealth = 100f;

    public Health healthModule;

    // Cached sprite renderers + original colors so the hit-flash always restores correctly
    private SpriteRenderer[] cachedRenderers;
    private Color[] cachedOriginalColors;
    private Coroutine activeFlashCoroutine;

    public virtual void Start()
    {
        // Auto-grab Rigidbody2D if not manually assigned (saves dragging it in the Inspector)
        if (RigidbodyModule == null)
        {
            RigidbodyModule = GetComponent<Rigidbody2D>();
        }

        healthModule = new Health(maxHealth);

        // Cache sprite renderers ONCE so the hit-flash always knows the true colors
        cachedRenderers = GetComponentsInChildren<SpriteRenderer>();
        cachedOriginalColors = new Color[cachedRenderers.Length];
        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            cachedOriginalColors[i] = cachedRenderers[i].color;
        }
    }


    public void Move()
    {
        // Set velocity directly — crisp, responsive, jitter-free movement.
        // movementDirection is already normalized (length 1) so speed = moveSpeed in units/sec.
        RigidbodyModule.linearVelocity = movementDirection * moveSpeed;
    }

    public void Rotate(Vector3 directionToRotate)
    {
        transform.up = directionToRotate - transform.position;
    }

    public virtual void Attack()
    {

    }

    // Central damage entry point — handles HP decrease, hit flash, and death check
    public virtual void TakeDamage(float amount)
    {
        if (isDead || healthModule == null) return;

        healthModule.DecreaseHealth(amount);

        // If a previous flash is still running, cancel it so the new one runs cleanly
        if (activeFlashCoroutine != null) StopCoroutine(activeFlashCoroutine);
        activeFlashCoroutine = StartCoroutine(FlashOnHit());

        if (healthModule.IsDead)
        {
            Die();
        }
    }

    // Brief white flash, then restore to the cached original colors
    private IEnumerator FlashOnHit()
    {
        if (cachedRenderers == null) yield break;

        // Switch every cached renderer to white
        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            if (cachedRenderers[i] != null) cachedRenderers[i].color = Color.white;
        }

        yield return new WaitForSeconds(0.08f);

        // Restore using the original colors we captured in Start()
        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            if (cachedRenderers[i] != null) cachedRenderers[i].color = cachedOriginalColors[i];
        }

        activeFlashCoroutine = null;
    }

    // Called when this character runs out of health.
    // virtual = subclasses (Enemy, Player) can override for custom death behavior
    public virtual void Die()
    {
        if (isDead) return; // Prevent dying twice in the same frame
        isDead = true;
        Destroy(gameObject);
    }
}
