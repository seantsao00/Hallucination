using UnityEngine;

public class CharacterClimb : MonoBehaviour {
    // [SerializeField] LayerMask AheadGroundLayerMask;
    [SerializeField] float climbingSpeed = 5f;
    Character character;
    Rigidbody2D rb;
    CharacterStateController characterStateController;

    void Awake() {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        characterStateController = GetComponent<CharacterStateController>();
    }

    void Update() {
        float direction = InputManager.Instance.CharacterVerticalMove;

        if (!characterStateController.HasState(CharacterState.Climbing)) {
            if (direction > 0 && character.IsBodyOnClimbable) StartClimb();
            if (direction < 0 && character.IsStandOnClimbable) StartClimb();
        }
        if (characterStateController.HasState(CharacterState.Climbing)) {
            if (character.IsGrounded) {
                if (direction >= 0 && !character.IsBodyOnClimbable) BackToNormal();
                if (direction <= 0 && !character.IsStandOnClimbable) BackToNormal();
            }
            if (!character.IsBodyOnClimbable && !character.IsStandOnClimbable) BackToNormal();
        }
        if (characterStateController.HasState(CharacterState.Climbing)) {
            rb.velocity = new Vector2(0, direction * climbingSpeed);
        }
    }

    void StartClimb() {
        rb.velocity = new Vector2(0, 0);
        gameObject.layer = LayerMask.NameToLayer("AheadGround");
        characterStateController.AddState(CharacterState.Climbing);
    }

    void BackToNormal() {
        gameObject.layer = 1 << 0; // default layer (0)
        characterStateController.RemoveState(CharacterState.Climbing);
    }
}
