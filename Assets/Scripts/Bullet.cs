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

        // Add a glowing trail behind the bullet (looks great with bloom)
        AddTrail();

        // Auto-destroy after lifetime so old bullets don't pile up
        Destroy(gameObject, lifetime);
    }

    private void AddTrail()
    {
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.15f;          // How long the trail lasts behind the bullet
        trail.startWidth = 0.18f;    // Width near the bullet (tip)
        trail.endWidth = 0.0f;       // Tapers to nothing at the tail
        trail.minVertexDistance = 0.05f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = new Color(1f, 0.95f, 0.3f, 1f); // bright yellow head
        trail.endColor = new Color(1f, 0.95f, 0.3f, 0f);   // fade to transparent
        trail.sortingOrder = 1;
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
        // TakeDamage handles the hit-flash, death-check, and death event for us
        if (victim != null)
        {
            victim.TakeDamage(damage);
        }

        // Bullet vanishes on impact (whether it hit a Character or a wall)
        Destroy(gameObject);
    }
}
