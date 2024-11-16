using System.Collections.Generic;
using UnityEngine;

public enum CharacterState {
    Idle,
    Walking,
    Climbing,
    Dashing,
    Grabbing,
    BeingBlown,
    LedgeClimbing,
    NotStandingOnGround
}

public class CharacterStateController : MonoBehaviour {

    HashSet<CharacterState> activeStates = new HashSet<CharacterState>();

    public delegate void StateChangedHandler(CharacterState state, bool added);
    public event StateChangedHandler OnStateChanged;

    void Awake() { OnStateChanged += HandleStateChange; }

    void OnDestroy() { OnStateChanged -= HandleStateChange; }

    public void AddState(CharacterState state) {
        if (activeStates.Add(state)) {
            OnStateChanged?.Invoke(state, true);
        }
    }

    public void RemoveState(CharacterState state) {
        if (activeStates.Remove(state)) {
            OnStateChanged?.Invoke(state, false);
        }
    }

    public bool HasState(CharacterState state) {
        return activeStates.Contains(state);
    }

    public HashSet<CharacterState> ActiveStates => new HashSet<CharacterState>(activeStates);

    private void HandleStateChange(CharacterState state, bool added) {
        UpdateInput();
        if (CharacterBusy) WorldSwitchManager.Instance.Locks.Add(gameObject);
        else WorldSwitchManager.Instance.Locks.Remove(gameObject);
    }

    public bool CharacterBusy =>
        activeStates.Contains(CharacterState.Grabbing) ||
        activeStates.Contains(CharacterState.Dashing) ||
        activeStates.Contains(CharacterState.Climbing) ||
        activeStates.Contains(CharacterState.BeingBlown) ||
        activeStates.Contains(CharacterState.NotStandingOnGround) ||
        activeStates.Contains(CharacterState.LedgeClimbing)
    ;

    public void UpdateInput() {
        PlayerControl Control = InputManager.Control;

        if (activeStates.Contains(CharacterState.LedgeClimbing)) {
            Control.Character.Disable();
        } else {
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
                activeStates.Contains(CharacterState.BeingBlown)
            ) {
                Control.Character.VerticalMove.Disable();
            } else {
                Control.Character.VerticalMove.Enable();
            }
            // Character.Jump
            Control.Character.Jump.Enable();
            // Character.Dash
            if (
                activeStates.Contains(CharacterState.Grabbing) ||
                activeStates.Contains(CharacterState.Dashing) ||
                activeStates.Contains(CharacterState.Climbing) ||
                activeStates.Contains(CharacterState.LedgeClimbing)
            ) {
                Control.Character.Dash.Disable();
            } else {
                Control.Character.Dash.Enable();
            }
            // Character.Interact
            if (CharacterBusy) {
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
        }
    }
}

