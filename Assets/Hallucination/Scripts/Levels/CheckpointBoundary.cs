using UnityEngine;

public class CheckpointBoundary : MonoBehaviour {
    [SerializeField] LevelCheckpoint checkpoint;
    [SerializeField] bool activeIfCheckpointCompleted;
    [SerializeField] string touchBoundaryDialogueName = "Bear Touch Boundary";
    int lockCount = 0;
    void Awake() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            spriteRenderer.enabled = false;
        }
        if (checkpoint != null) {
            if (activeIfCheckpointCompleted) {
                gameObject.SetActive(false);
                checkpoint?.CheckpointCompleted.AddListener(delegate { gameObject.SetActive(true); });
            } else {
                gameObject.SetActive(true);
                checkpoint?.CheckpointCompleted.AddListener(delegate { gameObject.SetActive(false); });
            }
        }
        WorldSwitchManager.Instance.WorldStartSwitching.AddListener(SelfLock);
        WorldSwitchManager.Instance.WorldStartSwitching.AddListener(SelfUnlock);
    }
    void SelfLock() {
        lockCount++;
    }
    void SelfUnlock() {
        lockCount--;
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if (!string.IsNullOrEmpty(touchBoundaryDialogueName) && collision.gameObject.CompareTag("Player")
            && lockCount == 0) {
            DialogueManager.Instance.StartDialogue(touchBoundaryDialogueName);
        }
    }
}
