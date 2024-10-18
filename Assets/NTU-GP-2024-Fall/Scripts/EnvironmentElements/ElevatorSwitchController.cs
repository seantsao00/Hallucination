using UnityEngine;
using System.Collections; // Needed for Coroutines

public class ElevatorSwitchController : SwitchController {
    public Transform targetPosition; // The position to move the player to
    public float moveSpeed = 2f; // Speed at which the player moves to the target
    public WorldSwitchManager worldSwitchManager;
    public TipManager tipManager;
    bool isMovingCharacter = false; // To prevent multiple coroutines from starting
    Transform characterTransform; // Reference to the player's transform

    Rigidbody2D characterRigidbody; // Reference to the player's Rigidbody2D
    bool originalIsKinematic; // To store the player's original isKinematic state

    // Reference to the player's controller script
    Character character;

    protected override void Update() {
        base.Update(); // Call the base class Update method
    }

    protected override void OnSwitchActivated() {
        if (!isMovingCharacter) {
            tipManager.ShowTip(false, "");
            // Start moving the player to the target position
            StartCoroutine(MoveCharacterToTarget());
        }
    }

    // Coroutine to move the player to the target position
    private IEnumerator MoveCharacterToTarget() {
        isMovingCharacter = true;
        worldSwitchManager.Disable();
        // Get the player's Rigidbody2D component
        if (characterTransform != null) {
            if (character != null) {
                character.CurrentState = new CharacterState.Transporting(character);
            } else {
                print("Not found");
            }
        }
        // While the player hasn't reached the target position
        while (Vector3.Distance(characterTransform.position, targetPosition.position) > 0.1f) {
            // Move the player towards the target position
            characterTransform.position = Vector3.MoveTowards(
                characterTransform.position,
                targetPosition.position,
                moveSpeed * Time.deltaTime
            );
            yield return null; // Wait for the next frame
        }

        // Ensure the player is exactly at the target position
        characterTransform.position = targetPosition.position;
        if (character != null) {
            // Set the player's climbing state back to false
            character.CurrentState = new CharacterState.Free(character);
        }


        // Change the switch color back to the original color
        spriteRenderer.color = originalColor;

        isMovingCharacter = false;
        worldSwitchManager.Enable();
    }

    protected override void OnTriggerEnter2D(Collider2D other) {
        base.OnTriggerEnter2D(other); // Call the base class method
        tipManager.ShowTip(true, "Press E to use the elevator");

        if (other.CompareTag("Player")) {
            character = other.GetComponent<Character>();
            characterTransform = other.transform; // Cache the player's transform
        }
    }

    protected override void OnTriggerExit2D(Collider2D other) {
        tipManager.ShowTip(false, "");
        base.OnTriggerExit2D(other); // Call the base class method

        if (other.CompareTag("Player")) {
            // playerTransform = null; // Clear the cached transform
        }
    }
}
