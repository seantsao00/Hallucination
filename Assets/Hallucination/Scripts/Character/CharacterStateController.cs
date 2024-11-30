using System.Collections.Generic;
using UnityEngine;

public enum CharacterState {
    Idle,
    Walking,
    PreReleaseJumping,
    Climbing,
    Dashing,
    HorizontalSpringFlying,
    Grabbing,
    BeingBlown,
    LedgeClimbing,
    NotStandingOnGround,
    AirHanging
}

public class CharacterStateController : MonoBehaviour {
    [Header("Gravity")]
    public float BeingBlownGravityMultiplier = 0.4f;
    public float AirHangingGravityMultiplier = 0.4f;

    HashSet<CharacterState> activeStates = new HashSet<CharacterState>();

    public delegate void StateChangedHandler(CharacterState state, bool added);
    public event StateChangedHandler OnStateChanged;

    Rigidbody2D rb;
    public float NormalGravityScale { get; private set; }
    Animator animator;

    void Awake() {
        OnStateChanged += HandleStateChange;
        rb = GetComponent<Rigidbody2D>();
        NormalGravityScale = rb.gravityScale;
        animator = GetComponent<Animator>();
    }

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
        UpdateGravity();
        UpdateAnimator();
        // Debug.Log($"{CharacterBusy}, states: {string.Join(", ", ActiveStates.ToArray())}");
        if (CharacterBusy) {
            WorldSwitchManager.Instance.Lock(gameObject);
        } else {
            WorldSwitchManager.Instance.Unlock(gameObject);
        }
    }

    public void UpdateGravity() {
        if (HasState(CharacterState.Climbing)) {
            rb.gravityScale = 0;
        } else if (HasState(CharacterState.Dashing)) {
            rb.gravityScale = 0;
        } else if (HasState(CharacterState.BeingBlown)) {
            rb.gravityScale = NormalGravityScale * BeingBlownGravityMultiplier;
        } else if (HasState(CharacterState.AirHanging)) {
            rb.gravityScale = NormalGravityScale * AirHangingGravityMultiplier;
        } else if (HasState(CharacterState.PreReleaseJumping)) {
            rb.gravityScale = NormalGravityScale * GetComponent<CharacterJump>().PreReleaseGravityMultiplier;
        } else {
            rb.gravityScale = NormalGravityScale;
        }
    }

    public void UpdateAnimator() {
        if (HasState(CharacterState.Walking)) {
            animator.SetBool("Movement", true);
        } else {
            animator.SetBool("Movement", false);
        }
        if (HasState(CharacterState.Climbing)) {
            animator.SetBool("Climb", true);
        } else {
            animator.SetBool("Climb", false);
        }
        if (HasState(CharacterState.LedgeClimbing)) {
            animator.SetBool("LedgeClimb", true);
        } else {
            animator.SetBool("LedgeClimb", false);
        }
    }

    public bool CharacterBusy =>
        activeStates.Contains(CharacterState.Grabbing) ||
        activeStates.Contains(CharacterState.Dashing) ||
        activeStates.Contains(CharacterState.HorizontalSpringFlying) ||
        activeStates.Contains(CharacterState.Climbing) ||
        activeStates.Contains(CharacterState.BeingBlown) ||
        activeStates.Contains(CharacterState.NotStandingOnGround) ||
        activeStates.Contains(CharacterState.LedgeClimbing)
    ;

    public void UpdateInput() {
        if (GameStateManager.Instance.CurrentGamePlayState != GamePlayState.Normal) return;
        PlayerControl Control = InputManager.Control;

        if (activeStates.Contains(CharacterState.LedgeClimbing)) {
            Control.Character.Disable();
        } else {
            // Character.HorizontalMove
            if (
                activeStates.Contains(CharacterState.Climbing) ||
                activeStates.Contains(CharacterState.Dashing)
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

    public void LogCurrentStates() {
        Debug.Log(
            $"Current {typeof(CharacterState)}:" +
            string.Join(", ", ActiveStates)
        );
    }
}
