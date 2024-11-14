using UnityEngine;

class Checkpoint : MonoBehaviour {
    bool reached;

    public delegate void CheckpointCompletedHandler(Checkpoint checkpoint);
    public event CheckpointCompletedHandler CheckpointCompleted;

    public bool switchWorld;
    public bool enableSwitchWorld;

    public string dialogueName;

    void Awake() {
        reached = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !reached) {
            reached = true;
            CheckpointCompleted?.Invoke(this);
            if (switchWorld) WorldSwitchManager.Instance.SwitchWorld();
            if (enableSwitchWorld) WorldSwitchManager.Instance.Enable();
            else WorldSwitchManager.Instance.Disable();

            if (dialogueName != "") {
                DialogueManager.Instance.StartDialogue(dialogueName);
            }
        }
    }
}

