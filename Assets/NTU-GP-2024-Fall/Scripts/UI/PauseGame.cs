using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour {
    public GameObject pauseMenuUI; // Assign the Pause Menu UI in the Inspector
    public GameObject fairy;      // Assign the Player GameObject in the Inspector
    public GameObject bear;
    public GameObject worldSwitchManager;
    bool isPaused = false;
    Character fairyCharacter, bearCharacter;
    CharacterHorizontalMove fairyHorizontalMove, bearHorizontalMove;
    CharacterDash fairyDash;
    CharacterJump fairyJump;
    CharacterClimb bearClimb;
    WorldSwitchManager worldSwitch;
    void Start() {
        fairyCharacter = fairy.GetComponent<Character>();
        bearCharacter = bear.GetComponent<Character>();
        fairyHorizontalMove = fairy.GetComponent<CharacterHorizontalMove>();
        bearHorizontalMove = bear.GetComponent<CharacterHorizontalMove>();
        fairyDash = fairy.GetComponent<CharacterDash>();
        fairyJump = fairy.GetComponent<CharacterJump>();
        bearClimb = bear.GetComponent<CharacterClimb>();
        worldSwitch = worldSwitchManager.GetComponent<WorldSwitchManager>();
    }


    void OnEnable() {
        InputManager.Instance.Game.Actions.Pause.performed += Pause;
    }
    void OnDisable() {
        InputManager.Instance.Game.Actions.Pause.performed -= Pause;
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
        SetMovementActive(true);
    }

    // Function to pause the game
    void Pause() {
        pauseMenuUI.SetActive(true);     // Show the pause menu
        Time.timeScale = 0f;             // Pause time
        isPaused = true;
        SetMovementActive(false);
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

    void SetMovementActive(bool isEnabled) {
        fairyCharacter.enabled = isEnabled;
        bearCharacter.enabled = isEnabled;
        fairyHorizontalMove.enabled = isEnabled;
        bearHorizontalMove.enabled = isEnabled;
        fairyDash.enabled = isEnabled;
        fairyJump.enabled = isEnabled;
        bearClimb.enabled = isEnabled;
        worldSwitch.enabled = isEnabled;
    }
}
