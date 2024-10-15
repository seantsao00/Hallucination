using UnityEngine;
using System.Collections; // Needed for Coroutines

public class ElevatorSwitchController : SwitchController
{
    public Transform targetPosition; // The position to move the player to
    public float moveSpeed = 2f; // Speed at which the player moves to the target

    private bool isMovingPlayer = false; // To prevent multiple coroutines from starting
    private Transform playerTransform; // Reference to the player's transform

    private Rigidbody2D playerRigidbody; // Reference to the player's Rigidbody2D
    private bool originalIsKinematic; // To store the player's original isKinematic state

    // Reference to the player's controller script
    private Character player;

    protected override void Update()
    {
        base.Update(); // Call the base class Update method
    }

    protected override void OnSwitchActivated()
    {
        if (!isMovingPlayer)
        {
            // Start moving the player to the target position
            StartCoroutine(MovePlayerToTarget());
        }
    }

    // Coroutine to move the player to the target position
    private IEnumerator MovePlayerToTarget()
    {
        isMovingPlayer = true;

        // Get the player's Rigidbody2D component
        if (playerTransform != null)
        {
            
            playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                // Store the original isKinematic state
                originalIsKinematic = playerRigidbody.isKinematic;

                // Set isKinematic to true to disable physics interaction
                playerRigidbody.isKinematic = true;
            }
            

            // Get the PlayerController component
            player = playerTransform.GetComponent<Character>();
            if (player != null)
            {
                player.IsControllable = false;
            } else {
                print("Not found");
            }
        }
        print(player.IsControllable);
        // While the player hasn't reached the target position
        while (Vector3.Distance(playerTransform.position, targetPosition.position) > 0.1f)
        {
            // Move the player towards the target position
            playerTransform.position = Vector3.MoveTowards(
                playerTransform.position,
                targetPosition.position,
                moveSpeed * Time.deltaTime
            );
            yield return null; // Wait for the next frame
        }

        // Ensure the player is exactly at the target position
        playerTransform.position = targetPosition.position;

        // Restore the player's original isKinematic state
        
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = originalIsKinematic;
        }
        

        
        if (player != null)
        {
            // Set the player's climbing state back to false
            player.IsControllable = true;
        }
        print(player.IsControllable);

        // Change the switch color back to the original color
        spriteRenderer.color = originalColor;

        isMovingPlayer = false;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other); // Call the base class method

        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform; // Cache the player's transform
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other); // Call the base class method

        if (other.CompareTag("Player"))
        {
            // playerTransform = null; // Clear the cached transform
        }
    }
}
