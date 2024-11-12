using UnityEngine;

public class Spring : MonoBehaviour
{
    public float verticalLaunchSpeed;
    public float horizontalLaunchSpeed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            Character character = other.GetComponent<Character>();
            if (playerRb != null)
            {
                Vector2 launchVelocity = playerRb.velocity;
                character.CurrentMovement.LaunchSpring(horizontalLaunchSpeed);
                launchVelocity.y = verticalLaunchSpeed;
                
                playerRb.velocity = launchVelocity;
                
                print(playerRb.velocity);
            }
        }
        
    }
}
