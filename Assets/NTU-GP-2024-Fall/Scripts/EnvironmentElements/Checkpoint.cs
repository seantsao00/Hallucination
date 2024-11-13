using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICheckpointControllable {
    void Activate();
    void Deactivate();
    GameObject GetGameObject();
}

class Checkpoint : MonoBehaviour {
    bool reached;
    public GameObject[] activateGameObject;
    public GameObject[] deactivateGameObject;

    List<ICheckpointControllable> activateCheckpointControllable = new List<ICheckpointControllable>();
    List<ICheckpointControllable> deactivateCheckpointControllable = new List<ICheckpointControllable>();

    public bool switchWorld;
    public bool enableSwitchWorld;

    public string dialogueName;

    void Awake() {
        reached = false;
        foreach (var obj in activateGameObject) {
            ICheckpointControllable controllable = obj.GetComponent<ICheckpointControllable>();
            if (controllable != null) {
                activateCheckpointControllable.Add(controllable);
            } else {
                Debug.LogWarning($"Omit object that has not implemented ICheckpointControllable: {obj}");
            }
        }
        foreach (var obj in deactivateGameObject) {
            ICheckpointControllable controllable = obj.GetComponent<ICheckpointControllable>();
            if (controllable != null) {
                deactivateCheckpointControllable.Add(controllable);
            } else {
                Debug.LogWarning($"Omit object that has not implemented ICheckpointControllable: {obj}");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        print("triggered");
        if (other.CompareTag("Player") && !reached) {
            reached = true;
            foreach (var controllable in activateCheckpointControllable) {
                controllable.Activate();
            }
            foreach (var controllable in deactivateCheckpointControllable) {
                controllable.Deactivate();
            }
            if (switchWorld) WorldSwitchManager.Instance.SwitchWorld();
            if (enableSwitchWorld) WorldSwitchManager.Instance.Enable();
            else WorldSwitchManager.Instance.Disable();

            if (dialogueName != "") {
                DialogueManager.Instance.StartDialogue(dialogueName);
            }
        }
    }

    
}

