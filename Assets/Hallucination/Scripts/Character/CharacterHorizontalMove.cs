using UnityEngine;

public class CharacterHorizontalMove : MonoBehaviour {
    CharacterStateController characterStateController;
    Rigidbody2D rb;
    [SerializeField] float normalSpeed = 5f;
    [SerializeField] float grabSpeed = 3f;
    public float CurrentBasicSpeed { get; private set; }
    [HideInInspector] public float WindBonusSpeed;

    public void ResetBasicSpeed() => CurrentBasicSpeed = normalSpeed;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        characterStateController = GetComponent<CharacterStateController>();
        CurrentBasicSpeed = normalSpeed;
        characterStateController.OnStateChanged += HandleStateChange;
    }

    void HandleStateChange(CharacterState state, bool added) {
        if (characterStateController.HasState(CharacterState.Grabbing)) {
            CurrentBasicSpeed = grabSpeed;
            return;
        }
        CurrentBasicSpeed = normalSpeed;
    }

    void FixedUpdate() {
        if (
            !InputManager.Control.Character.HorizontalMove.enabled ||
            characterStateController.HasState(CharacterState.HorizontalSpringFlying)
        ) {
            characterStateController.RemoveState(CharacterState.Walking);
            return;
        }
        float direction = InputManager.Instance.CharacterHorizontalMove;
        rb.velocity = new Vector2(direction * CurrentBasicSpeed + WindBonusSpeed, rb.velocity.y);
        if (direction == 0) {
            characterStateController.RemoveState(CharacterState.Walking);
        } else {
            characterStateController.AddState(CharacterState.Walking);
        }
    }

    void OnDestroy() {
        characterStateController.OnStateChanged -= HandleStateChange;
    }
}
