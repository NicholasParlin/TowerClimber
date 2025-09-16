using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

// This script manages the Pause Menu UI and the game's paused state.
public class PauseMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;

    // A static variable to easily check if the game is paused from any script.
    public static bool isGamePaused = false;

    private void Start()
    {
        // Ensure the pause menu is closed and the game is running at the start.
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    /// <summary>
    /// Toggles the pause state of the game. This is called by the PlayerInputManager.
    /// </summary>
    public void TogglePauseMenu()
    {
        if (isGamePaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // Resumes the flow of time in the game.
        isGamePaused = false;
    }

    private void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Freezes the flow of time in the game.
        isGamePaused = true;
    }

    // You would call these methods from buttons in your pause menu UI.
    public void LoadMainMenu()
    {
        // Make sure to unpause before changing scenes.
        Time.timeScale = 1f;
        isGamePaused = false;
        // SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with your scene name
        Debug.Log("Loading Main Menu...");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}