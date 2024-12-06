using UnityEngine;

public class CheckpointBoundary : MonoBehaviour {
    [SerializeField] LevelCheckpoint checkpoint;
    [SerializeField] bool activeIfCheckpointCompleted;
    [SerializeField] string touchBoundaryDialogueName = "Bear Touch Boundary";

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
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!string.IsNullOrEmpty(touchBoundaryDialogueName) && collision.gameObject.CompareTag("Player")) {
            DialogueManager.Instance.StartDialogue(touchBoundaryDialogueName);
        }
    }
}
