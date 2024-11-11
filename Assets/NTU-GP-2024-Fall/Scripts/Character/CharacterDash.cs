using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class CharacterDash : MonoBehaviour {
    [Header("Dash")]
    [SerializeField] float dashLength = 4f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] float dashGravityMultiplier = 0f;
    [SerializeField] AudioClip dashSound;  // Drag and drop your dash sound effect here in the Inspector
    float dashSpeed;

    Character character;
    Rigidbody2D rb;

    bool isDashCooling = false;
    TrailRenderer dashTrailRenderer;

    AudioSource audioSource;  // Reference to AudioSource component

    void Awake() {
        dashSpeed = dashLength / dashDuration;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
        character = GetComponent<Character>();
        Assert.IsTrue(dashDuration <= dashCooldown);
        dashTrailRenderer = GetComponent<TrailRenderer>();
    }

    void OnEnable() {
        InputManager.Instance.Character.Actions.Dash.performed += Dash;
    }
    void OnDisable() {
        InputManager.Instance.Character.Actions.Dash.performed -= Dash;
    }

    void Dash(InputAction.CallbackContext context) {
        if (!character.CurrentMovement.IsDashEnabled) return;

        if (!isDashCooling) {
            if (dashSound != null) {
                audioSource.PlayOneShot(dashSound);  // Play the dash sound effect
            }
            StartCoroutine(StartDash());
        }
    }

    IEnumerator StartDash() {
        isDashCooling = true;
        character.CurrentState = new CharacterState.Dashing();

        float originalGravity = rb.gravityScale;
        rb.gravityScale = dashGravityMultiplier * originalGravity;
        rb.velocity = new Vector2(character.FacingDirection.x * dashSpeed, 0);

        dashTrailRenderer.emitting = true;

        yield return new WaitForSeconds(dashDuration);

        character.CurrentState = new CharacterState.Free();

        rb.gravityScale = originalGravity;

        dashTrailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown - dashDuration);

        isDashCooling = false;
    }
}
