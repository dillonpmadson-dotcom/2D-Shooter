using UnityEngine;

// Abstract base class for all pickup types (NukePickup, GunPickup, etc.)
// Handles the common "did the player touch me?" logic.
// Subclasses implement OnPickedUp() with their unique effect.
public abstract class Pickup : MonoBehaviour
{
    [SerializeField] private float lifetime = 15f; // Self-destruct if uncollected
    [SerializeField] private float spinSpeed = 90f; // Visual flair: gentle rotation

    void Start()
    {
        // Auto-destroy if the player never picks it up
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Slow rotation makes pickups look "alive" and noticeable
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    // Unity calls this when something enters our trigger collider
    void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to the Player
        if (other.GetComponentInParent<Player>() == null) return;

        // Subclasses define what happens when picked up
        OnPickedUp(other.GetComponentInParent<Player>());

        // Pickup vanishes after use
        Destroy(gameObject);
    }

    // abstract = subclasses MUST implement this
    protected abstract void OnPickedUp(Player player);
}
