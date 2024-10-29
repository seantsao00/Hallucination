using System;
using UnityEngine;

public class CharacterJump : MonoBehaviour {
    Character character;

    [SerializeField] float maxJumpHeight = 3.5f;
    [Tooltip("Gravity multiplier applied when the player hold jump button and the character is jumping upward.")]
    [SerializeField] float slowerFallGravityMultiplier = 0.4f;
    [Tooltip("Gravity multiplier applied when the character’s vertical falling speed falls below the threshold.")]
    [SerializeField] float jumpHangTimeGravityMultiplier = 0.3f;
    [SerializeField] float jumpHangTimeThresholdSpeed = 0.5f;
    float jumpPower;

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
        if (!character.CurrentMovement.IsJumpEnabled) return;
        if (Input.GetButtonDown("Jump") && character.IsGrounded) {
            character.Rb.velocity = new Vector2(character.Rb.velocity.x, jumpPower);
            character.Rb.gravityScale = character.NormalGravityScale * slowerFallGravityMultiplier;
            if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
        }
        if (Input.GetButtonUp("Jump") && character.Rb.velocity.y > 0) {
            character.Rb.gravityScale = character.NormalGravityScale;
        }

        if (character.Rb.velocity.y <= 0) {
            if (Mathf.Abs(character.Rb.velocity.y) < jumpHangTimeThresholdSpeed) {
                character.Rb.gravityScale = character.NormalGravityScale * jumpHangTimeGravityMultiplier;
            } else {
                character.Rb.gravityScale = character.NormalGravityScale;
            }
        }
    }
}