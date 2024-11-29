using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crystal : InteractableObjectBase {
    bool isPlayerInRange;
    public CharacterNeighborDetector detector;
    override public void Interact(InputAction.CallbackContext context) {
        if (detector != null) {
            duplicatedNeighborObjects();
        }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            TipManager.Instance.ShowInteractableObjectTip(gameObject);
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            TipManager.Instance.CloseInteractableObjectTip(gameObject);
            isPlayerInRange = false;
        }
    }
    
    void duplicatedNeighborObjects() {
        GameObject[] neighbors = detector.NeighborObjects.ToArray();
        foreach (var neighbor in neighbors) {
            Vector2 relativePosition = detector.GetRelativePosition(neighbor);
            Vector2 newPosition = (Vector2)gameObject.transform.position + relativePosition;
            Transform parent = gameObject.transform.parent;
            GameObject duplicatedObject = Instantiate(neighbor, newPosition, neighbor.transform.rotation, parent);

            duplicatedObject.name = neighbor.name + "_Copy";

            Debug.Log($"Duplicated {neighbor.name} to new position {newPosition}");
        }
    }
}
