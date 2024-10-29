using UnityEngine;

public class CharacterClimb : MonoBehaviour {
    Character character;

    void Start() {
        character = GetComponent<Character>();
    }

    void Update() {
        float direction = Input.GetAxisRaw("Vertical");

        if (character.CurrentState is CharacterState.Free && character.OverlappedClimbable != null && direction != 0) {
            character.CurrentState = new CharacterState.Climbing();
        }
        if (character.CurrentState is CharacterState.Climbing) {
            if (character.IsGrounded && direction == 0)
                character.CurrentState = new CharacterState.Free();
            if (character.OverlappedClimbable == null)
                character.CurrentState = new CharacterState.Free();
        }
        if (character.CurrentState is CharacterState.Climbing)
            character.Rb.velocity = new Vector2(0, direction * character.CurrentMovement.ClimbingSpeed);
    }
}
