using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour {
    public GameObject pauseMenuUI; // Assign the Pause Menu UI in the Inspector
    private bool isPaused = false;

    void Update() {
        // Listen for the "Esc" key to toggle the pause state
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isPaused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    // Function to resume the game
    public void Resume() {
        pauseMenuUI.SetActive(false);     // Hide the pause menu
        Time.timeScale = 1f;              // Resume time
        isPaused = false;
    }

    // Function to pause the game
    void Pause() {
        pauseMenuUI.SetActive(true);     // Show the pause menu
        Time.timeScale = 0f;             // Pause time
        isPaused = true;
    }

    // Function to go back to the main menu
    public void GoToMainMenu() {
        Time.timeScale = 1f;             // Ensure time is resumed
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with the actual scene name
    }

    // Function to restart the current level
    public void RestartGame() {
        Time.timeScale = 1f;             // Ensure time is resumed
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}
