using UnityEngine;

public class CharacterJump : MonoBehaviour {
    Character character;

    [SerializeField] AudioClip jumpSound;  // Drag and drop your jump sound effect here in the Inspector
    AudioSource audioSource;  // Reference to AudioSource component

    void Start() {
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
        character = GetComponent<Character>();
    }

    void Update() {
        if (!character.CurrentMovement.IsJumpEnabled) return;
        if (Input.GetButtonDown("Jump") && character.IsGrounded) {
            character.Rb.velocity = new Vector2(character.Rb.velocity.x, character.CurrentMovement.JumpPower);
            if (jumpSound != null) {
                audioSource.PlayOneShot(jumpSound);  // Play the jump sound effect
            }
        }
    }
}