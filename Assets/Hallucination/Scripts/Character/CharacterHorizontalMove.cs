using UnityEngine;

public class CharacterHorizontalMove : MonoBehaviour {
    Character character;
    CharacterStateController characterStateController;
    Rigidbody2D rb;

    void Start() {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        characterStateController = GetComponent<CharacterStateController>();
    }

    void Update() {
        if (!InputManager.Control.Character.HorizontalMove.enabled) {
            characterStateController.RemoveState(CharacterState.Walking);
            return;
        }
        float direction = InputManager.Instance.CharacterHorizontalMove;
        rb.velocity = new Vector2(direction * character.CurrentMovement.HorizontalSpeed + character.CurrentMovement.SpringSpeed, rb.velocity.y);
        if (direction == 0) {
            characterStateController.RemoveState(CharacterState.Walking);
        } else {
            characterStateController.AddState(CharacterState.Walking);
        }
    }
}
