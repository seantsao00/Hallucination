using UnityEngine;

public class DisappearingPlatform: MonoBehaviour {
    [SerializeField] float timeToDisappear = 3f;
    [SerializeField] float timeToReappear = 2f;

    bool playerOnPlatform = false;
    bool playerOverlappingPlatform = false;
    bool isDisappeared;
    float timeOnPlatform = 0f;
    float timeOffPlatform = 0f;

    Renderer platformRenderer;
    Collider2D platformCollider;

    // Added variables for transparency
    Color platformColor;
    float initialAlpha, disappearAlpha = 0.3f;

    // Layer indices
    int defaultLayer;
    int groundLayer;

    void Start() {
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider2D>();

        platformColor = platformRenderer.material.color;
        initialAlpha = platformColor.a;

        defaultLayer = LayerMask.NameToLayer("Default");
        groundLayer = LayerMask.NameToLayer("Ground");

        gameObject.layer = groundLayer;
    }

    void Disappear() {
        SetPlatformTransparency(disappearAlpha);
        isDisappeared = true;
        platformCollider.isTrigger = true;
        playerOnPlatform = false; // Reset because the platform is now pass-through
        timeOnPlatform = 0f;
        gameObject.layer = defaultLayer;
    }

    void Appear() {
        SetPlatformTransparency(initialAlpha);
        isDisappeared = false;
        platformCollider.isTrigger = false;
        timeOffPlatform = 0f;
        gameObject.layer = groundLayer;
    }

    void Update() {
        if (playerOnPlatform) {
            timeOnPlatform += Time.deltaTime;
            if (timeOnPlatform >= timeToDisappear) {
                Disappear();
            }
        } else {
            if (platformColor.a < initialAlpha) {
                if (!playerOverlappingPlatform) {
                    timeOffPlatform += Time.deltaTime;
                    if (timeOffPlatform >= timeToReappear) {
                        Appear();
                    }
                } else {
                    timeOffPlatform = 0f;
                }
            } else {
                if (timeOnPlatform > 0f) {
                    Disappear();
                }
                timeOnPlatform = 0f;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            foreach (ContactPoint2D contact in collision.contacts) {
                // The normal points from the platform to the player
                // If the normal is pointing upwards, the player is on top
                if (contact.normal.y < -0.9f) {
                    playerOnPlatform = true;
                    return;
                }
            }
            // If none of the normals are pointing upwards, the player is not on top
            playerOnPlatform = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            foreach (ContactPoint2D contact in collision.contacts) {
                if (contact.normal.y < -0.9f) {
                    playerOnPlatform = true;
                    return;
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

    private void SetPlatformTransparency(float alpha) {
        platformColor.a = alpha;
        platformRenderer.material.color = platformColor;
    }
}
