using UnityEngine;
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
    DialogueActive,
    SwitchingWorld,
    FullScreenTip,
    AllInputDisabled
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
            switch (currentGameState) {
                case GameState.MainMenu:
                    CurrentGamePalyState = GamePlayState.None;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.Play:
                    Time.timeScale = 1f;
                    currentGamePlayState = GamePlayState.Normal;
                    break;
                default:
                    Debug.LogError($"Unhandled {nameof(CurrentGameState)}: {CurrentGameState}");
                    break;
            }

            if (oldState != currentGameState) GameStateChangedEvent?.Invoke(currentGameState);
        }
    }
    private GamePlayState currentGamePlayState = GamePlayState.Normal;
    public GamePlayState CurrentGamePalyState {
        get => currentGamePlayState;
        set {
            GamePlayState oldState = currentGamePlayState;
            if (
                oldState == GamePlayState.SwitchingWorld && 
                value != GamePlayState.Normal &&
                value != GamePlayState.None
            ) {
                Debug.LogError(
                    $"You tried to switch the state to {value}" +
                    $"You must switch the state back to {nameof(GamePlayState.Normal)} or {nameof(GamePlayState.None)}" +
                    $"from {nameof(GamePlayState.SwitchingWorld)}"
                );
            }
            currentGamePlayState = value;
            if (oldState != currentGamePlayState) GamePlayStateChangedEvent?.Invoke(currentGamePlayState);
        }
    }
}
