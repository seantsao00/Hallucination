using UnityEngine;
using Cinemachine;

public class CameraCheckpoint : MonoBehaviour {
    private CinemachineVirtualCamera checkpointCamera;
    
    void Awake() {
        checkpointCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        checkpointCamera.gameObject.SetActive(false);
    }

    void Start() {
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
