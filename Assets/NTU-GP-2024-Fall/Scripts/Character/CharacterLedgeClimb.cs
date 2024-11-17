using System.Collections;
using UnityEngine;

public class CharacterLedgeClimb : MonoBehaviour {
    [SerializeField] Vector2 ledgeClimbOffset = new Vector2(0.8f, 1f);
    Vector2 destination;
    Character character;
    Rigidbody2D rb;
    float eps = 1e-4f;
    bool isClimbingLedge;
    CharacterStateController characterStateController;

    void Awake() {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        characterStateController = GetComponent<CharacterStateController>();
    }

    void Update() {
        if (!InputManager.Control.Character.HorizontalMove.enabled) return;
        if (isClimbingLedge) return;
        if (Mathf.Abs(rb.velocity.y) > eps) return;
        if (!character.IsGrounded) return;
        if (character.IsLedgeDetected == false) return;
        float direction = InputManager.Instance.CharacterHorizontalMove;
        if (direction != 0) LedgeClimb();
    }

    void LedgeClimb() {
        isClimbingLedge = true;
        Vector2 currentPosition = transform.position;
        if (character.FacingDirection.x > 0) destination = currentPosition + ledgeClimbOffset;
        else destination = currentPosition + new Vector2(-ledgeClimbOffset.x, ledgeClimbOffset.y);
        characterStateController.AddState(CharacterState.LedgeClimbing);
        GetComponent<Animator>().SetBool("LedgeClimb", true);       // I'm not sure is it okay to put here
        rb.bodyType = RigidbodyType2D.Static;
    }

    // Invoked by the animator
    void LedgeClimbOver() {
        transform.position = destination;
        characterStateController.RemoveState(CharacterState.LedgeClimbing);
        GetComponent<Animator>().SetBool("LedgeClimb", false);
        rb.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(FinishClimb(0.05f));
    }

    IEnumerator FinishClimb(float shortDelay) {
        yield return new WaitForSeconds(shortDelay);
        isClimbingLedge = false;
    }

}