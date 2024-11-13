using System;
using System.Collections.Generic;
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

    public void SetPauseMode() {
        Control.Disable();
        Control.Game.Enable();
        Control.UI.Enable();
    }

    public void SetDialogueMode() {
        Control.Disable();
        Control.Game.Enable();
        Control.UI.Enable();
        Control.Dialogue.Enable();
    }

    public void SetNormalMode() {
        Control.Disable();
        Control.Game.Enable();
        GameObject currentPlayedCharacter;
        if (WorldSwitchManager.Instance.currentWorld == World.Fairy) {
            currentPlayedCharacter = GameObject.FindGameObjectsWithTag("fairy")[0];
        } else {
            currentPlayedCharacter = GameObject.FindGameObjectsWithTag("bear")[0];
        }
        UpdateInputAccordingToActiveState(currentPlayedCharacter.GetComponent<CharacterStateController>().ActiveStates);
    }

    public void DisableAllInput() {
        Control.Disable();
    }

    public void UpdateInputAccordingToActiveState(HashSet<CharacterState> activeStates) {
        Func<bool> characterBusy = () =>
            activeStates.Contains(CharacterState.Grabbing) ||
            activeStates.Contains(CharacterState.Dashing) ||
            activeStates.Contains(CharacterState.Climbing) ||
            activeStates.Contains(CharacterState.BeingBlown) ||
            activeStates.Contains(CharacterState.NotStandingOnGround)
        ;

        // Character.HorizontalMove
        if (
            activeStates.Contains(CharacterState.Climbing)
        ) {
            Control.Character.HorizontalMove.Disable();
        } else {
            Control.Character.HorizontalMove.Enable();
        }
        // Character.VerticalMove
        if (
            activeStates.Contains(CharacterState.Grabbing) ||
            activeStates.Contains(CharacterState.Dashing) ||
            activeStates.Contains(CharacterState.BeingBlown) ||
            activeStates.Contains(CharacterState.NotStandingOnGround)
        ) {
            Control.Character.VerticalMove.Disable();
        } else {
            Control.Character.VerticalMove.Enable();
        }
        // Character.Jump
        // Character.Dash
        if (characterBusy()) {
            Control.Character.Dash.Disable();
        } else {
            Control.Character.Dash.Enable();
        }
        // Character.Interact
        if (characterBusy()) {
            Control.Character.Interact.Disable();
        } else {
            Control.Character.Interact.Enable();
        }
        // Character.Grab
        if (
            activeStates.Contains(CharacterState.Dashing) ||
            activeStates.Contains(CharacterState.Climbing) ||
            activeStates.Contains(CharacterState.BeingBlown) ||
            activeStates.Contains(CharacterState.NotStandingOnGround)
        ) {
            Control.Character.Grab.Disable();
        } else {
            Control.Character.Grab.Enable();
        }

        // World
        if (characterBusy()) {
            Control.World.Disable();
        } else {
            Control.World.Enable();
        }
    }
}
