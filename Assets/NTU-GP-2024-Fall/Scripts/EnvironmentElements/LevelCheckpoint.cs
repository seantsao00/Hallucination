using UnityEngine;
using UnityEngine.Events;

public class LevelCheckpoint : MonoBehaviour
{
    public UnityEvent CheckpointCompleted;

    public int changeLevel;
    public bool switchWorld;
    public bool enableSwitchWorld;

    public string dialogueName;

    private bool reached;

    void Awake() {
        reached = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !reached) {
            reached = true;
            CheckpointCompleted?.Invoke();
            if (changeLevel != -1) {
                LevelManager.Instance.ChangeLevel(changeLevel);
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
