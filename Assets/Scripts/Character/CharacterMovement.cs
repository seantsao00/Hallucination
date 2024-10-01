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

    void Start() {
        character = GetComponent<Character>();
    }

    void Update() {
        movement.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && character.IsGrounded) {
            character.Rb.velocity = new Vector2(character.Rb.velocity.x, jumpPower);
        }
    }

    void FixedUpdate() {
        if (!character.IsDashing) {
            character.Rb.velocity = new Vector2(movement.x * moveSpeed, character.Rb.velocity.y);
        }
    }
}
