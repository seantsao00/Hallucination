using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGameMenu : MonoBehaviour {
    public Button initialButton;

    void Awake() {
        GameStateManager.Instance.GameStateChangedEvent.AddListener(HandleGameStateChange);
        InputManager.Control.Game.Pause.performed += PauseHandler;
        gameObject.SetActive(false);
    }

    public void Restart() {
        GameStateManager.Instance.CurrentGameState = GameState.Play;
        LevelNavigator.Instance.RestartCurrentLevel();
    }

    public void BackToMainMenu() {
        GameStateManager.Instance.CurrentGameState = GameState.MainMenu;
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseHandler(InputAction.CallbackContext context) {
        switch (GameStateManager.Instance.CurrentGameState) {
            case GameState.Paused:
                GameStateManager.Instance.CurrentGameState = GameState.Play;
                break;
            case GameState.Play:
                GameStateManager.Instance.CurrentGameState = GameState.Paused;
                SetFocusOnButton();
                break;
            default:
                Debug.LogError($"Pause is not supported in the current GameState: {GameStateManager.Instance.CurrentGameState}");
                break;
        }
    }
    public void SetFocusOnButton() {
        if (initialButton != null) {
            EventSystem.current.SetSelectedGameObject(initialButton.gameObject);
        } else {
            Debug.LogWarning("Target button is not assigned.");
        }
        Debug.Log("Focus set");
    }

    void OnDestroy() {
        GameStateManager.Instance.GameStateChangedEvent.RemoveListener(HandleGameStateChange);
        InputManager.Control.Game.Pause.performed -= PauseHandler;
    }

    public void HandleGameStateChange(GameState state) {
        if (state == GameState.Paused)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
