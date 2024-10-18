using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterDash : MonoBehaviour {
    [Header("Dash")]
    [Tooltip("The speed of the dash in units/second.")]
    [SerializeField] float dashSpeed = 20f;
    [Tooltip("How long the dash lasts, in seconds. This value must less than Dash Cooldown.")]
    [SerializeField] float dashDuration = 0.2f;
    [Tooltip("Cooldown time before the character can dash again, in seconds.")]
    [SerializeField] float dashCooldown = 1f;
    [Tooltip("The scaling factor of gravity when dashing.")]
    [SerializeField] float dashGravityScale = 0.2f;
    [SerializeField] AudioClip dashSound;  // Drag and drop your dash sound effect here in the Inspector

    Character character;

    bool isDashCooling = false;
    bool isDashReset = true;
    TrailRenderer dashTrailRenderer;

    AudioSource audioSource;  // Reference to AudioSource component

    void Start() {
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
        character = GetComponent<Character>();
        Assert.IsTrue(dashDuration <= dashCooldown);
        dashTrailRenderer = GetComponent<TrailRenderer>();
    }

    void Update() {
        if (character.IsGrounded) isDashReset = true;
        if (character.CurrentState is not CharacterState.Free) return;

        if (!isDashCooling && Input.GetButtonDown("Dash")) {
            // Play dash sound effect
            if (dashSound != null) {
                audioSource.PlayOneShot(dashSound);  // Play the dash sound effect
            }
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash() {
        isDashCooling = true;
        character.CurrentState = new CharacterState.Dashing();
        isDashReset = false;

        float originalGravity = character.Rb.gravityScale;
        character.Rb.gravityScale = dashGravityScale * originalGravity;
        character.Rb.velocity = new Vector2(character.FacingDirection.x * dashSpeed, 0);

        dashTrailRenderer.emitting = true;

        yield return new WaitForSeconds(dashDuration);

        character.CurrentState = new CharacterState.Free();

        character.Rb.gravityScale = originalGravity;

        dashTrailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown - dashDuration);

        isDashCooling = false;
    }
}
