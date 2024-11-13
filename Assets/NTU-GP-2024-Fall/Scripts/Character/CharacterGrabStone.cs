using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CharacterGrabStone : MonoBehaviour {
    [SerializeField] float grabSpeed = 3f;
    Character character;
    Stone stone => character.StoneWithinRange;
    public bool IsLeashingStone { get; private set; }
    CharacterStateController characterStateController;

    void Awake() {
        character = GetComponent<Character>();
        characterStateController = GetComponent<CharacterStateController>();
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
        stone.SetSpeed(grabSpeed);
        stone.Leash();
        character.CurrentMovement.HorizontalSpeed = grabSpeed;
        IsLeashingStone = true;
        characterStateController.AddState(CharacterState.Grabbing);
    }

    public void UnleashStone() {
        stone.Unleash();
        character.CurrentMovement.HorizontalSpeed = character.MovementAttributes.NormalHorizontalSpeed;
        IsLeashingStone = false;
        characterStateController.RemoveState(CharacterState.Grabbing);
    }
}