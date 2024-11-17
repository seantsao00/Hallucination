using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour {
    public GameObject pauseMenuUI; // Assign the Pause Menu UI in the Inspector
    bool isPaused = false;
    void Start() {
    }


    void OnEnable() {
        InputManager.Control.Game.Pause.performed += Pause;
    }
    void OnDisable() {
        InputManager.Control.Game.Pause.performed -= Pause;
    }

    void Pause(InputAction.CallbackContext context) {
        if (isPaused) {
            Resume();
        } else {
            Pause();
        }
    }

    // Function to resume the game
    public void Resume() {
        pauseMenuUI.SetActive(false);     // Hide the pause menu
        Time.timeScale = 1f;              // Resume time
        isPaused = false;
        InputManager.Instance.SetNormalMode();
    }

    // Function to pause the game
    void Pause() {
        pauseMenuUI.SetActive(true);     // Show the pause menu
        Time.timeScale = 0f;             // Pause time
        isPaused = true;
        InputManager.Instance.SetPauseMode();
    }

    // Function to go back to the main menu
    public void GoToMainMenu() {
        Time.timeScale = 1f;             // Ensure time is resumed
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with the actual scene name
    }

    // Function to restart the current level
    public void RestartGame() {
        Time.timeScale = 1f;             // Ensure time is resumed
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name); // Reload the current scene
        InputManager.Instance.SetNormalMode();
    }
}
