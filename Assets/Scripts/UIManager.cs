using UnityEngine;
using TMPro;

// UIManager: keeps the on-screen UI in sync with the player's state
public class UIManager : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Player player;

    [Header("Static UI (Top-Left)")]
    [SerializeField] private TextMeshProUGUI nukeCountText;

    [Header("Floating UI (Tracks Player)")]
    [SerializeField] private TextMeshProUGUI gunTimerText;
    [SerializeField] private Vector2 gunTimerScreenOffset = new Vector2(0, 60); // Pixels above player
    [SerializeField] private Camera mainCamera;

    void Start()
    {
        // Auto-find references if not assigned in Inspector
        if (player == null) player = FindAnyObjectByType<Player>();
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        if (player == null) return;

        // --- Nuke counter ---
        if (nukeCountText != null)
        {
            nukeCountText.text = "Nukes: " + player.NukeCount;
        }

        // --- Gun power-up timer (only visible when active) ---
        if (gunTimerText != null)
        {
            if (player.HasGunPowerUp)
            {
                gunTimerText.gameObject.SetActive(true);

                // Show the time remaining (rounded to 1 decimal)
                gunTimerText.text = "Rapid: " + player.GunPowerUpTimeLeft.ToString("F1") + "s";

                // Position the text on screen, just above the player
                Vector3 screenPos = mainCamera.WorldToScreenPoint(player.transform.position);
                gunTimerText.transform.position = screenPos + (Vector3)gunTimerScreenOffset;
            }
            else
            {
                // Hide the timer when not powered up
                gunTimerText.gameObject.SetActive(false);
            }
        }
    }
}
