using System.Collections;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class PlayerDash : MonoBehaviour {
    [Header("Dash")]
    [Tooltip("The speed of the dash in units/second.")]
    [SerializeField] private float dashSpeed = 20f;
    [Tooltip("How long the dash lasts, in seconds. This value must less than Dash Cooldown.")]
    [SerializeField] private float dashDurationSeconds = 0.2f;
    [Tooltip("Cooldown time before the player can dash again, in seconds.")]
    [SerializeField] private float dashCooldownSeconds = 1f;
    [Tooltip("The scaling factor of gravity when dashing.")]
    [SerializeField] private float dashGravityScale = 0.2f;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    private bool isDashCooling = false;
    private bool isDashReset = true;
    private TrailRenderer dashTrailRenderer;
    private Vector2 facingDirection = new Vector2(1, 0);

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        Assert.IsTrue(dashDurationSeconds <= dashCooldownSeconds);
        dashTrailRenderer = GetComponent<TrailRenderer>();
    }

    void Update() {
        facingDirection.x = Input.GetAxisRaw("Horizontal");
        bool isGrounded = playerMovement.IsGrounded;
        if (isGrounded) isDashReset = true;
        if (!isDashCooling && Input.GetButtonDown("Dash") && isDashReset) {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash() {
        isDashCooling = true;
        playerMovement.isDashing = true;
        isDashReset = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = dashGravityScale * originalGravity;
        rb.velocity = new Vector2(facingDirection.x * dashSpeed, 0);
        dashTrailRenderer.emitting = true;
        yield return new WaitForSeconds(dashDurationSeconds);
        dashTrailRenderer.emitting = false;
        rb.gravityScale = originalGravity;
        playerMovement.isDashing = false;
        yield return new WaitForSeconds(dashDurationSeconds - dashCooldownSeconds);
        isDashCooling = false;
    }
}
