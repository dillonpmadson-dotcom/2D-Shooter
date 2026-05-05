using UnityEngine;

// Abstract base class for all pickup types (NukePickup, GunPickup, etc.)
// Handles the common "did the player touch me?" logic.
// Subclasses implement OnPickedUp() with their unique effect.
public abstract class Pickup : MonoBehaviour
{
    [SerializeField] private float lifetime = 15f;     // Self-destruct if uncollected
    [SerializeField] private float spinSpeed = 90f;     // Slow rotation
    [SerializeField] private float bobAmplitude = 0.2f; // Vertical bob distance
    [SerializeField] private float bobFrequency = 2f;   // Bobs per second

    private Vector3 startPosition;

    void Start()
    {
        // Remember our spawn position so the bob doesn't drift over time
        startPosition = transform.position;

        // Auto-destroy if the player never picks it up
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Slow rotation makes pickups look "alive"
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        // Vertical bob — Sin gives smooth up/down motion
        float bobOffset = Mathf.Sin(Time.time * bobFrequency * Mathf.PI) * bobAmplitude;
        transform.position = startPosition + new Vector3(0f, bobOffset, 0f);
    }

    // Unity calls this when something enters our trigger collider
    void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to the Player
        if (other.GetComponentInParent<Player>() == null) return;

        // Play pickup SFX
        if (SoundManager.Instance != null) SoundManager.Instance.PlayPickup();

        // Subclasses define what happens when picked up
        OnPickedUp(other.GetComponentInParent<Player>());

        // Pickup vanishes after use
        Destroy(gameObject);
    }

    // abstract = subclasses MUST implement this
    protected abstract void OnPickedUp(Player player);
}
