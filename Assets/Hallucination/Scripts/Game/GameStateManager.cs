using UnityEngine;
using UnityEngine.Events;

public enum GameState {
    MainMenu,
    Animation,
    Paused,
    Play,
    End
}
public enum GamePlayState {
    None,
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
    static public void Init() {
        instance ??= new GameStateManager();
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
                    CurrentGamePlayState = GamePlayState.None;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.Animation:
                    CurrentGamePlayState = GamePlayState.None;
                    break;
                case GameState.Play:
                    Time.timeScale = 1f;
                    if (CurrentGamePlayState == GamePlayState.None)
                        CurrentGamePlayState = GamePlayState.Normal;
                    break;
                default:
                    Debug.LogError($"Unhandled {nameof(CurrentGameState)}: {CurrentGameState}");
                    break;
            }

            if (oldState != currentGameState) GameStateChangedEvent?.Invoke(currentGameState);
        }
    }

    private GamePlayState currentGamePlayState = GamePlayState.Normal;
    public GamePlayState CurrentGamePlayState {
        get => currentGamePlayState;
        set {
            GamePlayState oldState = currentGamePlayState;
            currentGamePlayState = value;
            if (
                oldState == GamePlayState.SwitchingWorld &&
                currentGamePlayState != GamePlayState.Normal &&
                currentGamePlayState != GamePlayState.None
            ) {
                Debug.LogError(
                    $"You switched the state to {currentGamePlayState}" +
                    $"You must switch the state back to {nameof(GamePlayState.Normal)} or {nameof(GamePlayState.None)}" +
                    $"from {nameof(GamePlayState.SwitchingWorld)}"
                );
            }
            // Debug.Log($"Change {nameof(GamePlayState)} from {oldState} to {currentGamePlayState}");
            if (oldState != currentGamePlayState) GamePlayStateChangedEvent?.Invoke(currentGamePlayState);
        }
    }
}
