using System.Collections;
using System.Diagnostics.Tracing;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class CharacterMovement : MonoBehaviour {
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpPower = 10f;

    Character character;
    Vector2 movement;

    [SerializeField] AudioClip jumpSound;  // Drag and drop your jump sound effect here in the Inspector
    AudioSource audioSource;  // Reference to AudioSource component

    void Start() {
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
        character = GetComponent<Character>();
    }

    void Update() {
        if (!character.IsTransporting) {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        } else {
            movement.x = movement.y = 0;
        }
        

        // climb
        if (!character.IsClimbing && character.OverlappedClimbalbe != null && movement.y != 0) {
            character.IsClimbing = true;
        }
        if (character.IsClimbing && character.IsGrounded && movement.y == 0) character.IsClimbing = false;
        if (character.IsClimbing && character.OverlappedClimbalbe == null) character.IsClimbing = false;

        // normal move
        if (!character.IsClimbing) {
            if (Input.GetButtonDown("Jump") && character.IsGrounded && !character.IsTransporting) {
                character.Rb.velocity = new Vector2(character.Rb.velocity.x, jumpPower);
                if (jumpSound != null) {
                    audioSource.PlayOneShot(jumpSound);  // Play the jump sound effect
                }
            }
        }
    }

    void FixedUpdate() {
        if (!character.IsDashing && !character.IsClimbing) {
            character.Rb.velocity = new Vector2(movement.x * moveSpeed, character.Rb.velocity.y);
        }
        if (character.IsClimbing) {
            character.Rb.velocity = new Vector2(0, movement.y * moveSpeed);
        }
    }

}
