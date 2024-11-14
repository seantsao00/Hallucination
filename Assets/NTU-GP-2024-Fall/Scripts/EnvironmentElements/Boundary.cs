using UnityEngine;

public class Boundary : MonoBehaviour {
    [SerializeField] Checkpoint checkpoint;
    [SerializeField] bool enableIfCheckpointCompleted;

    void Awake() {
        if (enableIfCheckpointCompleted) {
            checkpoint.CheckpointCompleted += enable;
            disable(checkpoint);
        }
        else {
            checkpoint.CheckpointCompleted += disable;
            enable(checkpoint);
        }
    }

    void enable(Checkpoint checkpoint) {
        print("Boundary enabled");
        gameObject.SetActive(true);
    }

    void disable(Checkpoint checkpoint) {
        print("Boundary disabled");
        gameObject.SetActive(false);
    }
    
}
