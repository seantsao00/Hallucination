using UnityEngine;

public class DisappearingPlatformManager : MonoBehaviour
{
    public float timeToDisappear = 2f; // Time the player has to stay for the platform to disappear
    public float timeToReappear = 3f;  // Time after the player leaves for the platform to reappear. 

    private bool playerOnPlatform = false;
    private float timeOnPlatform = 0f;
    private float timeOffPlatform = 0f;

    private Renderer platformRenderer;
    private Collider2D platformCollider; // Use Collider2D for 2D physics

    void Start()
    {
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider2D>(); // Use Collider2D
    }

    void Update()
    {
        if (playerOnPlatform)
        {
            timeOnPlatform += Time.deltaTime;
            if (timeOnPlatform >= timeToDisappear)
            {
                // Hide platform visually and make the collider a trigger
                platformRenderer.enabled = false;
                platformCollider.isTrigger = true;
                playerOnPlatform = false; // Reset because the platform is now pass-through
            }
        }
        else
        {
            if (!platformRenderer.enabled) // If platform is hidden
            {
                timeOffPlatform += Time.deltaTime;
                if (timeOffPlatform >= timeToReappear)
                {
                    // Show platform again and restore collider properties
                    platformRenderer.enabled = true;
                    platformCollider.isTrigger = false;
                    timeOffPlatform = 0f; // Reset the timer
                }
            }
            else
            {
                if (timeOnPlatform > 0f) {
                    platformRenderer.enabled = false;
                    platformCollider.isTrigger = true;
                    playerOnPlatform = false; // Reset because the platform is now pass-through
                }
                timeOnPlatform = 0f; // Reset the timer if player is not on the platform
                
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) // Ensure it's the player
        {
            // Check if the player is standing on top of the platform
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // The normal points from the platform to the player
                // If the normal is pointing upwards, the player is on top
                if (contact.normal.y < -0.9f)
                {
                    playerOnPlatform = true;
                    return; // No need to check other contacts
                }
            }
            // If none of the normals are pointing upwards, the player is not on top
            playerOnPlatform = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) // Ensure it's the player
        {
            playerOnPlatform = false;
        }
    }
}
