using UnityEngine;

public class Wind : MonoBehaviour {
    [SerializeField] float speed = 3.5f;
    CharacterHorizontalMove horizontalMove;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            horizontalMove = other.GetComponent<CharacterHorizontalMove>();
            horizontalMove.WindBonusSpeed = speed;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            horizontalMove = other.GetComponent<CharacterHorizontalMove>();
            horizontalMove.WindBonusSpeed = 0;
        }
    }
}