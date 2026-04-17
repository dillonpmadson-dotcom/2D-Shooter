using UnityEngine;

// Smoothly follows a target (the player) — gives the world a sense of endless space
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;          // Drag the Player here in the Inspector
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);  // Camera always sits at z=-10 in 2D

    void Start()
    {
        // Auto-find the player if no target is set in Inspector
        if (target == null)
        {
            Player player = FindAnyObjectByType<Player>();
            if (player != null) target = player.transform;
        }
    }

    // LateUpdate runs AFTER all Update() calls — perfect for cameras so we follow
    // the player's already-updated position (avoids jitter)
    void LateUpdate()
    {
        if (target == null) return;

        // Snap directly to the target position. Because the Player has Rigidbody2D
        // Interpolation enabled, target.position is already the smoothed visual position.
        // Doing extra Lerp here would create relative jitter.
        transform.position = target.position + offset;
    }
}
