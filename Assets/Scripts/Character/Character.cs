using System.Collections;
using System.Diagnostics.Tracing;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class Character : MonoBehaviour {
    [Header("GroundCheck")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    LayerMask groundLayer;

    private Rigidbody2D rb;
    public Rigidbody2D Rb { get { return rb; } }
    [HideInInspector] public bool IsGrounded = true;
    [HideInInspector] public bool IsDashing = false;

    void Awake() {
        groundLayer = LayerMask.GetMask("Ground");
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}