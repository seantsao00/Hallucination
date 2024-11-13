using UnityEngine;

public class CharacterClimb : MonoBehaviour {
    Character character;
    Rigidbody2D rb;

    void Start() {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        float direction = InputManager.Instance.CharacterVerticalMove;

        if (character.CurrentState is CharacterState.Free) {
            if (direction > 0 && character.IsBodyOnClimbable) StartClimb();
            if (direction < 0 && character.IsStandOnClimbable) StartClimb();
        }
        if (character.CurrentState is CharacterState.Climbing) {
            if (character.IsGrounded) {
                if (direction >= 0 && !character.IsBodyOnClimbable) BackToNormal();
                if (direction <= 0 && !character.IsStandOnClimbable) BackToNormal();
            }
            if (!character.IsBodyOnClimbable && !character.IsStandOnClimbable) BackToNormal();
        }
        if (character.CurrentState is CharacterState.Climbing) {
            rb.velocity = new Vector2(0, direction * character.CurrentMovement.ClimbingSpeed);
        }
    }

    void StartClimb() {
        character.CurrentState = new CharacterState.Climbing();
        GetComponent<Animator>().SetBool("Climb", true);
    }

    void BackToNormal() {
        character.CurrentState = new CharacterState.Free();
        GetComponent<Animator>().SetBool("Climb", false);
    }
}
