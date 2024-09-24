using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f;   // Speed of the character

    private Rigidbody2D rb;        // Reference to the Rigidbody2D component
    private Vector2 movement;      // Stores the movement input

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component attached to the player
    }

    // Update is called once per frame
    void Update() {
        // Get input from W, A, S, D keys (Vertical = W/S, Horizontal = A/D)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    // FixedUpdate is called at a fixed time interval (used for physics-related updates)
    void FixedUpdate() {
        // Move the character by adjusting its velocity
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
