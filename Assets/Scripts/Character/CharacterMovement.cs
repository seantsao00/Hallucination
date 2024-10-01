using System.Collections;
using System.Diagnostics.Tracing;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class CharacterMovement : MonoBehaviour {

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpPower = 10f;

    private Character character;
    /// <summary> The moving direction of the character. </summary>
    /// <remarks> Note that this is not the current speed of the character. </remarks>
    private Vector2 movement;

    public AudioClip jumpSound;  // Drag and drop your jump sound effect here in the Inspector
    private AudioSource audioSource;  // Reference to AudioSource component

    void Start() {
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
        character = GetComponent<Character>();
    }

    void Update() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Debug.Log($"Layer Index: {gameObject.layer}, Layer Name: {LayerMask.LayerToName(gameObject.layer)}");
        if (!character.IsClimbing && character.OverlappedClimbalbe != null && movement.y != 0) {
            character.IsClimbing = true;
        }
        if (character.IsClimbing && character.IsGrounded && movement.y == 0) character.IsClimbing = false;
        if (character.IsClimbing && character.OverlappedClimbalbe == null) character.IsClimbing = false;

        if (!character.IsClimbing) {
            if (Input.GetButtonDown("Jump") && character.IsGrounded) {
                character.Rb.velocity = new Vector2(character.Rb.velocity.x, jumpPower);
                if (jumpSound != null)
                {
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
