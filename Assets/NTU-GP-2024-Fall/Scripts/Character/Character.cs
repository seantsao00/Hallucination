using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;


public class Character : MonoBehaviour {
    CharacterState.ICharacterState currentState;
    public CharacterState.ICharacterState CurrentState {
        get { return currentState; }
        set { SetCurrentState(value); }
    }

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;

    Vector2 facingDirection = new Vector2(1, 0);
    public Vector2 FacingDirection {
        get { return facingDirection; }
        set { SetFacingDirection(value); }
    }
    [SerializeField] Transform faceCheck;
    [SerializeField] float faceCheckRadius = 0.2f;
    GameObject facedMovableGameObject;
    /// <summary>
    /// Faced movable object within character's interacting range
    /// </summary>
    public GameObject FacedMovableGameObject {
        get { return facedMovableGameObject; }
        set { SetFacedMovableObject(value); }
    }

    LayerMask groundLayerMask, climbableLayerMask, movableMask;

    private Rigidbody2D rb;
    public Rigidbody2D Rb { get { return rb; } }
    [HideInInspector] public bool IsGrounded = true;

    [HideInInspector] public Collider2D OverlappedClimbalbe;

    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public float NormalGravityScale;
    public float NormalMoveSpeed = 5f;
    public float GrabbingStoneSpeed = 2f;
    public float JumpPower = 10f;
    [HideInInspector] public float CurrentSpeed;


    void SetCurrentState(CharacterState.ICharacterState newState) {
        if (newState == currentState) return;
        currentState?.HandleStateChange(this, false);
        newState?.HandleStateChange(this, true);
        currentState = newState;
    }

    private void SetFacingDirection(Vector2 direction) {
        if (direction.x * facingDirection.x < 0) {
            if (direction.x * facingDirection.x < 0) {
                // Flip the faceCheck position along the X-axis relative to the character
                Vector3 currentLocalPosition = faceCheck.localPosition;
                currentLocalPosition.x = -currentLocalPosition.x;
                faceCheck.localPosition = currentLocalPosition;
            }
        }
        facingDirection = direction;
    }

    private void SetFacedMovableObject(GameObject gameObject) {
        if (gameObject != facedMovableGameObject) {
            Stone stone = facedMovableGameObject?.GetComponent<Stone>();
            if (stone != null) {
                stone.HorizontalMove(0);
                CurrentState = new CharacterState.Free();
            }
        }
        facedMovableGameObject = gameObject;
    }

    void Awake() {
        groundLayerMask = LayerMask.GetMask("Ground");
        climbableLayerMask = LayerMask.GetMask("Climbable");
        movableMask = LayerMask.GetMask("Movable");
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        NormalGravityScale = Rb.gravityScale;
        CurrentState = new CharacterState.Free();
        CurrentSpeed = NormalMoveSpeed;
    }

    void Update() {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask | movableMask);
        FacedMovableGameObject = Physics2D.OverlapCircle(faceCheck.position, faceCheckRadius, movableMask)?.gameObject;
        OverlappedClimbalbe = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, climbableLayerMask);
        if (CurrentState is not CharacterState.GrabbingMovable) {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal != 0) FacingDirection = new(horizontal, 0);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(faceCheck.position, faceCheckRadius);
    }
}