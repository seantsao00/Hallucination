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
        Control.Character.Enable();
        Control.World.Enable();
        GameObject currentPlayedCharacter = Utils.FindCurrentPlayedCharacter();
        CharacterStateController characterStateController = currentPlayedCharacter.GetComponent<CharacterStateController>();
        characterStateController.UpdateInput();
    }

    public void DisableAllInput() {
        Control.Disable();
    }
}
