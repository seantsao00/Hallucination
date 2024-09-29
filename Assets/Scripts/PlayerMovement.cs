using System.Collections;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Dash")]
    [Tooltip("Whether the player can dash.")]
    [SerializeField] private bool canDash = true;
    [Tooltip("The speed of the dash in units/second.")]
    [SerializeField] private float dashSpeed = 20f;
    [Tooltip("How long the dash lasts, in seconds. This value must not larger than Dash Cooldown.")]
    [SerializeField] private float dashDurationSeconds = 0.2f;
    [Tooltip("Cooldown time before the player can dash again, in seconds.")]
    [SerializeField] private float dashCooldownSeconds = 1f;
    [Tooltip("The scaling factor of gravity when dashing.")]
    [SerializeField] private float dashGravityScale = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    /// <summary> The moving direction of the player. </summary>
    /// <remarks> Note that this is not the current speed of the player. </remarks>
    private Vector2 movement;

    private bool isDashing = false;
    private bool isDashCooling = false;
    /// <summary>
    /// Whether the player has touched the ground after the last dash.
    /// The player should not able to dash twice without touching the ground.
    /// </summary>
    private bool isDashReset = true;
    private TrailRenderer dashTrailRenderer;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (canDash) {
            Assert.IsTrue(dashDurationSeconds <= dashCooldownSeconds);
            dashTrailRenderer = GetComponent<TrailRenderer>();
        }
    }

    void Update() {
        if (!isDashing) {
            movement.x = Input.GetAxisRaw("Horizontal");
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded) isDashReset = true;
        if (Input.GetButtonDown("Jump") && isGrounded) {

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (canDash && !isDashCooling && Input.GetButtonDown("Dash") && isDashReset) {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate() {
        if (!isDashing) {
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    IEnumerator Dash() {
        isDashCooling = true;
        isDashing = true;
        isDashReset = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = dashGravityScale * originalGravity;
        rb.velocity = new Vector2(movement.x * dashSpeed, 0);
        dashTrailRenderer.emitting = true;
        yield return new WaitForSeconds(dashDurationSeconds);
        dashTrailRenderer.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashDurationSeconds - dashCooldownSeconds);
        isDashCooling = false;
    }
}
