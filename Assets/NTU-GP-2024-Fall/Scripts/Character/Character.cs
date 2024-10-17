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
    bool isSittingOnBench = false;
    public bool IsSittingOnBench {
        get { return isSittingOnBench; }
        set { SetSittingState(value); }
    }

    [HideInInspector] public Collider2D OverlappedClimbalbe;

    [HideInInspector] public bool IsDashing = false;

    private float normalGravityScale;

    bool isClimbing;
    bool isTransporting;
    public bool IsClimbing {
        get { return isClimbing; }
        set { SetClimbingState(value); }
    }
    public bool IsTransporting {
        get { return isTransporting; }
        set { SetTransportingState(value); }
    }

    private void SetSittingState(bool sitting) {
        isSittingOnBench = sitting;
    }
    private void SetTransportingState(bool transporting) {
        isTransporting = transporting;
        if (isTransporting) {
            gameObject.layer = aheadGroundLayer;
            Rb.gravityScale = 0;
        } else {
            gameObject.layer = LayerMask.NameToLayer("Default");
            Rb.gravityScale = normalGravityScale;
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

    public bool isDead = false;

    void Awake() {
        groundLayerMask = LayerMask.GetMask("Ground");
        climbableLayerMask = LayerMask.GetMask("Climbable");
        aheadGroundLayer = LayerMask.NameToLayer("AheadGround");
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        isTransporting = false;
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