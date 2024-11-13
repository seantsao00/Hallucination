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
        /// 


        public float HorizontalSpeed;

        public float ClimbingSpeed;
        public float SpringSpeed;

        public bool IsHorizontalMoveEnabled;
        public bool IsJumpEnabled;
        public bool IsDashEnabled;
        public float lastSpringTime;

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
        public void LaunchSpring(float speed) {
            SpringSpeed = speed;
            lastSpringTime = Time.time;
        }
    }

    public CharacterMovementAttributes MovementAttributes;
    public CharacterCurrentMovement CurrentMovement;

    CharacterState.ICharacterState currentState;
    public CharacterState.ICharacterState CurrentState {
        get { return currentState; }
        set { SetCurrentState(value); }
    }

    Vector2 facingDirection = new Vector2(1, 0);
    public Vector2 FacingDirection {
        get { return facingDirection; }
        set { SetFacingDirection(value); }
    }
    public Stone StoneWithinRange;
    /// <summary>
    /// Faced movable object within character's interacting range
    /// </summary>

    LayerMask movableMask;

    private Rigidbody2D rb;
    public Rigidbody2D Rb { get { return rb; } }
    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool IsStandOnClimbable;
    [HideInInspector] public bool IsBodyOnClimbable;
    [HideInInspector] public bool IsLedgeDetected;

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
        Vector3 angle = transform.rotation.eulerAngles;
        if (direction.x < 0) transform.rotation = Quaternion.Euler(angle.x, 180, angle.z);
        if (direction.x > 0) transform.rotation = Quaternion.Euler(angle.x, 0, angle.z);
        facingDirection = direction;
    }

    void Awake() {
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
        if (IsGrounded && CurrentMovement.SpringSpeed != 0 && (Time.time - CurrentMovement.lastSpringTime) >= 0.2) CurrentMovement.SpringSpeed = 0;
        if ((Time.time - CurrentMovement.lastSpringTime) >= 0.4) {
            CurrentMovement.SpringSpeed *= 0.99f;
        }
        if (CurrentState is not CharacterState.GrabbingMovable) {
            float direction = InputManager.Instance.CharacterHorizontalMove;
            if (direction != 0) FacingDirection = new(direction, 0);
        }
        if (Rb.velocity.x != 0) {
            GetComponent<Animator>().SetBool("Movement", true);
        } else {
            GetComponent<Animator>().SetBool("Movement", false);
        }
        if (Rb.velocity.y < -MovementAttributes.velocityEps &&
            currentState is not CharacterState.Dashing &&
            currentState is not CharacterState.Climbing) {
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
    }
}
