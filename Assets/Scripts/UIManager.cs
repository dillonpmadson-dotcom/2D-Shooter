using UnityEngine;
using TMPro;

// UIManager: keeps the on-screen UI in sync with the player's state
public class UIManager : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Player player;

    [Header("Static UI (Top-Left)")]
    [SerializeField] private TextMeshProUGUI nukeCountText;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Floating UI (Tracks Player)")]
    [SerializeField] private TextMeshProUGUI gunTimerText;
    [SerializeField] private Vector2 gunTimerScreenOffset = new Vector2(0, 60); // Pixels above player
    [SerializeField] private Camera mainCamera;

    [Header("Dash Cooldown Bar")]
    [SerializeField] private UnityEngine.UI.Image dashCooldownBar;

    [Header("Wave Indicator")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private GameManager gameManager;

    void Start()
    {
        // Auto-find references if not assigned in Inspector
        if (player == null) player = FindAnyObjectByType<Player>();
        if (mainCamera == null) mainCamera = Camera.main;
        if (gameManager == null) gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (player == null) return;

        // --- Nuke counter ---
        if (nukeCountText != null)
        {
            nukeCountText.text = "Nukes: " + player.NukeCount;
        }

        // --- Wave indicator (survival time + difficulty %) ---
        if (waveText != null && gameManager != null)
        {
            int seconds = Mathf.FloorToInt(gameManager.ElapsedTime);
            int wave = Mathf.FloorToInt(gameManager.DifficultyPercent * 10f) + 1; // 1-10 wave scale
            waveText.text = "Wave " + wave + "  |  " + seconds + "s";
        }

        // --- Dash cooldown bar (uses Image's "Filled" type) ---
        if (dashCooldownBar != null)
        {
            dashCooldownBar.fillAmount = player.DashCooldownPercent;
        }

        // --- Health display ---
        if (healthText != null && player.healthModule != null)
        {
            // Mathf.Max keeps it from showing negative numbers
            int hp = Mathf.Max(0, Mathf.RoundToInt(player.healthModule.healthpoints));
            healthText.text = "HP: " + hp;

            // Color shifts red as HP gets low — visual urgency
            if (hp <= 25) healthText.color = Color.red;
            else if (hp <= 50) healthText.color = new Color(1f, 0.6f, 0f); // orange
            else healthText.color = Color.green;
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
