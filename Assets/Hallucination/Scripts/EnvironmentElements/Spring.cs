using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour {
    [SerializeField] float verticalSpeed;
    [SerializeField] float horizontalSpeed;
    [SerializeField] float springDuration = 0.5f;
    Rigidbody2D characterRb;
    CharacterStateController characterStateController;
    Coroutine springCoroutine;
    CharacterHorizontalMove horizontalMove;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            characterRb = other.GetComponent<Rigidbody2D>();
            horizontalMove = other.GetComponent<CharacterHorizontalMove>();
            characterStateController = other.GetComponent<CharacterStateController>();
            other.GetComponent<CharacterDash>()?.ResetDash();

            springCoroutine = StartCoroutine(LaunchSpring());
            horizontalMove.CurrentSpring = this;
        }
    }

    IEnumerator LaunchSpring() {
        if (horizontalSpeed != 0) {
            characterStateController.AddState(CharacterState.HorizontalSpringFlying);
            characterRb.velocity = new Vector2(horizontalSpeed, verticalSpeed);
        } else {
            characterRb.velocity = new Vector2(characterRb.velocity.x, verticalSpeed);
        }
        yield return new WaitForSeconds(springDuration);
        springCoroutine = null;
        characterStateController.RemoveState(CharacterState.HorizontalSpringFlying);
        horizontalMove.CurrentSpring = null;
    }

    public void StopSpringHorizontalSpeed() {
        if (springCoroutine != null) {
            StopCoroutine(springCoroutine);
            springCoroutine = null;
            characterStateController.RemoveState(CharacterState.HorizontalSpringFlying);
            horizontalMove.CurrentSpring = null;
        }
    }
}
