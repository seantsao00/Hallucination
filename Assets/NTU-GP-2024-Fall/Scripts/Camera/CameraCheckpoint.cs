using UnityEngine;
using Cinemachine;

public class CameraCheckpoint : MonoBehaviour {
    private CinemachineVirtualCamera checkpointCamera;

    void Start() {
        checkpointCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        // Make sure the checkpoint camera is off at the beginning
        // The camera will be turned on when the player enters the checkpoint
        checkpointCamera.gameObject.SetActive(false);
        // Make sure the checkpoint camera is always on top
        checkpointCamera.Priority = 100;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            checkpointCamera.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            checkpointCamera.gameObject.SetActive(false);
        }
    }
}
