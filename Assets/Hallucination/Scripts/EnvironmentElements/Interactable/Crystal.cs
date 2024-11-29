using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crystal : InteractableObjectBase {
    bool isPlayerInRange;
    GameObject character;
    override public void Interact(InputAction.CallbackContext context) {
        if (character != null) character.GetComponent<RadiusDuplicator>().DuplicateObjectsInRadius();
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            TipManager.Instance.ShowInteractableObjectTip(gameObject);
            isPlayerInRange = true;
            character = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            TipManager.Instance.CloseInteractableObjectTip(gameObject);
            isPlayerInRange = false;
            character = null;
        }
    }
}
