using UnityEngine;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour {
    [SerializeField] GameObject pauseMenuUI;

    void Awake() {
        GameStateManager.Instance.GameStateChangedEvent.AddListener(HandleGameStateChange);
        InputManager.Control.Game.Pause.performed += PauseHandler;
    }

    public void PauseHandler(InputAction.CallbackContext context) {
        switch(GameStateManager.Instance.CurrentGameState) {
            case GameState.Paused:
                GameStateManager.Instance.CurrentGameState = GameState.Play;
                break;
            case GameState.Play:
                GameStateManager.Instance.CurrentGameState = GameState.Paused;
                break;
            default:
                Debug.LogError($"Pause is not supported in the current GameState: {GameStateManager.Instance.CurrentGameState}");
                break;
        }
    }

    void OnDestroy() {
        GameStateManager.Instance.GameStateChangedEvent.AddListener(HandleGameStateChange);
    }

    public void HandleGameStateChange(GameState state) {
        if (state == GameState.Paused)
            pauseMenuUI.SetActive(true);
        else
            pauseMenuUI.SetActive(false);
    }
}
