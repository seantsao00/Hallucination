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
        InputManager.Instance.UpdateInputAccordingToActiveState(activeStates);
    }
}

