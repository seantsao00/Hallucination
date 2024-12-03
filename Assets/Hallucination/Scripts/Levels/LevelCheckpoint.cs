using UnityEngine;
using UnityEngine.Events;

public class LevelCheckpoint : MonoBehaviour {
    public UnityEvent<LevelCheckpoint> CheckpointCompleted;
    bool reached;
    [SerializeField] bool activeAtStart = true;

    void Awake() {
        gameObject.SetActive(activeAtStart);
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        if (!reached && other.CompareTag("Player")) {
            reached = true;
            CheckpointCompleted?.Invoke(this);
        }
    }
}
