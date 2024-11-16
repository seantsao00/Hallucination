using UnityEngine;

public class StoneDetection : MonoBehaviour {
    Character character;
    CharacterGrabStone characterGrabStone;

    void Awake() {
        character = transform.parent.GetComponent<Character>();
        characterGrabStone = transform.parent.GetComponent<CharacterGrabStone>();
        if (characterGrabStone == null) {
            Debug.LogWarning(
                $"No {typeof(CharacterGrabStone).Name} component is found in the parent object." +
                $"Automatically disable the {typeof(StoneDetection).Name} component."
            );
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Stone") && character.StoneWithinRange == null) {
            character.StoneWithinRange = collider.GetComponent<Stone>();
        }
    }
    void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Stone") && character.StoneWithinRange == collider.GetComponent<Stone>()) {
            if (characterGrabStone.IsLeashingStone) {
                characterGrabStone.UnleashStone();
            }
            character.StoneWithinRange = null;
        }
    }
}
