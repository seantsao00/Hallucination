using System.Collections;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class CharacterDash : MonoBehaviour {
    [Header("Dash")]
    [Tooltip("The speed of the dash in units/second.")]
    [SerializeField] float dashSpeed = 20f;
    [Tooltip("How long the dash lasts, in seconds. This value must less than Dash Cooldown.")]
    [SerializeField] float dashDurationSeconds = 0.2f;
    [Tooltip("Cooldown time before the character can dash again, in seconds.")]
    [SerializeField] float dashCooldownSeconds = 1f;
    [Tooltip("The scaling factor of gravity when dashing.")]
    [SerializeField] float dashGravityScale = 0.2f;

    Character character;

    bool isDashCooling = false;
    bool isDashReset = true;
    TrailRenderer dashTrailRenderer;
    Vector2 facingDirection = new Vector2(1, 0);

    void Start() {
        character = GetComponent<Character>();
        Assert.IsTrue(dashDurationSeconds <= dashCooldownSeconds);
        dashTrailRenderer = GetComponent<TrailRenderer>();
    }

    void Update() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
            facingDirection.x = horizontal;
        if (character.IsGrounded) isDashReset = true;
        if (!isDashCooling && Input.GetButtonDown("Dash") && isDashReset) {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash() {
        isDashCooling = true;
        character.IsDashing = true;
        isDashReset = false;
        float originalGravity = character.Rb.gravityScale;
        character.Rb.gravityScale = dashGravityScale * originalGravity;
        character.Rb.velocity = new Vector2(facingDirection.x * dashSpeed, 0);
        dashTrailRenderer.emitting = true;
        yield return new WaitForSeconds(dashDurationSeconds);
        dashTrailRenderer.emitting = false;
        character.Rb.gravityScale = originalGravity;
        character.IsDashing = false;
        yield return new WaitForSeconds(dashDurationSeconds - dashCooldownSeconds);
        isDashCooling = false;
    }
}
