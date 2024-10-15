using UnityEngine;

public class DisappearingPlatformManager : MonoBehaviour {
    public float timeToDisappear = 3f; // Time the player has to stay for the platform to disappear
    public float timeToReappear = 2f;  // Time after the player leaves for the platform to reappear.

    bool playerOnPlatform = false;
    bool playerOverlappingPlatform = false; // New variable
    float timeOnPlatform = 0f;
    float timeOffPlatform = 0f;

    Renderer platformRenderer;
    Collider2D platformCollider;

    // Added variables for transparency
    Color platformColor;
    float initialAlpha;

    // Layer indices
    int defaultLayer;
    int groundLayer;

    void Start() {
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider2D>();

        // Store the initial color and alpha
        platformColor = platformRenderer.material.color;
        initialAlpha = platformColor.a;

        // Get layer indices
        defaultLayer = LayerMask.NameToLayer("Default"); // Replace with your default layer name if different
        groundLayer = LayerMask.NameToLayer("Ground");   // Ensure this matches your actual ground layer name

        // Ensure the platform starts on the ground layer
        gameObject.layer = groundLayer;
    }

    void Update() {
        if (playerOnPlatform) {
            timeOnPlatform += Time.deltaTime;
            if (timeOnPlatform >= timeToDisappear) {
                // Make platform transparent and set collider to trigger
                SetPlatformTransparency(0.3f);
                platformCollider.isTrigger = true;
                playerOnPlatform = false; // Reset because the platform is now pass-through
                timeOnPlatform = 0f;

                // Change layer to Default (or a non-colliding layer)
                gameObject.layer = defaultLayer;
            }
        } else {
            if (platformColor.a < initialAlpha) // If platform is transparent
            {
                if (!playerOverlappingPlatform) {
                    timeOffPlatform += Time.deltaTime;
                    if (timeOffPlatform >= timeToReappear) {
                        // Restore platform opacity and collider properties
                        SetPlatformTransparency(initialAlpha);
                        platformCollider.isTrigger = false;
                        timeOffPlatform = 0f; // Reset the timer

                        // Change layer back to Ground
                        gameObject.layer = groundLayer;
                    }
                } else {
                    timeOffPlatform = 0f; // Reset the timer if player is overlapping
                }
            } else {
                if (timeOnPlatform > 0f) {
                    // Ensure platform becomes transparent if timeOnPlatform exceeded
                    SetPlatformTransparency(0.3f);
                    platformCollider.isTrigger = true;
                    playerOnPlatform = false;

                    // Change layer to Default
                    gameObject.layer = defaultLayer;
                }
                timeOnPlatform = 0f; // Reset the timer if player is not on the platform
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            // Check if the player is standing on top of the platform
            foreach (ContactPoint2D contact in collision.contacts) {
                // The normal points from the platform to the player
                // If the normal is pointing upwards, the player is on top
                if (contact.normal.y < -0.9f) // Adjust if necessary based on your game's coordinate system
                {
                    playerOnPlatform = true;
                    return; // No need to check other contacts
                }
            }
            // If none of the normals are pointing upwards, the player is not on top
            playerOnPlatform = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            // Check if the player is standing on top of the platform
            foreach (ContactPoint2D contact in collision.contacts) {
                if (contact.normal.y < -0.9f) {
                    playerOnPlatform = true;
                    return; // No need to check other contacts
                }
            }
            playerOnPlatform = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            playerOnPlatform = false;
        }
    }

    // New methods for detecting player overlapping
    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerOverlappingPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerOverlappingPlatform = false;
        }
    }

    // Helper method to set platform transparency
    private void SetPlatformTransparency(float alpha) {
        platformColor.a = alpha;
        platformRenderer.material.color = platformColor;
    }
}
