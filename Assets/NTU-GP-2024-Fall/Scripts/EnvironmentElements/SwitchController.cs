using UnityEngine;
using UnityEngine.InputSystem;

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

    void OnEnable() {
        InputManager.Control.Character.Interact.performed += Interact;
    }
    void OnDisable() {
        InputManager.Control.Character.Interact.performed -= Interact;
    }

    void Interact(InputAction.CallbackContext context) {
        if (!isPlayerInRange) return;
        print("Switch activated");
        spriteRenderer.color = interactionColor;

        // Call the method that handles the switch activation
        OnSwitchActivated();
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
