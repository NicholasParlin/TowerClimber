using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

// MODIFIED: Inherits from our new UIPanel base class
public class PauseMenuUI : UIPanel
{
    // A static variable to easily check if the game is paused from any script.
    public static bool isGamePaused = false;

    /// <summary>
    /// Resumes the game. Called by the UIManager or the resume button.
    /// </summary>
    public void Resume()
    {
        // This method can be called directly by a "Resume" button in the pause menu UI.
        // It tells the UIManager to close the currently open panel, which is the pause menu.
        if (UIManager.Instance != null)
        {
            // MODIFIED: This now correctly calls CloseTopPanel, which is the intended action.
            UIManager.Instance.CloseTopPanel();
        }
    }

    // We now override the Open() method from the base class.
    public override void Open()
    {
        base.Open(); // This activates the panel GameObject.
        Time.timeScale = 0f; // Freezes the flow of time in the game.
        isGamePaused = true;
    }

    // We now override the Close() method from the base class.
    public override void Close()
    {
        base.Close(); // This deactivates the panel GameObject.
        Time.timeScale = 1f; // Resumes the flow of time in the game.
        isGamePaused = false;
    }

    // These methods can be called from buttons in your pause menu UI.
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