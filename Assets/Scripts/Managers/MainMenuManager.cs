using UnityEngine;
using UnityEngine.SceneManagement;

// MainMenuManager: handles button clicks on the title screen
public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;          // Title + buttons
    [SerializeField] private GameObject instructionsPanel;  // Controls / how-to-play

    [Header("Scene to Load")]
    [SerializeField] private string gameSceneName = "SampleScene";

    void Start()
    {
        // Make sure we start on the main panel (not on instructions)
        if (mainPanel != null) mainPanel.SetActive(true);
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
    }

    // Hooked to the PLAY button's OnClick
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // Hooked to the INSTRUCTIONS button's OnClick
    public void ShowInstructions()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (instructionsPanel != null) instructionsPanel.SetActive(true);
    }

    // Hooked to the BACK button on the instructions panel
    public void HideInstructions()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
    }

    // Hooked to the QUIT button's OnClick (only works in built game, not Editor)
    public void QuitGame()
    {
        Debug.Log("Quit pressed — closing game.");
        Application.Quit();
    }
}
