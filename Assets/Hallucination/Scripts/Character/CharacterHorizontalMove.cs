using UnityEngine;

public class CharacterHorizontalMove : MonoBehaviour {
    CharacterStateController characterStateController;
    Rigidbody2D rb;
    [SerializeField] float normalSpeed = 5f;
    public float CurrentBasicSpeed;
    public float SpringBonusSpeed;

    public void ResetBasicSpeed() => CurrentBasicSpeed = normalSpeed;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        characterStateController = GetComponent<CharacterStateController>();
        CurrentBasicSpeed = normalSpeed;
    }

    void Update() {
        if (!InputManager.Control.Character.HorizontalMove.enabled) {
            characterStateController.RemoveState(CharacterState.Walking);
            return;
        }
        float direction = InputManager.Instance.CharacterHorizontalMove;
        rb.velocity = new Vector2(direction * CurrentBasicSpeed + SpringBonusSpeed, rb.velocity.y);
        if (direction == 0) {
            characterStateController.RemoveState(CharacterState.Walking);
        } else {
            characterStateController.AddState(CharacterState.Walking);
        }
    }
}
