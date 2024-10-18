using System;
using System.Collections;
using System.Diagnostics.Tracing;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class CharacterMovement : MonoBehaviour {
    Character character;
    Vector2 movement;

    [SerializeField] AudioClip jumpSound;  // Drag and drop your jump sound effect here in the Inspector
    AudioSource audioSource;  // Reference to AudioSource component

    void Start() {
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
        character = GetComponent<Character>();
    }

    void Update() {
        if (character.CurrentState is not CharacterState.Transporting) {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        } else {
            movement.x = movement.y = 0;
        }


        // climb
        if (character.CurrentState is CharacterState.Free && character.OverlappedClimbalbe != null && movement.y != 0) {
            character.CurrentState = new CharacterState.Climbing();
        }
        if (character.CurrentState is CharacterState.Climbing) {
            if (character.IsGrounded && movement.y == 0)
                character.CurrentState = new CharacterState.Free();
            if (character.OverlappedClimbalbe == null)
                character.CurrentState = new CharacterState.Free();
        }

        // normal move
        if (character.CurrentState is CharacterState.Free || character.CurrentState is CharacterState.Dashing) {
            if (Input.GetButtonDown("Jump") && character.IsGrounded) {
                character.Rb.velocity = new Vector2(character.Rb.velocity.x, character.JumpPower);
                if (jumpSound != null) {
                    audioSource.PlayOneShot(jumpSound);  // Play the jump sound effect
                }
            }
        }
    }

    void FixedUpdate() {
        if (character.CurrentState is not CharacterState.Dashing
            && character.CurrentState is not CharacterState.Climbing
        ) {
            character.Rb.velocity = new Vector2(movement.x * character.CurrentSpeed, character.Rb.velocity.y);
        }
        if (character.CurrentState is CharacterState.Climbing) {
            character.Rb.velocity = new Vector2(0, movement.y * character.CurrentSpeed);
        }
    }

}
