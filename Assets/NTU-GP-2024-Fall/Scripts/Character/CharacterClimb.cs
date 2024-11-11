using UnityEngine;

public class CharacterClimb : MonoBehaviour {
    Character character;

    void Start() {
        character = GetComponent<Character>();
    }

    void Update() {
        float direction = InputManager.Instance.CharacterVerticalMove;

        if (character.CurrentState is CharacterState.Free && character.OverlappedClimbable != null && direction != 0) {
            character.CurrentState = new CharacterState.Climbing();
            GetComponent<Animator>().SetBool("Climb", true);
        }
        if (character.CurrentState is CharacterState.Climbing) {
            if (character.IsGrounded && direction == 0) {
                character.CurrentState = new CharacterState.Free();
                GetComponent<Animator>().SetBool("Climb", false);
            }
            if (character.OverlappedClimbable == null) {
                character.CurrentState = new CharacterState.Free();
                GetComponent<Animator>().SetBool("Climb", false);
            }
        }
        if (character.CurrentState is CharacterState.Climbing)
            character.Rb.velocity = new Vector2(0, direction * character.CurrentMovement.ClimbingSpeed);
    }
}
