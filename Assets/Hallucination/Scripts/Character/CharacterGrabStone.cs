using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterGrabStone : MonoBehaviour {
    Character character;
    CharacterHorizontalMove horizontalMove;
    Stone stone => character.StoneWithinRange;
    public bool IsLeashingStone { get; private set; }
    CharacterStateController characterStateController;
    float grabSpeed;

    void Awake() {
        character = GetComponent<Character>();
        characterStateController = GetComponent<CharacterStateController>();
        horizontalMove = GetComponent<CharacterHorizontalMove>();
    }

    void OnEnable() {
        InputManager.Control.Character.Grab.performed += Grab;
        InputManager.Control.Character.Grab.canceled += Grab;
    }
    void OnDisable() {
        InputManager.Control.Character.Grab.performed -= Grab;
        InputManager.Control.Character.Grab.canceled -= Grab;
    }

    void Grab(InputAction.CallbackContext context) {
        if (stone == null) return;
        if (!IsLeashingStone && context.performed) { LeashStone(); }
        if (IsLeashingStone && context.canceled) { UnleashStone(); }
    }

    void LeashStone() {
        stone.Leash();
        IsLeashingStone = true;
        characterStateController.AddState(CharacterState.Grabbing);
        grabSpeed = horizontalMove.CurrentBasicSpeed;
        stone.SetSpeed(grabSpeed);
    }

    public void UnleashStone() {
        stone.Unleash();
        horizontalMove.ResetBasicSpeed();
        IsLeashingStone = false;
        characterStateController.RemoveState(CharacterState.Grabbing);
    }
}
