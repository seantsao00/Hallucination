using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.Events;

public enum GameState {
    MainMenu,
    Paused,
    Play
}
public enum GamePlayState {
    None,
    Cinematic,
    Normal,
    DialogueActive
}

public class GameStateManager {
    static GameStateManager instance;
    public static GameStateManager Instance {
        get {
            instance ??= new GameStateManager();
            return instance;
        }
    }

    public UnityEvent<GameState> GameStateChangedEvent = new UnityEvent<GameState>();
    public UnityEvent<GamePlayState> GamePlayStateChangedEvent = new UnityEvent<GamePlayState>();

    private GameState currentGameState = GameState.Play;
    public GameState CurrentGameState {
        get => currentGameState;
        set {
            GameState oldState = currentGameState;
            currentGameState = value;
            if (oldState != currentGameState) GameStateChangedEvent?.Invoke(currentGameState);
        }
    }
    private GamePlayState currentGamePlayState = GamePlayState.Normal;
    public GamePlayState CurrentGamePalyState {
        get => currentGamePlayState;
        set {
            GamePlayState oldState = currentGamePlayState;
            currentGamePlayState = value;
            if (oldState != currentGamePlayState) GamePlayStateChangedEvent?.Invoke(currentGamePlayState);
        }
    }

    public void StartGame() {
        Assert.IsTrue(CurrentGameState != GameState.Paused && CurrentGameState != GameState.Play);
        CurrentGameState = GameState.Play;
        Time.timeScale = 1f;
    }

    public void ResumeGame() {
        Assert.IsTrue(CurrentGameState == GameState.Paused);
        CurrentGameState = GameState.Play;
        Time.timeScale = 1f;
    }

    void PauseGame() {
        CurrentGameState = GameState.Paused;
        Time.timeScale = 0f;
    }

    public void GoToMainMenu() {
        CurrentGameState = GameState.MainMenu;
        CurrentGamePalyState = GamePlayState.None;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame() {
        CurrentGameState = GameState.Play;
        Time.timeScale = 1f;
        LevelNavigator.Instance.RestartCurrentLevel();
    }
}
