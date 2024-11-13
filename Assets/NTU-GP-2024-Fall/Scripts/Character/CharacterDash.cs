using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class CharacterDash : MonoBehaviour {
    [Header("Dash")]
    [SerializeField] float dashLength = 4f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] AudioClip dashSound;  // Drag and drop your dash sound effect here in the Inspector
    float dashSpeed;

    Character character;
    Rigidbody2D rb;

    bool isDashCooling = false;
    TrailRenderer dashTrailRenderer;

    AudioSource audioSource;  // Reference to AudioSource component
    CharacterStateController characterStateController;

    void Awake() {
        dashSpeed = dashLength / dashDuration;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
        character = GetComponent<Character>();
        Assert.IsTrue(dashDuration <= dashCooldown);
        dashTrailRenderer = GetComponent<TrailRenderer>();
        characterStateController = GetComponent<CharacterStateController>();
    }

    void OnEnable() {
        InputManager.Control.Character.Dash.performed += Dash;
    }
    void OnDisable() {
        InputManager.Control.Character.Dash.performed -= Dash;
    }

    void Dash(InputAction.CallbackContext context) {
        if (isDashCooling) return;
        if (dashSound != null) {
            audioSource.PlayOneShot(dashSound);  // Play the dash sound effect
        }
        StartCoroutine(StartDash());
    }

    IEnumerator StartDash() {
        isDashCooling = true;
        characterStateController.AddState(CharacterState.Dashing);
        rb.velocity = new Vector2(character.FacingDirection.x * dashSpeed, 0);
        dashTrailRenderer.emitting = true;
        InputManager.Control.Character.HorizontalMove.Disable();
        yield return new WaitForSeconds(dashDuration);

        characterStateController.RemoveState(CharacterState.Dashing);
        dashTrailRenderer.emitting = false;
        InputManager.Control.Character.HorizontalMove.Enable();
        yield return new WaitForSeconds(dashCooldown - dashDuration);

        isDashCooling = false;
    }
}
