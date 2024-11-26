using System;
using UnityEngine;
using System.Collections;

public enum CharacterTypeEnum { None, Fairy, Bear };

/// <summary>
/// The <c>Character</c> class maintains the character's current state, including movement states 
/// (e.g., jumping, dashing) and speed control (e.g., maximum fall speed).
/// This class provides methods and properties to change character states and movement, ensuring 
/// that updates notify all necessary dependencies.
/// </summary>
public class Character : MonoBehaviour {
    [Serializable]
    public class CharacterMovementAttributes {
        [Header("HorizontalMovement")]
        public float NormalHorizontalSpeed = 5f;

        [Header("Behavior")]
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

    private Rigidbody2D rb;
    bool isGrounded;
    [HideInInspector]
    public bool IsGrounded {
        get => isGrounded;
        set {
            isGrounded = value;
            if (isGrounded) characterStateController.RemoveState(CharacterState.NotStandingOnGround);
            else characterStateController.AddState(CharacterState.NotStandingOnGround);
        }
    }
    [HideInInspector] public bool IsStandOnClimbable;
    [HideInInspector] public bool IsBodyOnClimbable;
    [HideInInspector] public bool IsLedgeDetected;

    [HideInInspector] public bool IsDead = false;

    public bool isFairy;

    float springFullSpeedDuration = 0.2f;
    float springFullSpeed;
    float springDuration = 0.4f;
    float springTimeCounter;

    private void SetFacingDirection(Vector2 direction) {
        Vector3 angle = transform.rotation.eulerAngles;
        if (direction.x < 0) transform.rotation = Quaternion.Euler(angle.x, 180, angle.z);
        if (direction.x > 0) transform.rotation = Quaternion.Euler(angle.x, 0, angle.z);
        facingDirection = direction;
    }

    void Awake() {
        CurrentMovement = new CharacterCurrentMovement(MovementAttributes);
        rb = GetComponent<Rigidbody2D>();
        characterStateController = GetComponent<CharacterStateController>();
        WorldSwitchManager.Instance.WorldSwitching.AddListener(StopMotion);
        WorldSwitchManager.Instance.WorldSwitched.AddListener(StopMotion);
    }

    void OnDestroy() {
        WorldSwitchManager.Instance.WorldSwitching.RemoveListener(StopMotion);
        WorldSwitchManager.Instance.WorldSwitched.RemoveListener(StopMotion);
    }

    public void LaunchSpring(float speed) {
        springTimeCounter = 0;
        springFullSpeed = speed;
        CurrentMovement.SpringSpeed = speed;
    }

    void Update() {
        if (springTimeCounter > springFullSpeedDuration) {
            if (springTimeCounter > springDuration) CurrentMovement.SpringSpeed = 0;
            else CurrentMovement.SpringSpeed -= 
                Mathf.Sign(CurrentMovement.SpringSpeed)
                * springFullSpeed
                / (springDuration - springFullSpeedDuration)
                * Time.deltaTime;
        }
        springTimeCounter += Time.deltaTime;
        if (!characterStateController.HasState(CharacterState.Grabbing)) {
            float direction = InputManager.Instance.CharacterHorizontalMove;
            if (direction != 0) FacingDirection = new(direction, 0);
        }
        if (rb.velocity.y < -MovementAttributes.velocityEps) {
            if (Mathf.Abs(rb.velocity.y) < MovementAttributes.AirHangTimeThresholdSpeed) {
                characterStateController.AddState(CharacterState.AirHanging);
            } else {
                characterStateController.RemoveState(CharacterState.AirHanging);
                characterStateController.RemoveState(CharacterState.PreReleaseJumping);
            }
        }
        if (rb.velocity.y <= -MovementAttributes.MaxFallingSpeed) {
            rb.velocity = new Vector2(rb.velocity.x, -MovementAttributes.MaxFallingSpeed);
        }
    }

    public void StopMotion() {
        rb.velocity = new Vector2(0, 0);
    }
}
