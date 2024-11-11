using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterGrab : MonoBehaviour {
    Character character;
    Stone stone;

    void Awake() {
        character = GetComponent<Character>();
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
        if (context.performed) {
            character.CurrentState = new CharacterState.GrabbingMovable();
            stone.SetSpeed(character.CurrentMovement.HorizontalSpeed);
            stone.Leash();
        }
        if (context.canceled) {
            character.CurrentState = new CharacterState.Free();
            stone.Unleash();
        }
    }

    void Update() {
        GameObject movableGameObject = character.FacedMovableGameObject;
        if (movableGameObject == null) return;
        stone = movableGameObject.GetComponent<Stone>();
    }


}
