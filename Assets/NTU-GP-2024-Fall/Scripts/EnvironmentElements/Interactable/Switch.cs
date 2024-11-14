using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public interface ISwitchControlled {
    public void SetState(int index);
}

public class Switch : InteractableObjectBase {
    [SerializeField] GameObject[] controlledObjects;
    [SerializeField] int numberOfStates = 2;
    [SerializeField] int startStateIndex;
    int currentStateIndex;
    bool isPlayerInRange;

    List<ISwitchControlled> controlledList = new List<ISwitchControlled>();

    void Awake() {
        currentStateIndex = startStateIndex;
        foreach (var obj in controlledObjects) {
            ISwitchControlled controlled = obj.GetComponent<ISwitchControlled>();
            if (controlled != null) {
                controlledList.Add(controlled);
                controlled.SetState(currentStateIndex);
            } else {
                Debug.LogWarning($"Omit object that has not implemented ISwitchControlled: {obj}");
            }
        }
    }

    override public void Interact(InputAction.CallbackContext context) {
        if (!isPlayerInRange) return;
        currentStateIndex = (currentStateIndex + 1) % numberOfStates;
        foreach (var controlled in controlledList) {
            controlled.SetState(currentStateIndex);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayerInRange = false;
        }
    }
}
