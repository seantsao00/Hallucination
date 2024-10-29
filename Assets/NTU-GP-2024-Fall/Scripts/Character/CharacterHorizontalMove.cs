using UnityEngine;

public class CharacterHorizontalMove : MonoBehaviour {
    Character character;

    void Start() {
        character = GetComponent<Character>();
    }

    void Update() {
        if (!character.CurrentMovement.IsHorizontalMoveEnabled) return;
        float direction = Input.GetAxisRaw("Horizontal");
        character.Rb.velocity = new Vector2(direction * character.CurrentMovement.HorizontalSpeed, character.Rb.velocity.y);
    }
}
