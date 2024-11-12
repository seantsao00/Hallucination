using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InteractableObjectBase: MonoBehaviour {
    public abstract void Interact(InputAction.CallbackContext context);

    protected virtual void OnEnable() {
        InputManager.Control.Character.Interact.performed += Interact;
    }

    protected virtual void OnDisable() {
        InputManager.Control.Character.Interact.performed -= Interact;
    }
}

