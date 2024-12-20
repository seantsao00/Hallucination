using UnityEngine;

public class Spring: MonoBehaviour {
    protected Rigidbody2D characterRb;
    protected Character character;
    protected CharacterStateController characterStateController;

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            character = other.GetComponent<Character>();
            characterRb = other.GetComponent<Rigidbody2D>();
            other.GetComponent<CharacterDash>()?.ResetDash();
            character.StopSpringHorizontalSpeed();
            characterStateController = other.GetComponent<CharacterStateController>();
            characterStateController.RemoveState(CharacterState.PreReleaseJumping);
            characterStateController.AddState(CharacterState.InsideSpring);
            GetComponent<Animator>().SetBool("Trigger", true);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            characterStateController.RemoveState(CharacterState.InsideSpring);
            GetComponent<Animator>().SetBool("Trigger", false);
        }
    }
}
