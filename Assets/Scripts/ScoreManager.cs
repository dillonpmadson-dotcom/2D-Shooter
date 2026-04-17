using UnityEngine;
using TMPro;

// Tracks the player's score and updates the UI text
// Static fields persist across scene reloads UNLESS reset — we reset in Awake
public class ScoreManager : MonoBehaviour
{
    // Static so any script can read/add to it without finding the instance
    public static int Score;

    // Drag the score Text (TMP) here in the Inspector
    [SerializeField] private TextMeshProUGUI scoreText;

    void Awake()
    {
        // Reset score whenever a new game starts
        Score = 0;
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Score;
        }
    }

    // Public API — call from anywhere to add points
    public static void AddScore(int amount)
    {
        Score += amount;
    }
}
