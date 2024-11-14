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

        public float SpringSpeed;

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
        }
        public void LaunchSpring(float speed) {
            SpringSpeed = speed;
            lastSpringTime = Time.time;
        }
    }

    public CharacterMovementAttributes MovementAttributes;
    public CharacterCurrentMovement CurrentMovement;

    CharacterStateController characterStateController;

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
    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool IsStandOnClimbable;
    [HideInInspector] public bool IsBodyOnClimbable;
    [HideInInspector] public bool IsLedgeDetected;

    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public float NormalGravityScale;
    SpriteRenderer spriteRenderer;

    [SerializeField] TipManager tipManager;
    string grabTip = "Hold E or C to move the stone";


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
        NormalGravityScale = rb.gravityScale;
        characterStateController = GetComponent<CharacterStateController>();
        characterStateController.OnStateChanged += HandleStateChange;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void HandleStateChange(CharacterState state, bool added) {
        if (characterStateController.HasState(CharacterState.Climbing) ||
            characterStateController.HasState(CharacterState.Dashing)) {
            rb.gravityScale = 0;
        } else if (characterStateController.HasState(CharacterState.BeingBlown)) {
            rb.gravityScale = NormalGravityScale * MovementAttributes.AirHangTimeGravityMultiplier;
        } else {
            rb.gravityScale = NormalGravityScale;
        }
    }

    void Update() {
        if (IsGrounded && CurrentMovement.SpringSpeed != 0 && (Time.time - CurrentMovement.lastSpringTime) >= 0.2) CurrentMovement.SpringSpeed = 0;
        if ((Time.time - CurrentMovement.lastSpringTime) >= 0.4) {
            CurrentMovement.SpringSpeed *= 0.99f;
        }
        if (!characterStateController.HasState(CharacterState.Grabbing)) {
            float direction = InputManager.Instance.CharacterHorizontalMove;
            if (direction != 0) FacingDirection = new(direction, 0);
        }
        if (rb.velocity.x != 0) {
            GetComponent<Animator>().SetBool("Movement", true);
        } else {
            GetComponent<Animator>().SetBool("Movement", false);
        }
        if (
            rb.velocity.y < -MovementAttributes.velocityEps &&
            !characterStateController.HasState(CharacterState.Dashing) &&
            !characterStateController.HasState(CharacterState.BeingBlown) &&
            !characterStateController.HasState(CharacterState.Climbing)
        ) {
            if (Mathf.Abs(rb.velocity.y) < MovementAttributes.AirHangTimeThresholdSpeed) {
                rb.gravityScale = NormalGravityScale * MovementAttributes.AirHangTimeGravityMultiplier;
            } else {
                rb.gravityScale = NormalGravityScale;
            }
        }
        if (rb.velocity.y <= -MovementAttributes.MaxFallingSpeed) {
            rb.velocity = new Vector2(rb.velocity.x, -MovementAttributes.MaxFallingSpeed);
        }
    }
}
