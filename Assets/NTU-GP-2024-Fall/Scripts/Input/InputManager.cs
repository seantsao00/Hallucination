using UnityEngine;

public class InputManager {
    static InputManager instance;
    public static InputManager Instance {
        get {
            if (instance == null) {
                instance = new InputManager();
                instance.control.Character.Enable();
                instance.control.World.Enable();
                instance.control.Game.Enable();
                GameStateManager.Instance.GameStateChangedEvent.AddListener(instance.SetInputAccordingToGameState);
                GameStateManager.Instance.GamePlayStateChangedEvent.AddListener(instance.SetInputAccordingToGamePlayState);
            }
            return instance;
        }
    }
    private InputManager() {
        control = new PlayerControl();
    }

    private PlayerControl control;
    static public PlayerControl Control {
        get { return Instance.control; }
    }

    public float CharacterHorizontalMove => Control.Character.HorizontalMove.ReadValue<float>();
    public float CharacterVerticalMove => Control.Character.VerticalMove.ReadValue<float>();

    void SetInputAccordingToGameState(GameState state) {
        Control.Disable();
        switch (state) {
            case GameState.MainMenu:
                Control.UI.Enable();
                break;
            case GameState.Paused:
                Control.UI.Enable();
                Control.Game.Enable();
                break;
            case GameState.Play:
                SetInputAccordingToGamePlayState(GameStateManager.Instance.CurrentGamePalyState);
                break;
            default:
                break;
        }
        WorldSwitchManager.Instance.UpdateWorldSwitchIcon();
    }

    void SetInputAccordingToGamePlayState(GamePlayState state) {
        Control.Disable();
        Control.Game.Enable();
        switch (state) {
            case GamePlayState.Normal:
                Control.Character.Enable();
                Control.World.Enable();
                GameObject currentPlayedCharacter = Utils.FindCurrentPlayedCharacter();
                CharacterStateController characterStateController = currentPlayedCharacter.GetComponent<CharacterStateController>();
                characterStateController.UpdateInput();
                break;
            case GamePlayState.DialogueActive:
                Control.UI.Enable();
                Control.Dialogue.Enable();
                break;
            case GamePlayState.FullScreenTip:
                Control.Tip.Enable();
                Control.Game.Disable();
                break;
            case GamePlayState.SwitchingWorld:
                Control.Disable();
                break;
            case GamePlayState.AllInputDisabled:
                Control.Disable();
                break;
            default:
                break;
        }
        WorldSwitchManager.Instance.UpdateWorldSwitchIcon();
    }
}
