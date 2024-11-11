using System;
using UnityEngine;

/// <summary>
/// The <c>Character</c> class maintains the character's current state, including movement states 
/// (e.g., jumping, dashing) and speed control (e.g., maximum fall speed).
/// This class provides methods and properties to change character states and movement, ensuring 
/// that updates notify all necessary dependencies.
/// </summary>
public class Character : MonoBehaviour {
    [System.Serializable]
    public class CharacterMovementAttributes {
        [Header("HorizontalMovement")]
        public float NormalHorizontalSpeed = 5f;
        public float JumpPower = 12f;
        public float ClimbingSpeed = 5f;
        public float GrabbingHorizontalSpeed = 3f;

        [Header("Behavior")]
        [Tooltip("Gravity multiplier applied when the character’s vertical falling speed falls below the threshold.")]
        public float AirHangTimeGravityMultiplier = 0.4f;
        public float AirHangTimeThresholdSpeed = 0.5f;
        public float StickOnWallFallingSpeed = 3f;
        public float MaxFallingSpeed = 16f;

        [HideInInspector, NonSerialized] public float velocityEps = 1e-4f;
    }

    public class CharacterCurrentMovement {
        private CharacterMovementAttributes attributes;

        /// <summary>
        /// The current horizontal movement speed applied to the character.
        /// Adjusts based on character state (e.g., reduced speed when pulling an object).
        /// </summary>
        public float HorizontalSpeed;

        public float ClimbingSpeed;

        public bool IsHorizontalMoveEnabled;
        public bool IsJumpEnabled;
        public bool IsDashEnabled;

        public CharacterCurrentMovement(CharacterMovementAttributes attributes) {
            Init(attributes);
        }
        public void Init(CharacterMovementAttributes attributes) {
            this.attributes = attributes;
            SetNormal();
        }
        public void SetNormal() {
            HorizontalSpeed = attributes.NormalHorizontalSpeed;
            ClimbingSpeed = attributes.ClimbingSpeed;
            IsHorizontalMoveEnabled = true;
            IsJumpEnabled = true;
            IsDashEnabled = true;
        }
        public void SetDashing() {
            SetNormal();
            IsHorizontalMoveEnabled = false;
        }
        public void SetGrabbing() {
            HorizontalSpeed = attributes.GrabbingHorizontalSpeed;
            IsHorizontalMoveEnabled = true;
            IsJumpEnabled = false;
            IsDashEnabled = false;
        }
        public void SetTransporting() {
            IsHorizontalMoveEnabled = false;
            IsJumpEnabled = false;
            IsDashEnabled = false;
        }
    }

    public CharacterMovementAttributes MovementAttributes;
    public CharacterCurrentMovement CurrentMovement;

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

    [HideInInspector] public Collider2D OverlappedClimbable;

    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public float NormalGravityScale;
    SpriteRenderer spriteRenderer;

    [SerializeField] TipManager tipManager;
    string grabTip = "Hold E or C to move the stone";


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
        spriteRenderer.flipX = direction.x > 0;
        facingDirection = direction;
    }

    private void SetFacedMovableObject(GameObject gameObject) {
        GameObject oldObject = facedMovableGameObject;
        facedMovableGameObject = gameObject;
        Stone stone;
        if (gameObject != oldObject) {
            stone = oldObject?.GetComponent<Stone>();
            if (stone != null) {
                tipManager.ShowTip(false);
                stone.Unleash();
                CurrentState = new CharacterState.Free();
            }
            stone = gameObject?.GetComponent<Stone>();
            if (stone != null) {
                tipManager.ShowTip(true, grabTip);
            }
        }
    }

    void Awake() {
        groundLayerMask = LayerMask.GetMask("Ground");
        climbableLayerMask = LayerMask.GetMask("Climbable");
        movableMask = LayerMask.GetMask("Movable");
        CurrentMovement = new CharacterCurrentMovement(MovementAttributes);
        rb = GetComponent<Rigidbody2D>();
        NormalGravityScale = Rb.gravityScale;
    }

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CurrentState = new CharacterState.Free();
    }

    void Update() {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask | movableMask);
        FacedMovableGameObject = Physics2D.OverlapCircle(faceCheck.position, faceCheckRadius, movableMask)?.gameObject;
        OverlappedClimbable = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, climbableLayerMask);
        if (CurrentState is not CharacterState.GrabbingMovable) {
            float direction = InputManager.Instance.CharacterHorizontalMove;
            if (direction != 0) FacingDirection = new(direction, 0);
        }
        if (Rb.velocity.x != 0) {
            GetComponent<Animator>().SetBool("Movement", true);
        } else {
            GetComponent<Animator>().SetBool("Movement", false);
        }
        if (Rb.velocity.y < -MovementAttributes.velocityEps && currentState is not CharacterState.Dashing) {
            if (Mathf.Abs(Rb.velocity.y) < MovementAttributes.AirHangTimeThresholdSpeed) {
                Rb.gravityScale = NormalGravityScale * MovementAttributes.AirHangTimeGravityMultiplier;
            } else {
                Rb.gravityScale = NormalGravityScale;
            }
        }
        if (Rb.velocity.y <= -MovementAttributes.MaxFallingSpeed) {
            Rb.velocity = new Vector2(Rb.velocity.x, -MovementAttributes.MaxFallingSpeed);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(faceCheck.position, faceCheckRadius);
    }
}
