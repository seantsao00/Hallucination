using UnityEngine;

public class CharacterHorizontalMove : MonoBehaviour {
    Character character;
    Rigidbody2D rb;

    void Start() {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (!InputManager.Control.Character.HorizontalMove.enabled) return;
        float direction = InputManager.Instance.CharacterHorizontalMove;
        rb.velocity = new Vector2(direction * character.CurrentMovement.HorizontalSpeed + character.CurrentMovement.SpringSpeed, rb.velocity.y);
    }
}
