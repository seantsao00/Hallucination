using UnityEngine;
using UnityEngine.Events;

public class LevelCheckpoint : MonoBehaviour {
    public UnityEvent<LevelCheckpoint> CheckpointCompleted;
    bool reached;
    void OnTriggerEnter2D(Collider2D other) {
        if (!reached && other.CompareTag("Player")) {
            reached = true;
            CheckpointCompleted?.Invoke(this);
        }
    }
}
