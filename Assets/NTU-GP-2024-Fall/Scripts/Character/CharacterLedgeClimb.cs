using UnityEditor.Callbacks;
using UnityEngine;

public class CharacterLedgeClimb : MonoBehaviour {
    [SerializeField] Vector2 LedgeClimbOffset = new Vector2(0.8f, 1f);
    [SerializeField] float testHaltInterval = 0.4f;
    Vector2 destination;
    Character character;
    bool isClimbingLedge;

    void Awake() {
        character = GetComponent<Character>();
    }

    void Update() {
        if (isClimbingLedge) return;
        if (character.LedgeDetected == false) return;
        float direction = InputManager.Instance.CharacterHorizontalMove;
        if (direction != 0) LedgeClimb();
    }

    void LedgeClimb() {
        isClimbingLedge = true;
        InputManager.Instance.DisableAllInput();
        Vector2 currentPosition = transform.position;
        if (character.FacingDirection.x > 0) destination = currentPosition + LedgeClimbOffset;
        else destination = currentPosition + new Vector2(-LedgeClimbOffset.x, LedgeClimbOffset.y);
        // TODO: We should invoke ledge climb animation in the animator.
        Invoke("LedgeClimbOver", testHaltInterval);
        // TODO: We should invoke the function LedgeClimbOver in the animator.
    }

    void LedgeClimbOver() {
        transform.position = destination;
        InputManager.Instance.SetNormalMode();
        isClimbingLedge = false;
    }
    
}