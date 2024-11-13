using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterJump : MonoBehaviour {
    Character character;
    Rigidbody2D rb;

    [SerializeField] float maxJumpHeight = 3.5f;
    [Tooltip("Gravity multiplier applied when the player hold jump button and the character is jumping upward.")]
    [SerializeField] float slowerFallGravityMultiplier = 0.7f;
    [SerializeField] float releaseJumpSpeedMultiplier = 0.5f;
    [Tooltip("Duration the character can still jump after leaving the ground.")]
    [SerializeField] float coyoteTime = 0.1f;
    [Tooltip("Time buffer to allow jumps even if the jump button is pressed slightly early, "
         + "ensuring intended jumps are more likely to succeed.")]
    [SerializeField] float jumpBufferTime = 0.1f;

    float coyoteTimeCounter;
    float jumpBufferCounter;

    [SerializeField] AudioClip jumpSound;
    AudioSource audioSource;

    float jumpPower => Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * character.NormalGravityScale *
             slowerFallGravityMultiplier * maxJumpHeight);

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable() {
        InputManager.Control.Character.Jump.performed += Jump;
        InputManager.Control.Character.Jump.canceled += Jump;
    }
    void OnDisable() {
        InputManager.Control.Character.Jump.performed -= Jump;
        InputManager.Control.Character.Jump.canceled -= Jump;
    }

    void Jump(InputAction.CallbackContext context) {
        if (context.performed) {
            jumpBufferCounter = jumpBufferTime;
        }
        if (context.canceled && rb.velocity.y > 0) {
            rb.gravityScale = character.NormalGravityScale;
            rb.velocity = new Vector2(rb.velocity.x, releaseJumpSpeedMultiplier * rb.velocity.y);
        }
    }

    void Update() {
        if (character.IsGrounded) {
            coyoteTimeCounter = coyoteTime;
        } else {
            coyoteTimeCounter -= Time.deltaTime;
        }
        jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f) {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            rb.gravityScale = character.NormalGravityScale * slowerFallGravityMultiplier;
            if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }
}