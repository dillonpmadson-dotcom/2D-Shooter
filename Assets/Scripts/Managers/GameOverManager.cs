using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Listens for player death, shows the Game Over panel, manages high score persistence
public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;     // The whole panel that pops up
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    // PlayerPrefs key for saving the best score across sessions
    private const string BestScoreKey = "BestScore";

    void Start()
    {
        // Hide the panel at start (only shown after death)
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void OnEnable()
    {
        Player.OnPlayerDied += ShowGameOver;
    }

    void OnDisable()
    {
        Player.OnPlayerDied -= ShowGameOver;
    }

    // Called automatically when the player dies
    private void ShowGameOver()
    {
        // Save best score if this run beat the previous high
        int finalScore = ScoreManager.Score;
        int bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        if (finalScore > bestScore)
        {
            bestScore = finalScore;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.Save();
        }

        // Update panel text
        if (finalScoreText != null) finalScoreText.text = "Score: " + finalScore;
        if (bestScoreText != null) bestScoreText.text = "Best: " + bestScore;

        // Show the panel
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // Pause the world (enemies, spawner, bullets all freeze)
        Time.timeScale = 0f;
    }

    // Hooked up to the "Play Again" button via the Inspector OnClick
    public void RestartGame()
    {
        // Important: unfreeze before reloading the scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
