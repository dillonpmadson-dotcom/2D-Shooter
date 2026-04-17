using UnityEngine;

// Bullet flies forward, damages anything it hits, destroys itself after a short time
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 10f;

    private Rigidbody2D rigidbodyModule;

    // Unity calls Start() once when the bullet is created
    void Start()
    {
        // Auto-grab the Rigidbody2D on this same GameObject
        rigidbodyModule = GetComponent<Rigidbody2D>();

        // Push the bullet forward in the direction it's facing (transform.up)
        rigidbodyModule.linearVelocity = transform.up * speed;

        // Schedule this bullet to be destroyed after `lifetime` seconds
        // (so we don't leave bullets flying forever — would slow the game down)
        Destroy(gameObject, lifetime);
    }

    // Public setter so the weapon can configure this bullet's damage when firing
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    // Unity calls this when this bullet's collider touches another collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Don't damage the player who fired this bullet
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        // If the thing we hit has a Character script, deal damage to it
        Character victim = collision.gameObject.GetComponentInParent<Character>();
        if (victim != null && victim.healthModule != null)
        {
            victim.healthModule.DecreaseHealth(damage);

            // Kill it if its health hit zero
            if (victim.healthModule.IsDead)
            {
                victim.Die();
            }
        }

        // Destroy the bullet on impact
        Destroy(gameObject);
    }
}
