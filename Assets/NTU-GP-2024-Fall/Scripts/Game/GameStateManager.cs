using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public enum GameState {
    Menu,
    Paused,
    Playing
}
public enum InGameState {
    None,
    Normal,
    HavingConversation
}

public class GameStateManager {
    static GameStateManager instance;
    public static GameStateManager Instance {
        get {
            if (instance == null) {
                instance = new GameStateManager();
            }
            return instance;
        }
    }

    public GameState CurrentGameState = GameState.Playing;
    public InGameState CurrentInGameState = InGameState.Normal;

    public void Resume() {
        Assert.IsTrue(CurrentGameState == GameState.Paused);
        Time.timeScale = 1f;
        InputManager.Instance.SetNormalMode();
    }

    // Function to pause the game
    void Pause() {
        Time.timeScale = 0f;
        CurrentGameState = GameState.Paused;
        InputManager.Instance.SetPauseMode();
    }

    // Function to go back to the main menu
    public void GoToMainMenu() {
        Time.timeScale = 1f;
        CurrentGameState = GameState.Menu;
        CurrentInGameState = InGameState.None;
        SceneManager.LoadScene("MainMenu");
    }

    // Function to restart the current level
    public void RestartGame() {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        InputManager.Instance.SetNormalMode();
    }
}
