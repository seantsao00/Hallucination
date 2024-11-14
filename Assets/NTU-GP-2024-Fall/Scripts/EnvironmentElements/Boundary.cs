using UnityEngine;

public class Boundary : MonoBehaviour {
    [SerializeField] Checkpoint checkpoint;

    void Awake() {
        checkpoint.CheckpointActivated += Activate;
        checkpoint.CheckpointDeactivated += Deactivate;
    }

    void Activate(Checkpoint checkpoint) {
        gameObject.SetActive(true);
    }

    void Deactivate(Checkpoint checkpoint) {
        gameObject.SetActive(false);
    }
}
