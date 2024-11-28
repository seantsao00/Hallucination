using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterDash : MonoBehaviour {
    [SerializeField] float dashLength = 4f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 0.3f;
    [SerializeField] AudioClip dashSound;  // Drag and drop your dash sound effect here in the Inspector
    float dashSpeed => dashLength / dashDuration;
    bool canDash;

    Character character;
    Rigidbody2D rb;

    bool isDashCooling = false;
    TrailRenderer dashTrailRenderer;

    AudioSource audioSource;
    CharacterStateController characterStateController;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        character = GetComponent<Character>();
        dashTrailRenderer = GetComponent<TrailRenderer>();
        characterStateController = GetComponent<CharacterStateController>();
    }

    void OnEnable() {
        InputManager.Control.Character.Dash.performed += Dash;
    }
    void OnDisable() {
        InputManager.Control.Character.Dash.performed -= Dash;
    }

    void LateUpdate() {
        if (character.IsGrounded) canDash = true;
    }

    /// <summary>
    /// Stop the current dash, reset the dash cooldown and allow the player to dash again.
    /// </summary>
    public void ResetDash() {
        StopAllCoroutines();
        characterStateController.RemoveState(CharacterState.Dashing);
        dashTrailRenderer.emitting = false;
        canDash = true;
        isDashCooling = false;
    }

    void Dash(InputAction.CallbackContext context) {
        if (isDashCooling || !canDash) return;
        if (dashSound != null) {
            audioSource.PlayOneShot(dashSound);
        }
        StartCoroutine(PerformDash());
    }

    IEnumerator PerformDash() {
        isDashCooling = true;
        characterStateController.AddState(CharacterState.Dashing);
        rb.velocity = new Vector2(character.FacingDirection.x * dashSpeed, 0);
        dashTrailRenderer.emitting = true;
        yield return new WaitForSeconds(dashDuration);

        characterStateController.RemoveState(CharacterState.Dashing);
        dashTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldown);

        isDashCooling = false;
    }
}
