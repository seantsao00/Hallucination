using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterDash : MonoBehaviour {
    [Header("Dash")]
    [SerializeField] float dashLength = 4f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] float dashGravityScale = 0f;
    [SerializeField] AudioClip dashSound;  // Drag and drop your dash sound effect here in the Inspector
    float dashSpeed;

    Character character;

    bool isDashCooling = false;
    TrailRenderer dashTrailRenderer;

    AudioSource audioSource;  // Reference to AudioSource component

    void Awake() {
        dashSpeed = dashLength / dashDuration;
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
        character = GetComponent<Character>();
        Assert.IsTrue(dashDuration <= dashCooldown);
        dashTrailRenderer = GetComponent<TrailRenderer>();
    }

    void Update() {
        if (!character.CurrentMovement.IsDashEnabled) return;

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
