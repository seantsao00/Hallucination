using UnityEngine;

class Checkpoint : MonoBehaviour {
    bool reached;

    public delegate void ActivateCheckpointHandler(Checkpoint checkpoint);
    public event ActivateCheckpointHandler CheckpointActivated;
    public delegate void DeactivateCheckpointHandler(Checkpoint checkpoint);
    public event DeactivateCheckpointHandler CheckpointDeactivated;

    public bool switchWorld;
    public bool enableSwitchWorld;

    public string dialogueName;

    void Awake() {
        reached = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        print("triggered");
        if (other.CompareTag("Player") && !reached) {
            reached = true;
            CheckpointActivated?.Invoke(this);
            CheckpointDeactivated?.Invoke(this);
            if (switchWorld) WorldSwitchManager.Instance.SwitchWorld();
            if (enableSwitchWorld) WorldSwitchManager.Instance.Enable();
            else WorldSwitchManager.Instance.Disable();

            if (dialogueName != "") {
                DialogueManager.Instance.StartDialogue(dialogueName);
            }
        }
    }

    
}

