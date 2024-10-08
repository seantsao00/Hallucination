using System.Collections;
using System.Diagnostics.Tracing;
using System.Threading;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class Character : MonoBehaviour {
    [Header("GroundCheck")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    LayerMask groundLayerMask, climbableLayerMask;
    int aheadGroundLayer;

    private Rigidbody2D rb;
    public Rigidbody2D Rb { get { return rb; } }
    [HideInInspector] public bool IsGrounded = true;
    [HideInInspector] public Collider2D OverlappedClimbalbe;

    [HideInInspector] public bool IsDashing = false;

    private float normalGravityScale;

    bool isClimbing;
    public bool IsClimbing {
        get { return isClimbing; }
        set {
            SetClimbingState(value);
        }
    }
    private void SetClimbingState(bool climbing) {
        isClimbing = climbing;
        if (isClimbing) {
            gameObject.layer = aheadGroundLayer;
            Rb.gravityScale = 0;
            Rb.velocity = new Vector2(0, Rb.velocity.y);
        } else {
            gameObject.layer = LayerMask.NameToLayer("Default");
            Rb.gravityScale = normalGravityScale;
        }
    }

    void Awake() {
        groundLayerMask = LayerMask.GetMask("Ground");
        climbableLayerMask = LayerMask.GetMask("Climbable");
        aheadGroundLayer = LayerMask.NameToLayer("AheadGround");
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        normalGravityScale = Rb.gravityScale;
    }

    void Update() {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        OverlappedClimbalbe = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, climbableLayerMask);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}