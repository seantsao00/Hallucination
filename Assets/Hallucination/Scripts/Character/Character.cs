using System;
using System.Collections;
using UnityEngine;

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
        [Header("Behavior")]
        public float AirHangTimeThresholdSpeed = 3f;
        public float StickOnWallFallingSpeed = 3f;
        public float MaxFallingSpeed = 16f;
        [HideInInspector, NonSerialized] public float velocityEps = 1e-3f;
    }

    public CharacterMovementAttributes MovementAttributes;

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
    [HideInInspector] public bool IsStandOnClimbable;
    [HideInInspector] public bool IsBodyOnClimbable;
    [HideInInspector] public bool IsFootOnGround;
    [HideInInspector] public bool IsBellyInGround;
    [HideInInspector] public bool IsLedgeDetected;

    Coroutine springCoroutine;

    void SetFacingDirection(Vector2 direction) {
        Vector3 angle = transform.rotation.eulerAngles;
        if (direction.x < 0) transform.rotation = Quaternion.Euler(angle.x, 180, angle.z);
        if (direction.x > 0) transform.rotation = Quaternion.Euler(angle.x, 0, angle.z);
        facingDirection = direction;
    }

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        characterStateController = GetComponent<CharacterStateController>();
        WorldSwitchManager.Instance.WorldStartSwitching.AddListener(StopMotion);
        WorldSwitchManager.Instance.WorldSwitching.AddListener(StopMotion);
    }

    void OnDestroy() {
        // Debug.Log("Character Destroyed");
        WorldSwitchManager.Instance.WorldStartSwitching.RemoveListener(StopMotion);
        WorldSwitchManager.Instance.WorldSwitching.RemoveListener(StopMotion);
    }

    void Update() {
        if (!characterStateController.HasState(CharacterState.Grabbing)) {
            float direction = InputManager.Instance.CharacterHorizontalMove;
            if (direction != 0) FacingDirection = new(direction, 0);
        }
        if (rb.velocity.y < -MovementAttributes.velocityEps) {
            characterStateController.RemoveState(CharacterState.PreReleaseJumping);
            if (Mathf.Abs(rb.velocity.y) < MovementAttributes.AirHangTimeThresholdSpeed) {
                characterStateController.AddState(CharacterState.AirHanging);
            } else {
                characterStateController.RemoveState(CharacterState.AirHanging);
            }
        } else {
            characterStateController.RemoveState(CharacterState.AirHanging);
        }
        if (rb.velocity.y <= -MovementAttributes.MaxFallingSpeed) {
            rb.velocity = new Vector2(rb.velocity.x, -MovementAttributes.MaxFallingSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // If the character's body touches the ground on one of the 2 sides, stop the horizontal speed given by the spring.
        if (springCoroutine != null) {
            if (((1 << collision.gameObject.layer) & LayerMask.NameToLayer("Ground")) != 0) {
                foreach (ContactPoint2D contact in collision.contacts) {
                    float angle = Vector2.Angle(contact.normal, Vector2.up);
                    if (Mathf.Approximately(angle, 90)) {
                        StopSpringHorizontalSpeed();
                        Debug.Log("Spring duration stopped");
                        break;
                    }
                }
            }
        }
    }

    public void GainSpringHorizontalSpeed(Vector2 springStartPosition, Vector2 speed, float duration) =>
        springCoroutine = StartCoroutine(HandleSpringHorizontalSpeed(springStartPosition, speed, duration));

    IEnumerator HandleSpringHorizontalSpeed(Vector2 springStartPosition, Vector2 speed, float duration) {
        characterStateController.AddState(CharacterState.HorizontalSpringFlying);
        // make sure the character is above the spring before starting the timer
        do {
            rb.velocity = speed;
            yield return null;
        } while (transform.position.y < springStartPosition.y);
        yield return new WaitForSeconds(duration);
        springCoroutine = null;
        characterStateController.RemoveState(CharacterState.HorizontalSpringFlying);
    }

    public void StopSpringHorizontalSpeed() {
        if (springCoroutine != null) {
            // Debug.Log("Spring duration stopped");
            StopCoroutine(springCoroutine);
            springCoroutine = null;
            characterStateController.RemoveState(CharacterState.HorizontalSpringFlying);
        }
    }

    public void StopMotion() {
        if (rb != null) rb.velocity = new Vector2(0, 0);
        GetComponent<CharacterDash>()?.ResetDash();
        StopSpringHorizontalSpeed();
    }
}
