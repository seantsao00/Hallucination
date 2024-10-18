using UnityEngine;

public class SwitchController : MonoBehaviour {
    public Color interactionColor = Color.green; // Color when the switch is activated
    protected SpriteRenderer spriteRenderer;
    protected Color originalColor;
    protected bool isPlayerInRange = false; // To track if the player is within range

    protected virtual void Start() {
        // Get the SpriteRenderer component to change color
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    protected virtual void Update() {
        // Check if the player is in range and presses the E key
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) {
            // Change the color of the object when E is pressed
            print("Switch activated");
            spriteRenderer.color = interactionColor;

            // Call the method that handles the switch activation
            OnSwitchActivated();
        }
    }

    // Method to be overridden by derived classes to add specific functionality
    protected virtual void OnSwitchActivated() {
        // Base class does nothing; functionality is defined in derived classes
    }

    // Detect when the player enters the range
    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) // Ensure it's the player
        {
            isPlayerInRange = true; // Player is in range
        }
    }

    // Detect when the player exits the range
    protected virtual void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) // Ensure it's the player
        {
            isPlayerInRange = false; // Player is no longer in range
            // Optionally reset the color when player leaves
            // spriteRenderer.color = originalColor;
        }
    }
}
