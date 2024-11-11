using UnityEngine;

public class CharacterHorizontalMove : MonoBehaviour {
    Character character;
    Rigidbody2D rb;

    void Start() {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (!character.CurrentMovement.IsHorizontalMoveEnabled) return;
        float direction = InputManager.Instance.CharacterHorizontalMove;
        rb.velocity = new Vector2(direction * character.CurrentMovement.HorizontalSpeed, rb.velocity.y);
    }
}
