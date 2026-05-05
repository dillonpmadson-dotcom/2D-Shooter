using UnityEngine;
using UnityEngine.SceneManagement;

// Hit Esc to toggle pause. Sets Time.timeScale = 0 to freeze the world.
public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pausePanel != null) pausePanel.SetActive(isPaused);
    }

    // Hooked to Resume button OnClick
    public void Resume()
    {
        if (isPaused) TogglePause();
    }

    // Hooked to Main Menu button OnClick
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // unfreeze before loading
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Hooked to Quit button OnClick (only works in built game)
    public void QuitGame()
    {
        Application.Quit();
    }
}
