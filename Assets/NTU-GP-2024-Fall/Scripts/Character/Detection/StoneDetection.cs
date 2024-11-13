using UnityEngine;

public class StoneDetection : MonoBehaviour {
    Character character;

    void Awake() {
        character = transform.parent.GetComponent<Character>();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Stone") && character.StoneWithinRange == null) {
            character.StoneWithinRange = collider.GetComponent<Stone>();
        }
    }
    void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Stone") && character.StoneWithinRange == collider.GetComponent<Stone>())
            character.StoneWithinRange = null;
    }
}
