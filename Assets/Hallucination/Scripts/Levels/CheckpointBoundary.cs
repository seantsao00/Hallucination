using UnityEngine;

public class CheckpointBoundary : MonoBehaviour {
    [SerializeField] LevelCheckpoint checkpoint;
    [SerializeField] bool enableIfCheckpointCompleted;
    

    void Awake() {
        if (enableIfCheckpointCompleted) {
            gameObject.SetActive(false);
            checkpoint.CheckpointCompleted.AddListener(delegate { gameObject.SetActive(true); });
        } else {
            gameObject.SetActive(true);
            checkpoint.CheckpointCompleted.AddListener(delegate { gameObject.SetActive(false); });
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        DialogueManager.Instance.StartDialogue("Bear Touch Boundary", true);
    }
}
