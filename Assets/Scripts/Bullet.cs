using UnityEngine;

// Bullet flies forward, damages anything it hits (except its own shooter), self-destructs after a time
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 10f;

    private Rigidbody2D rigidbodyModule;

    // Who fired this bullet — used so the bullet doesn't damage its own shooter
    private Character owner;

    void Start()
    {
        rigidbodyModule = GetComponent<Rigidbody2D>();
        rigidbodyModule.linearVelocity = transform.up * speed;

        // Auto-destroy after lifetime so old bullets don't pile up
        Destroy(gameObject, lifetime);
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    // Set by the weapon when it fires — tells the bullet who shot it
    public void SetOwner(Character newOwner)
    {
        owner = newOwner;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Find the Character we hit (might be on this collider, or its parent)
        Character victim = collision.gameObject.GetComponentInParent<Character>();

        // Don't damage our own shooter
        if (victim != null && victim == owner)
        {
            return;
        }

        // Apply damage if we hit any other Character
        if (victim != null && victim.healthModule != null)
        {
            victim.healthModule.DecreaseHealth(damage);
            if (victim.healthModule.IsDead)
            {
                victim.Die();
            }
        }

        // Bullet vanishes on impact (whether it hit a Character or a wall)
        Destroy(gameObject);
    }
}
