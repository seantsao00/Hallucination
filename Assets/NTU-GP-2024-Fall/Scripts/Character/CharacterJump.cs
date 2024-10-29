using System;
using UnityEngine;

public class CharacterJump : MonoBehaviour {
    Character character;

    [SerializeField] float maxJumpHeight = 3.5f;
    [Tooltip("Gravity multiplier applied when the player hold jump button and the character is jumping upward.")]
    [SerializeField] float slowerFallGravityMultiplier = 0.7f;
    [SerializeField] float releaseJumpSpeedMultiplier = 0.5f;
    [Tooltip("Duration the character can still jump after leaving the ground.")]
    [SerializeField] float coyoteTime = 0.1f;
    [Tooltip("Time buffer to allow jumps even if the jump button is pressed slightly early, "
         + "ensuring intended jumps are more likely to succeed.")]
    [SerializeField] float jumpBufferTime = 0.1f;

    float jumpPower;

    float coyoteTimeCounter;
    float jumpBufferCounter;

    [SerializeField] AudioClip jumpSound;
    AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        character = GetComponent<Character>();
    }

    void Start() {
        jumpPower = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * character.NormalGravityScale *
            slowerFallGravityMultiplier * maxJumpHeight);
    }

    void Update() {
        if (character.IsGrounded) {
            coyoteTimeCounter = coyoteTime;
        } else {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (!character.CurrentMovement.IsJumpEnabled) return;

        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferTime;
        } else {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f) {
            character.Rb.velocity = new Vector2(character.Rb.velocity.x, jumpPower);
            character.Rb.gravityScale = character.NormalGravityScale * slowerFallGravityMultiplier;
            if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
        if (Input.GetButtonUp("Jump") && character.Rb.velocity.y > 0) {
            character.Rb.gravityScale = character.NormalGravityScale;
            character.Rb.velocity = new Vector2(character.Rb.velocity.x,
                releaseJumpSpeedMultiplier * character.Rb.velocity.y);
        }
    }
}